using UnityEngine;
using System.Collections;

public class Homing : Magic_Parameter {

    public GameObject Bullet;//弾のObject
    private Z_Camera z_Camera;//注目用
    private Magic_Controller MC;
    private Player_ControllerZ pcZ;

    private Animator animator;//アニメ
    private AudioSource SE;//音


    // Use this for initialization
    void Start()
    {
        z_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Z_Camera>();
        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcZ = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerZ>();

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
        GameObject bullet;

        bullet = GameObject.Instantiate(Bullet);
        MC.AddExistBullet(bullet);//現在の弾数を増やす
        bullet.GetComponent<Homing_Bullet>().Parent = this.Parent;//もらった親を渡しておく必要がある

        Parent.GetComponent<Player_ControllerZ>().SetMP(Parent.GetComponent<Player_ControllerZ>().GetMP() - GetSMP());//MPを減らす

        animator.SetTrigger("Shoot");

        Vector3 Target;

        //弾を飛ばす処理
        bullet.transform.position = transform.position;

        //注目中なら注目対象、注目してなかったらマウスの位置
        if (pcZ.GetF_Watch())
        {
            Target = z_Camera.Target.transform.position;
            bullet.GetComponent<Homing_Bullet>().TargetSet(z_Camera.Target);//撃った時に注目していたらホーミングする
            bullet.transform.rotation = Quaternion.LookRotation(-(Target - Parent.transform.position).normalized);//回転させて矢じりを進行方向に向ける
            bullet.GetComponent<Rigidbody>().velocity = ((Target - Parent.transform.position).normalized * bullet.GetComponent<Homing_Bullet>().speed);
        }
        else
        {
            //カメラとキャラの向きが90°以上ずれてたら
            if (Vector3.Dot(pcZ.direction.normalized, Parent.transform.TransformDirection(Vector3.forward).normalized) < 0)//二つのベクトル間の角度が90°以上(たぶん)
            {
                bullet.GetComponent<Rigidbody>().velocity = Parent.transform.TransformDirection(Vector3.forward).normalized * bullet.GetComponent<Attack_Parameter>().speed;//キャラの向いてる方向
            }
            else
            {
                bullet.GetComponent<Rigidbody>().velocity = (Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 50)) - transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed;//画面の真ん中
            }

            bullet.transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward).normalized);//回転させて弾頭を進行方向に向ける

        }

        //効果音と演出
        if (!SE.isPlaying)
        {

            SE.PlayOneShot(SE.clip);//SE

        }

        Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

        yield return new WaitForSeconds(bullet.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Manager>().SetActive();

    }    
}
