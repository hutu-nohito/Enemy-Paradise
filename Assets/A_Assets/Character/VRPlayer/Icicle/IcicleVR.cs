using UnityEngine;
using System.Collections;

public class IcicleVR : Magic_Parameter {

    public GameObject bullet_Prefab;//弾のプレハブ
    public GameObject HandR;

    //ガイド用
    public GameObject TargetArea;
    private bool flag_guide = false;

    private Magic_Controller MC;
    private Player_ControllerVR pcVR;
    private AudioSource SE;//音

    // Use this for initialization
    void Start()
    {
        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcVR = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerVR>();
        SE = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flag_guide)
        {
            TargetArea.SetActive(true);
            TargetArea.transform.position = new Vector3(transform.position.x + (HandR.transform.TransformDirection(Vector3.forward) * 15).x,
                        transform.position.y - 4.5f,
                        transform.position.z + (HandR.transform.TransformDirection(Vector3.forward) * 15).z);
        }

    }

    //魔法を保持してる間
    void Guide()
    {
        flag_guide = true;
    }

    void Fire()
    {
        flag_guide = false;
        TargetArea.SetActive(false);
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        //MPの処理
        pcVR.SetMP(pcVR.GetMP() - GetSMP());

        GameObject[] bullet = new GameObject[GetExNum()];

        //効果音と演出
        if (!SE.isPlaying)
        {

            SE.PlayOneShot(SE.clip);//SE

        }
        for (int i = 0; i < GetExNum(); i++)
        {

            bullet[i] = GameObject.Instantiate(bullet_Prefab);//弾生成

            if (bullet[i] != null)
            {

                bullet[i].GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

                //bullet[i].transform.position = transform.position + Parent.transform.TransformDirection(new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));//ランダム
                bullet[i].transform.position = new Vector3(
                    transform.position.x + (HandR.transform.TransformDirection(Vector3.forward) * 15).x + Parent.transform.TransformDirection(new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3))).x,
                    transform.position.y,
                    transform.position.z + (HandR.transform.TransformDirection(Vector3.forward) * 15).z + Parent.transform.TransformDirection(new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3))).z
                    );//
                bullet[i].transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.down));//回転させて弾頭を進行方向に向ける            

                Destroy(bullet[i], bullet_Prefab.GetComponent<Attack_Parameter>().GetA_Time());

            }

            //yield return new WaitForSeconds(0.1f);//弾の生成間隔
        }

        for (int i = 0; i < GetExNum(); i++)
        {
            if (bullet[i] != null)
            {
                bullet[i].GetComponent<Rigidbody>().velocity = ((Parent.transform.TransformDirection(Vector3.down)) * bullet_Prefab.GetComponent<Attack_Parameter>().speed);
                yield return new WaitForSeconds(0.08f);//落とすまでの間
            }
        }

    }
}
