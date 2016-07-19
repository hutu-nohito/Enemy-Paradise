using UnityEngine;
using System.Collections;

public class Bullet : Magic_Parameter {

    /******************************************************************************/
    /** @brief バレットの動きを制御
    * @date 2016/05/29
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
*/
    /******************************************************************************/
    /* 更新履歴
    *  全ての魔法はこれをもとに作られる
    */
    /******************************************************************************/


    public GameObject bullet_Prefab;//弾のプレハブ

    private Magic_Controller MC;
    private Player_ControllerVR pcZ;

    private Animator animator;//アニメ
    private AudioSource SE;//音
    
    // Use this for initialization
    void Start()
    {
        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcZ = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerVR>();
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
        Parent.GetComponent<Character_Parameters>().SetKeylock();
        GameObject bullet;

        animator.SetTrigger("Shoot");

        bullet = GameObject.Instantiate(bullet_Prefab);//弾生成
        //MC.AddExistBullet(bullet);//現在の弾数を増やす
        bullet.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

        //MPの処理
        pcZ.SetMP(pcZ.GetMP() - GetSMP());

        //弾を飛ばす処理
        bullet.transform.position = transform.position;//Muzzleの位置
        bullet.transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward).normalized);//回転させて弾頭を進行方向に向ける
        //カメラとキャラの向きが90°以上ずれてたら
        if (Vector3.Dot(pcZ.direction.normalized, Parent.transform.TransformDirection(Vector3.forward).normalized) < 0)//二つのベクトル間の角度が90°以上(たぶん)
        {
            bullet.GetComponent<Rigidbody>().velocity = Parent.transform.TransformDirection(Vector3.forward).normalized * bullet.GetComponent<Attack_Parameter>().speed;//キャラの向いてる方向
        }
        else
        {
            bullet.GetComponent<Rigidbody>().velocity = (Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 50)) - transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed;//画面の真ん中
        }
        //注目中だったら
        if (pcZ.GetF_Watch())
        {
            //ちょっと上を狙わないと地面に向かってく
            bullet.GetComponent<Rigidbody>().velocity = (Camera.main.GetComponent<Z_Camera>().Target.transform.position + new Vector3(0,Camera.main.GetComponent<Z_Camera>().Target.transform.localScale.y,0) - transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed;//敵の方向
        }

        //効果音と演出
        if(!SE.isPlaying){
			
            SE.PlayOneShot(SE.clip);//SE
			
        }

        Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

        yield return new WaitForSeconds(bullet.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Parameters>().SetActive();

    }

}
