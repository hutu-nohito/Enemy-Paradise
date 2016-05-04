using UnityEngine;
using System.Collections;

public class Quake : Magic_Parameter {

    public GameObject bullet_Prefab;//弾のプレハブ

    private Magic_Controller MC;
    private Player_ControllerZ pcZ;

    // Use this for initialization
    void Start()
    {
        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcZ = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerZ>();
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

        bullet = GameObject.Instantiate(bullet_Prefab);//弾生成
        MC.AddExistBullet(bullet);//現在の弾数を増やす
        bullet.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

        //MPの処理
        pcZ.SetMP(pcZ.GetMP() - GetSMP());

        //足元を見る
        RaycastHit hit;
        GameObject hitObject;

        Vector3 LineDirection = Vector3.down;//下でおｋ

        if (Physics.Raycast(transform.position, LineDirection, out hit, 50))
        {
            hitObject = hit.collider.gameObject;//レイヤーがIgnoreLayerのObjectは弾かれる。

            //Debug.DrawLine(LineStart, hit.point, Color.blue);
            //Debug.Log(hitObject);

            //地面だったら
            if (hitObject.gameObject.name == "Terrain")
            {

                bullet.transform.position = hit.point;

            }
        }


        //弾を飛ばす処理
        bullet.transform.position = transform.position + Parent.GetComponent<Character_Parameters>().direction;//前方に飛ばす
        bullet.transform.rotation = Quaternion.LookRotation(-(Parent.GetComponent<MousePoint>().worldPoint - Parent.transform.position).normalized);//回転させて弾頭を進行方向に向ける
        bullet.GetComponent<Rigidbody>().velocity = ((Parent.GetComponent<MousePoint>().worldPoint - Parent.transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed);

        //効果音と演出
        /*if(!audioSource.isPlaying){
			
            audioSource.Play();//SE
			
        }*/

        Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

        yield return new WaitForSeconds(bullet.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Manager>().SetActive();

    }

}
