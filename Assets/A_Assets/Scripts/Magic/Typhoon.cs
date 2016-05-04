using UnityEngine;
using System.Collections;

public class Typhoon : Magic_Parameter {


    public GameObject bullet_Prefab;//弾のプレハブ

    private Magic_Controller MC;
    private Player_ControllerZ pcZ;
    private Z_Camera zcamera;//注目対象はここで取得

    // Use this for initialization
    void Start()
    {
        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcZ = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerZ>();
        zcamera = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Z_Camera>();
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

        //たぶんここに撃つアニメーション
        yield return new WaitForSeconds(bullet_Prefab.GetComponent<Attack_Parameter>().GetR_Time());//撃つ前の硬直

        bullet = GameObject.Instantiate(bullet_Prefab);//弾生成
        MC.AddExistBullet(bullet);//現在の弾数を増やす
        bullet.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

        //MPの処理
        pcZ.SetMP(pcZ.GetMP() - GetSMP());

        //弾を飛ばす処理
        if (pcZ.GetF_Watch())//注目してたら相手の後ろ
        {
            bullet.transform.position = zcamera.Target.transform.position - transform.TransformDirection(Vector3.forward);//回り
        }
        else
        {
            bullet.transform.position = transform.position;
        }

        bullet.transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward));//回転させて弾頭を進行方向に向ける
        //bullet.GetComponent<Rigidbody>().velocity = ((Parent.GetComponent<MousePoint>().worldPoint - Parent.transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed);

        //効果音と演出
        /*if(!audioSource.isPlaying){
			
            audioSource.Play();//SE
			
        }*/

        Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

        yield return new WaitForSeconds(0.5f);//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Manager>().SetActive();

    }

}
