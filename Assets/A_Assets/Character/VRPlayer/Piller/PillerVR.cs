using UnityEngine;
using System.Collections;

public class PillerVR : Magic_Parameter {

    public GameObject bullet_Prefab;//弾のプレハブ

    private Magic_Controller MC;
    private Player_ControllerVR pcVR;
    private Animator animator;//アニメ
    private AudioSource SE;//音

    private Z_Camera zcamera;//足元用 注目対象はここで取得

    // Use this for initialization
    void Start()
    {
        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcVR = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerVR>();
        animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        SE = GetComponent<AudioSource>();

        //zcamera = Camera.main.gameObject.GetComponentInChildren<Z_Camera>();

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

        yield return new WaitForSeconds(bullet_Prefab.GetComponent<Attack_Parameter>().GetR_Time());//撃つ前の硬直

        bullet = GameObject.Instantiate(bullet_Prefab);//弾生成
        bullet.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

        //MPの処理
        pcVR.SetMP(pcVR.GetMP() - GetSMP());

        //弾を飛ばす処理
        //if (pcVR.GetF_Watch())//注目してたら相手の足元
        //{
        //    //bullet.transform.position = zcamera.Target.transform.position - new Vector3(0, zcamera.Target.transform.localScale.y / 2,0);//前方に飛ばす

        //    //足元を見る
        //    RaycastHit hit;
        //    GameObject hitObject;

        //    Vector3 LineStart = new Vector3(zcamera.Target.transform.position.x, zcamera.Target.transform.position.y /*- zcamera.Target.transform.localScale.y / 2*/, zcamera.Target.transform.position.z);
        //    Vector3 LineDirection = Vector3.down;//下でおｋ

        //    if (Physics.Raycast(LineStart, LineDirection, out hit, 50))
        //    {
        //        hitObject = hit.collider.gameObject;//レイヤーがIgnoreLayerのObjectは弾かれる。

        //        //Debug.DrawLine(LineStart, hit.point, Color.blue);
        //        //Debug.Log(hitObject);

        //        //地面だったら
        //        if (hitObject.gameObject.name == "Terrain")
        //        {

        //            bullet.transform.position = hit.point;
        //            Debug.Log("w");

        //        }
        //    }
        //    else
        //    {
        //        bullet.transform.position = new Vector3(zcamera.Target.transform.position.x, zcamera.Target.transform.position.y - zcamera.Target.transform.localScale.y / 2, zcamera.Target.transform.position.z);//下に地面がなかったら
        //    }
        //}
        //else
        //{
        //    bullet.transform.position = transform.position;
        //}

        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward));//回転させて弾頭を進行方向に向ける

        //効果音と演出
        if (!SE.isPlaying)
        {

            SE.PlayOneShot(SE.clip);//SE

        }

        Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

        yield return new WaitForSeconds(0.5f);//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Parameters>().SetActive();

    }

}
