using UnityEngine;
using System.Collections;

public class Meteor : Magic_Parameter {
    public GameObject bullet_Prefab;//弾のプレハブ

    private Magic_Controller MC;
    private Player_ControllerZ pcZ;

    private int num_meteor;//出せるメテオの数

    //演出用・アニメータ、パーティクル・・・
    private Animator animator;
    public GameObject Jin;//魔方陣
    private bool isFade = false;//魔方陣フェード用
    private float time = 0;
    private AudioSource SE;//音

    //演出
    private Camera_ControllerZ CCZ;

    // Use this for initialization
    void Start()
    {
        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcZ = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerZ>();
        animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        SE = GetComponent<AudioSource>();

        CCZ = Camera.main.gameObject.GetComponent<Camera_ControllerZ>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFade)
        {
            if(time < 1)time += Time.deltaTime * 3;
            Jin.GetComponent<Renderer>().material.color = new Color(1, 1, 1, time);
            
        }
        else
        {
            if (time > 0) time -= Time.deltaTime * 3;
            Jin.GetComponent<Renderer>().material.color = new Color(1, 1, 1, time);
        }
        
    }

    void Fire()
    {
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        Parent.GetComponent<Character_Manager>().SetKeylock();

        animator.SetTrigger("Shoot");//アニメーション
        //魔方陣つける
        isFade = true;

        //効果音と演出
        if (!SE.isPlaying)
        {

            SE.PlayOneShot(SE.clip);//SE

        }

        yield return new WaitForSeconds(1);//撃つまでのため

        //MPの処理
        pcZ.SetMP(pcZ.GetMP() - GetSMP());

        CCZ.flag_quake = true;//カメラ揺らす

        GameObject[] bullet = new GameObject[GetExNum()];

        //体力に応じて出せるメテオ数を変える
        if (pcZ.GetHP() > pcZ.max_HP / 2)//体力が半分以上
            num_meteor = 10;
        else if (pcZ.GetHP() > pcZ.max_HP / 4)//4分の1以上
            num_meteor = 7;
        else//瀕死
            num_meteor = 3;

        for (int i = 0; i < num_meteor; i++)
        {

            bullet[i] = GameObject.Instantiate(bullet_Prefab);//弾生成

            if (bullet[i] != null)
            {

                MC.AddExistBullet(bullet[i]);//現在の弾数を増やす
                bullet[i].GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

                bullet[i].transform.position = transform.position + Parent.transform.TransformDirection(new Vector3(Random.Range(-5, 6), Random.Range(0, 5), Random.Range(-5, 6)));//ランダム
                bullet[i].transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward));//回転させて弾頭を進行方向に向ける
                bullet[i].GetComponent<Rigidbody>().velocity = ((Parent.transform.TransformDirection(new Vector3(0, -4, 2))) * bullet_Prefab.GetComponent<Attack_Parameter>().speed);

                Destroy(bullet[i], bullet[0].GetComponent<Attack_Parameter>().GetA_Time());

            }


        }

        yield return new WaitForSeconds(0.3f);//魔方陣消す

        //魔方陣を消す
        isFade = false;

        yield return new WaitForSeconds(bullet_Prefab.GetComponent<Attack_Parameter>().GetR_Time() - 0.3f);//撃った後の硬直

        CCZ.flag_quake = false;//揺れを止める

        //硬直を解除
        Parent.GetComponent<Character_Manager>().SetActive();

    }
}
