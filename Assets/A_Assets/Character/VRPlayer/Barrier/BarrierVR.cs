using UnityEngine;
using System.Collections;

public class BarrierVR : Magic_Parameter
{
    /******************************************************************************/
    /** @brief バリアの動きを制御(VR)
    * @date 2016/08/14
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
*/
    /******************************************************************************/
    /* 更新履歴
    *  バリアを張ってる間はダメージを受けないようにする
    *  このままだと出すたびにフラグ反転するから何とかする
    *  出始めとで終わりが不安定かも
    */
    /******************************************************************************/


    public GameObject bullet_Prefab;//弾のプレハブ

    private Magic_Controller MC;
    private Player_ControllerVR pcVR;
    public GameObject Player;

    //private Animator animator;//アニメ
    private AudioSource SE;//音

    // Use this for initialization
    void Start()
    {
        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcVR = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerVR>();
        //animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        SE = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //魔法を保持してる間
    void Guide()
    {

    }

    void Fire()
    {
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        GameObject bullet;

        //animator.SetTrigger("Shoot");

        Parent.GetComponent<Character_Parameters>().Reverse_Damage();//ダメージを受けないようにする

        bullet = GameObject.Instantiate(bullet_Prefab);//弾生成
        //MC.AddExistBullet(bullet);//現在の弾数を増やす
        bullet.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

        //MPの処理
        pcVR.SetMP(pcVR.GetMP() - GetSMP());

        //弾を飛ばす処理
        bullet.transform.position = transform.position;//Muzzleの位置
        bullet.transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward).normalized);//回転させて弾頭を進行方向に向ける
        
        //効果音と演出
        if (!SE.isPlaying)
        {

            SE.PlayOneShot(SE.clip);//SE

        }

        Destroy(bullet, 2);

        yield return new WaitForSeconds(2.0f);//撃った後の硬直

        Parent.GetComponent<Character_Parameters>().Reverse_Damage();//ダメージを受けるようにする
       
    }
}
