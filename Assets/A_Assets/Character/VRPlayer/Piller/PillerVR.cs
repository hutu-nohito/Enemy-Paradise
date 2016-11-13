using UnityEngine;
using System.Collections;

public class PillerVR : Magic_Parameter {

    public GameObject bullet_Prefab;//弾のプレハブ

    private Magic_Controller MC;
    private Player_ControllerVR pcVR;
    public GameObject HandR;
    //private Animator animator;//アニメ
    private AudioSource SE;//音

    //ガイド用
    public GameObject TargetArea;
    private bool flag_guide = false;

    private Z_Camera zcamera;//足元用 注目対象はここで取得

    // Use this for initialization
    void Start()
    {
        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcVR = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerVR>();
        //animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        SE = GetComponent<AudioSource>();

        //zcamera = Camera.main.gameObject.GetComponentInChildren<Z_Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        //RaycastHit hit;
        //GameObject hitObject;

        //Vector3 LineStart = HandR.transform.position;
        //Vector3 LineDirection = HandR.transform.TransformDirection(Vector3.forward);//

        //if (Physics.Raycast(LineStart, LineDirection, out hit, 5000))
        //{
        //    hitObject = hit.collider.gameObject;//レイヤーがIgnoreLayerのObjectは弾かれる。

        //    Debug.DrawLine(LineStart, hit.point, Color.blue);
        //    //Debug.Log(hitObject);

        //}

        if (flag_guide)
        {
            TargetArea.SetActive(true);
            TargetArea.transform.position = new Vector3(transform.position.x + (HandR.transform.TransformDirection(Vector3.forward) * 15).x,
                        transform.position.y + 4.5f,
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
        GameObject[] bullet = new GameObject[3];
        Vector3 direction = Parent.transform.TransformDirection(Vector3.forward);//この時点でのプレイヤの向きを基準にする
        Vector3 HandDirection = HandR.transform.TransformDirection(Vector3.forward) * 5;
        Vector3 OldTransform = transform.position;
        //animator.SetTrigger("Shoot");

        //yield return new WaitForSeconds(1.0f);

        //MPの処理
        pcVR.SetMP(pcVR.GetMP() - GetSMP());
        
        
        //
        for(int i = 0; i < 1; i++)
        {
            bullet[i] = GameObject.Instantiate(bullet_Prefab);//弾生成
            bullet[i].transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward));//回転させて弾頭を進行方向に向ける
            bullet[i].GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある
            bullet[i].transform.position = new Vector3(
                    //OldTransform.x + HandDirection.x * ((i + 2) * 2) - 2,
                    TargetArea.transform.position.x,
                    transform.position.y,
                    //OldTransform.z + HandDirection.z * ((i + 2) * 2) - 2
                    TargetArea.transform.position.z
                    );//
            
            Destroy(bullet[i], bullet_Prefab.GetComponent<Attack_Parameter>().GetA_Time());

            //効果音と演出
            if (!SE.isPlaying)
            {

                SE.PlayOneShot(SE.clip);//SE

            }

            yield return new WaitForSeconds(1.5f);
        }

    }

}
