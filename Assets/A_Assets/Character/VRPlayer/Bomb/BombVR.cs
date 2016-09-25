using UnityEngine;
using System.Collections;

public class BombVR : Magic_Parameter
{

    public GameObject bullet_Prefab;//弾のプレハブ
    public GameObject Hand;//右手
    private GameObject bullet;//
    private Vector3 OldPos = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    private float elapsedTime = 0.0f;

    private Magic_Controller MC;
    private Player_ControllerVR pcVR;
    private Animator animator;//アニメ
    private AudioSource SE;//音

    private Z_Camera zcamera;//注目対象はここで取得

    // Use this for initialization
    void Start()
    {

        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcVR = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerVR>();
        animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        SE = GetComponent<AudioSource>();
        zcamera = Camera.main.gameObject.GetComponentInChildren<Z_Camera>();

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
        bullet = GameObject.Instantiate(bullet_Prefab);//弾生成
        bullet.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある
        bullet.GetComponent<Collider>().enabled = false;
        bullet.transform.position = Hand.transform.position;//手の位置
        OldPos = bullet.transform.position;

    }

    //ボムをキープ
    void Hold()
    {
        elapsedTime += Time.deltaTime;
        bullet.GetComponent<Collider>().enabled = false;
        bullet.transform.position = Hand.transform.position + Hand.transform.TransformDirection(Vector3.forward);//手の位置
        direction = bullet.transform.position - OldPos;

        if(elapsedTime >= 0.5f)
        {
            OldPos = bullet.transform.position;
            elapsedTime = 0.0f;
        }
        
        Debug.Log(direction);

    }

    //ボムを投げる
    void Throw()
    {
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        Parent.GetComponent<Character_Parameters>().SetKeylock();
        //GameObject bullet;

        animator.SetTrigger("Shoot");
        bullet.GetComponent<Collider>().enabled = true;
        bullet.GetComponent<Rigidbody>().velocity = (Parent.transform.TransformDirection(Vector3.forward) + direction * 100000).normalized * direction.magnitude * bullet.GetComponent<Attack_Parameter>().speed;//キャラの向いてる方向
        //bullet.GetComponent<Rigidbody>().velocity = direction.normalized * bullet.GetComponent<Attack_Parameter>().speed * 100;//キャラの向いてる方向

        //bullet = GameObject.Instantiate(bullet_Prefab);//弾生成
        //bullet.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

        //MPの処理
        //pcVR.SetMP(pcVR.GetMP() - GetSMP());

        //弾を飛ばす処理
        //bullet.transform.position = transform.position;//Muzzleの位置
        //bullet.transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward).normalized);//回転させて弾頭を進行方向に向ける
        //bullet.GetComponent<Rigidbody>().velocity = Parent.transform.TransformDirection(Vector3.forward).normalized * bullet.GetComponent<Attack_Parameter>().speed;//キャラの向いてる方向
        //カメラとキャラの向きが90°以上ずれてたら
        //if (Vector3.Dot(pcVR.direction.normalized, Parent.transform.TransformDirection(Vector3.forward).normalized) < 0)//二つのベクトル間の角度が90°以上(たぶん)
        //{
        //    bullet.GetComponent<Rigidbody>().velocity = Parent.transform.TransformDirection(Vector3.forward).normalized * bullet.GetComponent<Attack_Parameter>().speed;//キャラの向いてる方向
        //}
        //else
        //{
        //    if (pcVR.GetF_Watch())
        //    {
        //        if ((zcamera.Target.transform.position.y - Parent.transform.position.y) >
        //           ((zcamera.Target.transform.position.x - Parent.transform.position.x) +
        //           (zcamera.Target.transform.position.z - Parent.transform.position.z)) / 2)
        //        {

        //            //奥狙う
        //            bullet.GetComponent<Rigidbody>().velocity = ((Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 50)) - transform.position).normalized + Parent.transform.TransformDirection(new Vector3(0, 2, 1)).normalized) * bullet.GetComponent<Attack_Parameter>().speed;//画面の真ん中

        //        }
        //        else
        //        {

        //            //下狙う
        //            bullet.GetComponent<Rigidbody>().velocity = ((Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 50)) - transform.position).normalized + Parent.transform.TransformDirection(new Vector3(0, 1, 2)).normalized) * bullet.GetComponent<Attack_Parameter>().speed;//画面の真ん中

        //        }

        //    }
        //    else
        //    {

        //        bullet.GetComponent<Rigidbody>().velocity = (Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 50)) - transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed;//画面の真ん中

        //    }

        //}
        //注目中だったら イラン
        /*if (pcVR.GetF_Watch())
        {
            //ちょっと上を狙わないと地面に向かってく
            bullet.GetComponent<Rigidbody>().velocity = (Camera.main.GetComponent<Z_Camera>().Target.transform.position + new Vector3(0, Camera.main.GetComponent<Z_Camera>().Target.transform.localScale.y, 0) - transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed;//敵の方向
        }*/

        //弾を飛ばす処理
        /*bullet.transform.position = transform.position + Parent.GetComponent<Character_Parameters>().direction;//前方に飛ばす
        bullet.transform.rotation = Quaternion.LookRotation(-(Parent.GetComponent<MousePoint>().worldPoint - Parent.transform.position).normalized);//回転させて弾頭を進行方向に向ける
        bullet.GetComponent<Rigidbody>().velocity = ((Parent.GetComponent<MousePoint>().worldPoint - Parent.transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed);
        */

        //効果音と演出
        if (!SE.isPlaying)
        {

            SE.PlayOneShot(SE.clip);//SE

        }

        Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

        yield return new WaitForSeconds(bullet.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Parameters>().SetActive();

    }

}
