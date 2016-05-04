using UnityEngine;
using System.Collections;

public class Wall2 : Magic_Parameter
{
    /*
        耐久力は保留
    */

    public GameObject bullet_Prefab;//弾のプレハブ

    private Magic_Controller MC;
    private Player_ControllerZ pcZ;

    private Z_Camera zcamera;//足元用 注目対象はここで取得

    private Animator animator;//アニメ
    private AudioSource SE;//音
    

    //演出
    private Camera_ControllerZ CCZ;
    private AudioSource breakSE;//壊れる音
    private GameObject breakEff;//壊れるエフェクト

    // Use this for initialization
    void Start()
    {
        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcZ = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerZ>();

        animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        SE = GetComponent<AudioSource>();

        CCZ = Camera.main.gameObject.GetComponent<Camera_ControllerZ>();

        zcamera = Camera.main.gameObject.GetComponentInChildren<Z_Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Hold()
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
        animator.SetTrigger("Shoot");
        SE.enabled = true;//SE戻す

        yield return new WaitForSeconds(0.5f);//出すまで

        //効果音と演出
        if (!SE.isPlaying)
        {
            //ゴゴゴゴは始まるまでにラグがある
            SE.PlayOneShot(SE.clip);//SE

        }

        CCZ.flag_quake = true;//カメラ揺らす

        bullet = GameObject.Instantiate(bullet_Prefab);//弾生成
        MC.AddExistBullet(bullet);//現在の弾数を増やす
        bullet.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

        //MPの処理
        pcZ.SetMP(pcZ.GetMP() - GetSMP());

        //弾を飛ばす処理
        //足元を見る
        RaycastHit hit;
        GameObject hitObject;

        Vector3 LineStart = new Vector3(zcamera.Target.transform.position.x, zcamera.Target.transform.position.y /*- zcamera.Target.transform.localScale.y / 2*/, zcamera.Target.transform.position.z);
        Vector3 LineDirection = Vector3.down;//下でおｋ

        //弾を飛ばす処理
        if (pcZ.GetF_Watch())//注目してたら相手の足元
        {
            if (Physics.Raycast(LineStart, LineDirection, out hit, 200))
            {
                hitObject = hit.collider.gameObject;//レイヤーがIgnoreLayerのObjectは弾かれる。

                //Debug.DrawLine(LineStart, hit.point, Color.blue);
                //Debug.Log(hitObject);

                //地面だったら
                if (hitObject.gameObject.name == "Terrain")
                {
                    //bullet.transform.position = transform.position - new Vector3(0, transform.position.y - hit.point.y, 0) - new Vector3(0, 4, 0);
                    bullet.transform.position = hit.point - new Vector3(0, 4, 0);

                }
                else
                {
                    //（仮）
                    bullet.transform.position = transform.position - new Vector3(0, transform.position.y - hit.point.y - hitObject.transform.localScale.y, 0) - new Vector3(0, 4, 0);
                }
            }
        }
        else
        {
            bullet.transform.position = transform.position - new Vector3(0, 4, 0);
        }
        
        bullet.transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward).normalized);//回転させて弾頭を進行方向に向ける

        //必要なものを拾っとく
        GameObject effect;
        effect = bullet.transform.FindChild("Spurt").gameObject;
        breakEff = bullet.transform.FindChild("Break").gameObject;
        breakEff.SetActive(false);//いったん消しとく
        breakSE = bullet.GetComponent<AudioSource>();

        bullet.GetComponent<Rigidbody>().velocity = (Vector3.up * bullet.GetComponent<Attack_Parameter>().speed);

        yield return new WaitForSeconds(0.5f);//止める

        bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
        bullet.GetComponent<Rigidbody>().isKinematic = true;//固定
        Destroy(effect, 1.3f);
        bullet.GetComponent<Collider>().enabled = false;
        bullet.transform.FindChild("Wall_Model").gameObject.GetComponent<Collider>().enabled = true;
        Invoke("SEStop", 1.3f);
        //bullet.GetComponent<MeshCollider>().isTrigger = false;

        Invoke("Break", bullet.GetComponent<Attack_Parameter>().GetA_Time() - 0.6f);//消えるちょい前に演出
        Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

        yield return new WaitForSeconds(bullet.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Manager>().SetActive();

    }

    void SEStop()
    {
        SE.enabled = false;//SE止める
        CCZ.flag_quake = false;//揺れを止める
    }

    //壊れる演出
    void Break()
    {
        breakEff.SetActive(true);//演出ON
        //breakSE.enabled = true;//SEON
        breakSE.PlayOneShot(breakSE.clip);//SE

    }
}
