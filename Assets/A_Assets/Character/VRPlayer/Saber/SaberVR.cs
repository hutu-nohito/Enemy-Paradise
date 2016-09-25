using UnityEngine;
using System.Collections;

public class SaberVR : Magic_Parameter {

    public GameObject bullet_Prefab;//弾のプレハブ
    public GameObject HandR;
    private GameObject bullet;

    private GameObject Player;
    private Magic_Controller MC;
    private Player_ControllerVR pcVR;
    private Coroutine coroutine;
    
    //演出用
    private Animator animator;
    private AudioSource SE;//音
    public AudioClip[] se;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        MC = Player.GetComponent<Magic_Controller>();
        pcVR = Player.GetComponent<Player_ControllerVR>();
        //vision = Player.GetComponentInChildren<Vision>();

        animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
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

    //剣を手放す
    void Break()
    {
        SE.PlayOneShot(se[1]);//SE
        Destroy(bullet.gameObject);
    }

    //握ってる間
    void Hold()
    {

    }

    void Fire()
    {
        coroutine = StartCoroutine(Shot());
    }

    //剣を持たせる
    IEnumerator Shot()
    {
        yield return new WaitForSeconds(0);

        
        bullet = GameObject.Instantiate(bullet_Prefab);//弾生成
        bullet.transform.position = HandR.transform.position;
        bullet.transform.parent = HandR.transform;//右手の子にする
        bullet.transform.FindChild("Armature").gameObject.transform.FindChild("Bone").gameObject.GetComponentInChildren<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

        bullet.transform.rotation = Quaternion.LookRotation(HandR.transform.TransformDirection(Vector3.forward));//回転させて弾頭を進行方向に向ける
        
        //残り体力に応じて大きさ変化(少ないほうが大きい)
        if (pcVR.GetHP() < pcVR.max_HP / 2)
        {//体力半分以上
            bullet.transform.localScale = new Vector3(bullet.transform.localScale.x * 3, bullet.transform.localScale.y * 3, bullet.transform.localScale.z * 3);
            bullet.transform.FindChild("Armature").gameObject.transform.localPosition = new Vector3(0,0.05f,0.45f);
            bullet.transform.FindChild("Armature").gameObject.transform.FindChild("Bone").gameObject.GetComponentInChildren<Attack_Parameter>().power *= 2;
        }
        if (pcVR.GetHP() < pcVR.max_HP / 4)
        {//体力4分の1以上
            bullet.transform.localScale = new Vector3(bullet.transform.localScale.x * 2, bullet.transform.localScale.y * 2, bullet.transform.localScale.z * 2);
            bullet.transform.FindChild("Armature").gameObject.transform.localPosition = new Vector3(0, 0.01f, 0.42f);
            bullet.transform.FindChild("Armature").gameObject.transform.FindChild("Bone").gameObject.GetComponentInChildren<Attack_Parameter>().power *= 2;
        }

        SE.PlayOneShot(se[0]);//SE
        
    }

}
