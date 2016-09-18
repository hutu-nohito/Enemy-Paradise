using UnityEngine;
using System.Collections;

public class SaberVR : Magic_Parameter {

    public GameObject[] bullet_Prefab;//弾のプレハブ

    private GameObject Player;
    private Magic_Controller MC;
    private Player_ControllerVR pcVR;

    //近接ホーミング用 ホーミングはないけど一応入れとく
    //private Vision vision;
    //private bool flag_Homing;

    //入力受付
    private Coroutine coroutine;
    private bool flag_Input = false;
    public float inputTime = 0.9f;//入力受付時間
    private int inputCount = 0;//入力は一回まで
    private float elapsedTime;

    //演出用
    private Animator animator;
    private AudioSource SE;//音
    private bool flag_attack;//攻撃時に移動するため
    private Vector3 StartPos;
    private Vector3 EndPos;
    private float time = 0.2f;
    private Vector3 deltaPos;
    private float moveTime;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        MC = Player.GetComponent<Magic_Controller>();
        pcVR = Player.GetComponent<Player_ControllerVR>();
        //vision = Player.GetComponentInChildren<Vision>();

        animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        SE = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //ホーミングが必要になったら解放
        /*if (flag_Homing)
        {
            if (vision.nearTarget != null)
            {
                Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, Quaternion.LookRotation(vision.nearTarget.transform.position - Player.transform.position), 0.5f);//Playerをターゲットのほうにゆっくり向ける
                Player.transform.rotation = Quaternion.Euler(0, Player.transform.eulerAngles.y, 0);//Playerのx,zの回転を直す。回転嫌い。全部Eulerにしてしまえばよい
            }
        }*/

        if (flag_Input)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > inputTime)
            {
                flag_Input = false;
                elapsedTime = 0;
                inputCount = 0;
            }
            if (inputCount < 3)//3連撃
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))//ここを変える
                {
                    Parent.GetComponent<Character_Parameters>().Reverse_Magic();//魔法を使えるようにする
                    StopCoroutine(coroutine);//硬直の解除を防ぐ
                    coroutine = StartCoroutine(Shot());
                    elapsedTime = 0;
                    flag_Input = false;
                }
            }

        }

        //前に進ませる
        //if (flag_attack)
        //{

        //    Parent.transform.position += deltaPos * Time.deltaTime;
        //    moveTime += Time.deltaTime;
        //    if (moveTime > time)
        //    {

        //        moveTime = 0;
        //        flag_attack = false;

        //    }

        //}
    }

    //剣を手放す
    void Break()
    {

    }

    void Fire()
    {
        coroutine = StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        Parent.GetComponent<Character_Parameters>().SetKeylock();
        GameObject[] bullet = new GameObject[GetExNum()];

        //プレイヤを敵のほうに向けてホーミングしてるように見せる
        //if (vision.nearTarget != null)
        //{
        //    flag_Homing = true;
        //}

        //インプットの回数に応じて違うアニメーションの入った弾を出す

        //前に進ませる(保留)
        //EndPos = Parent.transform.position + Parent.transform.TransformDirection(Vector3.forward) * 0;//
        //StartPos = Parent.transform.position;
        //deltaPos = (EndPos - StartPos) / time;
        //if (inputCount == 0)
        //{
        //    //アニメーション
        //}
        //if (inputCount == 1)
        //{

        //}
        //if (inputCount == 2)
        //{

        //}
        //animator.SetTrigger("Walk");
        //flag_attack = true;

        //yield return new WaitForSeconds(0.2f);//振り向くまで待つ

        for (int i = 0; i < 1; i++)
        {
            if (inputCount == 0)
            {
                bullet[i] = GameObject.Instantiate(bullet_Prefab[0]);//弾生成
                bullet[i].transform.position = transform.position + Parent.transform.TransformDirection(new Vector3(0.5f, -0.5f, 0));//
                Destroy(bullet[i], bullet[0].transform.FindChild("Armature").gameObject.transform.FindChild("Bone").gameObject.GetComponentInChildren<Attack_Parameter>().GetA_Time());

            }
            if (inputCount == 1)
            {
                bullet[i] = GameObject.Instantiate(bullet_Prefab[1]);//弾生成
                bullet[i].transform.position = transform.position + Parent.transform.TransformDirection(new Vector3(-0.5f, -0.5f, 0));//
                Destroy(bullet[i], bullet[0].transform.FindChild("Armature").gameObject.transform.FindChild("Bone").gameObject.GetComponentInChildren<Attack_Parameter>().GetA_Time());
            }
            if (inputCount == 2)
            {
                bullet[i] = GameObject.Instantiate(bullet_Prefab[2]);//弾生成
                bullet[i].transform.position = transform.position + Parent.transform.TransformDirection(new Vector3(0.0f, 1.0f, -0.5f));//
                Destroy(bullet[i], bullet[0].transform.FindChild("Armature").gameObject.transform.FindChild("Bone").gameObject.GetComponentInChildren<Attack_Parameter>().GetA_Time() + 0.5f);

            }

            if (bullet[i] != null)
            {

                bullet[i].transform.FindChild("Armature").gameObject.transform.FindChild("Bone").gameObject.GetComponentInChildren<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

                bullet[i].transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.back));//回転させて弾頭を進行方向に向ける

                //CPに応じて大きさ変化
                if (pcVR.GetMP() > pcVR.max_MP / 2)
                {//体力半分以上
                    //bullet[i].transform.localScale = Parent.transform.TransformDirection(new Vector3(bullet[i].transform.localScale.x, bullet[i].transform.localScale.y, bullet[i].transform.localScale.z * 3));
                    bullet[i].transform.localScale = new Vector3(bullet[i].transform.localScale.x, bullet[i].transform.localScale.y, bullet[i].transform.localScale.z * 1.5f);
                }
                else if (pcVR.GetMP() > pcVR.max_MP / 4)
                {//体力4分の1以上
                    //bullet[i].transform.localScale = Parent.transform.TransformDirection(new Vector3(bullet[i].transform.localScale.x, bullet[i].transform.localScale.y, bullet[i].transform.localScale.z * 2));
                    bullet[i].transform.localScale = new Vector3(bullet[i].transform.localScale.x, bullet[i].transform.localScale.y, bullet[i].transform.localScale.z * 1.2f);
                }
                
            }

        }
        
        //MPの処理
        pcVR.SetMP(pcVR.GetMP() - GetSMP());

        //ずっと発動してるのでおろしとく
        //if (flag_Homing)
        //{
        //    flag_Homing = false;
        //}

        //効果音と演出
        //効果音と演出
        //if (!SE.isPlaying)
        //{

        SE.PlayOneShot(SE.clip);//SE

        //}
        yield return new WaitForSeconds(0.3f);//振り下ろし

        //振り下ろしてから0.3秒後から入力受付
        inputCount++;
        flag_Input = true;

        //yield return new WaitForSeconds(bullet_Prefab.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直
        yield return new WaitForSeconds(0.5f);//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Parameters>().SetActive();
        inputCount = -1;

    }

}
