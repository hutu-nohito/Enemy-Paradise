using UnityEngine;
using System.Collections;

public class Claw : Magic_Parameter {

    /*

        入力タイミング
        一回目の入力から0.3~0.9
    */

    public GameObject bullet_Prefab;//弾のプレハブ

    private GameObject Player;
    private Magic_Controller MC;
    private Player_ControllerZ pcZ;

    //近接ホーミング用
    private Vision vision;
    private bool flag_Homing;

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
        pcZ = Player.GetComponent<Player_ControllerZ>();
        animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        SE = GetComponent<AudioSource>();
        vision = Player.GetComponentInChildren<Vision>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flag_Homing)
        {
            if(vision.nearTarget != null)
            {
                Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, Quaternion.LookRotation(vision.nearTarget.transform.position - Player.transform.position), 0.5f);//Playerをターゲットのほうにゆっくり向ける
                Player.transform.rotation = Quaternion.Euler(0, Player.transform.eulerAngles.y, 0);//Playerのx,zの回転を直す。回転嫌い。全部Eulerにしてしまえばよい
            }
        }

        if (flag_Input)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > inputTime)
            {
                flag_Input = false;
                elapsedTime = 0;
                inputCount = 0;
            }
            if(inputCount < 2)//3連撃
            {
                if (Input.GetMouseButtonDown(0))//左クリック
                {
                    Parent.GetComponent<Character_Manager>().Reverse_Magic();//魔法を使えるようにする
                    StopCoroutine(coroutine);//硬直の解除を防ぐ
                    coroutine = StartCoroutine(Shot());
                    elapsedTime = 0;
                    inputCount++;
                    flag_Input = false;
                }
            }
            
        }

        //前に進ませる
        if (flag_attack)
        {

            Parent.transform.position += deltaPos * Time.deltaTime;
            moveTime += Time.deltaTime;
            if (moveTime > time)
            {

                moveTime = 0;
                flag_attack = false;

            }
            
        }
    }

    void Fire()
    {
        coroutine = StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        Parent.GetComponent<Character_Manager>().SetKeylock();
        GameObject[] bullet = new GameObject[GetExNum()];

        //プレイヤを敵のほうに向けてホーミングしてるように見せる
        if (vision.nearTarget != null)
        {
            flag_Homing = true;
        }

        //インプットの回数に応じて違うアニメーションの入った弾を出す

        //前に進ませる
        if(inputCount == 1)
        {
            EndPos = Parent.transform.position + Parent.transform.TransformDirection(Vector3.forward) * 3;//
            StartPos = Parent.transform.position;
            deltaPos = (EndPos - StartPos) / time;
            animator.SetTrigger("Walk");
            flag_attack = true;
        }
        
        yield return new WaitForSeconds(0.2f);//振り向くまで待つ

        for (int i = 0; i < 1; i++)
        {

            bullet[i] = GameObject.Instantiate(bullet_Prefab);//弾生成

            if (bullet[i] != null)
            {

                MC.AddExistBullet(bullet[i]);//現在の弾数を増やす
                bullet[i].transform.FindChild("Armature").gameObject.transform.FindChild("Bone").gameObject.GetComponentInChildren<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

                bullet[i].transform.position = transform.position + Parent.transform.TransformDirection(new Vector3(0.5f, -1, 0));//
                bullet[i].transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward));//回転させて弾頭を進行方向に向ける

                Destroy(bullet[i], bullet[0].transform.FindChild("Armature").gameObject.transform.FindChild("Bone").gameObject.GetComponentInChildren<Attack_Parameter>().GetA_Time());

            }
            
        }

        //効果音と演出
        if (!SE.isPlaying)
        {

            SE.PlayOneShot(SE.clip);//SE

        }

        //MPの処理
        pcZ.SetMP(pcZ.GetMP() - GetSMP());

        //ずっと発動してるのでおろしとく
        if (flag_Homing)
        {
            flag_Homing = false;
        }

        yield return new WaitForSeconds(0.3f);//振り下ろし

        //振り下ろしてから0.3秒後から入力受付
        flag_Input = true;

        yield return new WaitForSeconds(0.5f);//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Manager>().SetActive();
        inputCount = 0;

    }

}
