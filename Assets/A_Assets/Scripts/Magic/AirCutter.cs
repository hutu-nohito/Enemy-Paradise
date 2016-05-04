using UnityEngine;
using System.Collections;

public class AirCutter : Magic_Parameter {
    public GameObject bullet_Prefab;//弾のプレハブ

    private GameObject Player;
    private Magic_Controller MC;
    private Player_ControllerZ pcZ;

    private Animator animator;//アニメ
    private AudioSource SE;//音

    private Vector3 oldDir = Vector3.zero;//撃った方向
    private bool flag_Parent = false;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        MC = Player.GetComponent<Magic_Controller>();
        pcZ = Player.GetComponent<Player_ControllerZ>();

        animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        SE = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Fire()
    {
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        Parent.GetComponent<Character_Manager>().SetKeylock();
        GameObject[] bullet = new GameObject[GetExNum()];

        //弾を飛ばす処理
        for (int i = 0; i < 2; i++)
        {

            bullet[i] = GameObject.Instantiate(bullet_Prefab);//弾生成


            if (bullet[i] != null)
            {

                MC.AddExistBullet(bullet[i]);//現在の弾数を増やす
                bullet[i].GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

                bullet[i].transform.position = transform.position + Parent.transform.TransformDirection(new Vector3(-0.5f + i, 0, 0));//
                bullet[i].transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward).normalized);//回転させて弾頭を進行方向に向ける

                Destroy(bullet[i], bullet_Prefab.GetComponent<Attack_Parameter>().GetA_Time());

            }

        }

        //MPの処理
        pcZ.SetMP(pcZ.GetMP() - GetSMP());

        for (int i = 0; i < 2; i++)
        {

            if (bullet[i] != null)
            {

                //広げる
                //カメラとキャラの向きが90°以上ずれてたら
                if (Vector3.Dot(pcZ.direction.normalized, Parent.transform.TransformDirection(Vector3.forward).normalized) < 0)//二つのベクトル間の角度が90°以上(たぶん)
                {
                    bullet[i].GetComponent<Rigidbody>().velocity = (Parent.transform.TransformDirection(Vector3.forward).normalized + Parent.transform.TransformDirection(new Vector3(-1.5f + i * 3, 0, 0)).normalized) * bullet[i].GetComponent<Attack_Parameter>().speed;//キャラの向いてる方向
                    oldDir = Parent.transform.TransformDirection(Vector3.forward).normalized + Parent.transform.TransformDirection(new Vector3(-1.5f + i * 3, 0, 0)).normalized;
                    flag_Parent = true;
                }
                else
                {
                    bullet[i].GetComponent<Rigidbody>().velocity = ((Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 50)) - transform.position).normalized + Camera.main.transform.TransformDirection(new Vector3(-1.5f + i * 3, 0, 0))) * bullet[i].GetComponent<Attack_Parameter>().speed;//画面の真ん中
                    oldDir = (Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 50)) - transform.position).normalized + Camera.main.transform.TransformDirection(new Vector3(-1.5f + i * 3, 0, 0));
                    flag_Parent = false;
                }
                //注目中だったら
                if (pcZ.GetF_Watch())
                {
                    //ちょっと上を狙わないと地面に向かってく
                    bullet[i].GetComponent<Rigidbody>().velocity = ((Camera.main.GetComponent<Z_Camera>().Target.transform.position + new Vector3(0, Camera.main.GetComponent<Z_Camera>().Target.transform.localScale.y, 0) - transform.position).normalized + Parent.transform.TransformDirection(new Vector3(-1.5f + i * 3, 0, 0))) * bullet[i].GetComponent<Attack_Parameter>().speed;//敵の方向
                    oldDir = (Camera.main.GetComponent<Z_Camera>().Target.transform.position + new Vector3(0, Camera.main.GetComponent<Z_Camera>().Target.transform.localScale.y, 0) - transform.position).normalized + Parent.transform.TransformDirection(new Vector3(-1.5f + i * 3, 0, 0));
                    flag_Parent = false;
                }

                //bullet[i].GetComponent<Rigidbody>().velocity = (Parent.transform.TransformDirection(new Vector3(-0.5f + i, 0, 0.5f)) * bullet[i].GetComponent<Attack_Parameter>().speed);

            }

        }

        yield return new WaitForSeconds(1);//弾を曲げるまでの時間

        for (int i = 0; i < 2; i++)
        {

            if (bullet[i] != null)
            {
                //縮める
                if (flag_Parent)
                {
                    bullet[i].GetComponent<Rigidbody>().velocity = (oldDir.normalized + Parent.transform.TransformDirection(new Vector3(0.5f - i, 0, 1.5f)) * bullet[i].GetComponent<Attack_Parameter>().speed);

                }
                else
                {
                    bullet[i].GetComponent<Rigidbody>().velocity = (oldDir.normalized + Camera.main.transform.TransformDirection(new Vector3(0.5f - i, 0, 1.5f)) * bullet[i].GetComponent<Attack_Parameter>().speed);

                }


            }

        }

        //効果音と演出
        if (!SE.isPlaying)
        {

            SE.PlayOneShot(SE.clip);//SE

        }

        yield return new WaitForSeconds(bullet_Prefab.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Manager>().SetActive();

    }

}
