using UnityEngine;
using System.Collections;

public class Rayfall : Magic_Parameter {

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
        //MPの処理
        pcZ.SetMP(pcZ.GetMP() - GetSMP());

        GameObject[] bullet = new GameObject[GetExNum()];

        for (int i = 0; i < GetExNum(); i++)
        {

            bullet[i] = GameObject.Instantiate(bullet_Prefab);//弾生成
            bullet[i].transform.position = transform.position + Parent.transform.TransformDirection(new Vector3(-1.5f + i, 0, 0));//
            if (bullet[i] != null)
            {

                MC.AddExistBullet(bullet[i]);//現在の弾数を増やす
                bullet[i].GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある


            }
            

        }

        //弾を飛ばす処理
        /*bullet[0].transform.position = new Vector3(-3, 0, -3) - transform.position + Parent.GetComponent<Character_Parameters>().direction;// - transform.localPosition;//前方に飛ばす
        bullet[1].transform.position = new Vector3(-3, 0, 3) - transform.position + Parent.GetComponent<Character_Parameters>().direction;// - transform.localPosition;//前方に飛ばす
        bullet[2].transform.position = new Vector3(3, 0, -3) - transform.position + Parent.GetComponent<Character_Parameters>().direction;// - transform.localPosition;//前方に飛ばす
        bullet[3].transform.position = new Vector3(3, 0, 3) - transform.position + Parent.GetComponent<Character_Parameters>().direction;// - transform.localPosition;//前方に飛ばす*/

        for (int i = 0; i < GetExNum(); i++) {

            if (bullet[i] != null)
            {
                //bullet[i].transform.position = transform.position + Parent.transform.TransformDirection(new Vector3(-1.5f + i, 0, 0));//
                bullet[i].transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.up) + Parent.transform.TransformDirection(Vector3.forward) + Parent.transform.TransformDirection(new Vector3(-1.5f + i, 0, 0)));//回転させて弾頭を進行方向に向ける
                bullet[i].GetComponent<Rigidbody>().velocity = ((Parent.transform.TransformDirection(Vector3.up) + Parent.transform.TransformDirection(Vector3.forward) + Parent.transform.TransformDirection(new Vector3(-1.5f + i, 0, 0))) * bullet[0].GetComponent<Attack_Parameter>().speed / 3);

                

                Destroy(bullet[i], bullet[0].GetComponent<Attack_Parameter>().GetA_Time());
            }

            yield return new WaitForSeconds(0.1f);//弾を生成する間隔
        }

        yield return new WaitForSeconds(bullet_Prefab.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Manager>().SetActive();
        
        //yield return new WaitForSeconds(0.1f);//上で止まる

        for (int i = 0; i < GetExNum(); i++)
        {

            if (bullet[i] != null)
            {
                bullet[i].transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward));//回転させて弾頭を進行方向に向ける
                bullet[i].GetComponent<Rigidbody>().velocity = Vector3.zero;

            }
                

        }

        yield return new WaitForSeconds(1);//発射

        //効果音と演出
        /*if(!audioSource.isPlaying){
			
            audioSource.Play();//SE
			
        }*/

        for (int i = 0; i < GetExNum(); i++)
        {

            if (bullet[i] != null)
            {

                bullet[i].transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward) - new Vector3(0, 0.5f, 0));//回転させて弾頭を進行方向に向ける
                bullet[i].GetComponent<Rigidbody>().velocity = ((bullet[i].transform.TransformDirection(Vector3.forward) - new Vector3(0, 0.5f, 0)) * bullet[0].GetComponent<Attack_Parameter>().speed);

            }

        }

    }
}
