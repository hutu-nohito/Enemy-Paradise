using UnityEngine;
using System.Collections;

public class Arrow : Magic_Parameter {

    private Magic_Controller MC;
    private Player_ControllerZ pcZ;

    private Animator animator;//アニメ
    private AudioSource SE;//音

    public GameObject Bullet;//矢のObject

    private float time = 0;//矢を飛ばすまでの時間

	// Use this for initialization
	void Start () {
        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcZ = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerZ>();

        animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        SE = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void Hold()
    {
        time += Time.deltaTime;
        Parent.GetComponent<Character_Manager>().SetMovelock();
        animator.SetTrigger("Shoot");
    }

    void Fire()
    {
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        Parent.GetComponent<Character_Manager>().SetKeylock();
        GameObject bullet;

        bullet = GameObject.Instantiate(Bullet);
        MC.AddExistBullet(bullet);//現在の弾数を増やす
        bullet.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

        pcZ.SetMP(pcZ.GetMP() - GetSMP());

        animator.SetTrigger("Shoot");

        if (time >= 2.0f) { time = 2.0f; }//1秒までスピードに反映
        Debug.Log(time);

        //弾を飛ばす処理
        bullet.transform.position = transform.position;//Muzzleの位置
        bullet.transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.back).normalized);//回転させて弾頭を進行方向に向ける
        //カメラとキャラの向きが90°以上ずれてたら
        if (Vector3.Dot(pcZ.direction.normalized, Parent.transform.TransformDirection(Vector3.forward).normalized) < 0)//二つのベクトル間の角度が90°以上(たぶん)
        {
            bullet.GetComponent<Rigidbody>().velocity = Parent.transform.TransformDirection(Vector3.forward).normalized * bullet.GetComponent<Attack_Parameter>().speed * (time * time);//キャラの向いてる方向
        }
        else
        {
            bullet.GetComponent<Rigidbody>().velocity = (Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 50)) - transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed * (time * time);//画面の真ん中
        }
        //注目中だったら
        if (pcZ.GetF_Watch())
        {
            //ちょっと上を狙わないと地面に向かってく
            bullet.GetComponent<Rigidbody>().velocity = (Camera.main.GetComponent<Z_Camera>().Target.transform.position + new Vector3(0, Camera.main.GetComponent<Z_Camera>().Target.transform.localScale.y, 0) - transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed * (time * time);//敵の方向
        }

        //効果音と演出
        if (!SE.isPlaying)
        {

            SE.PlayOneShot(SE.clip);//SE

        }

        //弾を飛ばす処理
        /*bullet.transform.position = transform.position + Parent.GetComponent<Character_Parameters>().direction;
        bullet.transform.rotation = Quaternion.LookRotation(-(Parent.GetComponent<MousePoint>().worldPoint - Parent.transform.position).normalized);//回転させて矢じりを進行方向に向ける
        bullet.GetComponent<Rigidbody>().velocity = ((Parent.GetComponent<MousePoint>().worldPoint - Parent.transform.position).normalized * Bullet.GetComponent<Attack_Parameter>().speed * (time * time));
        */
        /*if(!audioSource.isPlaying){
			
            audioSource.Play();
			
        }*/

        Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

        yield return new WaitForSeconds(bullet.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Manager>().SetActive();
        time = 0;

    }

}
