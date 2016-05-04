using UnityEngine;
using System.Collections;

public class Wall : Magic_Parameter {

    public GameObject bullet_Prefab;//弾のプレハブ

    private Magic_Controller MC;
    private Player_ControllerZ pcZ;

    private Animator animator;//アニメ
    private AudioSource SE;//音

    //魔法を打つ位置を決めるよう
    private Vector3 direction;
    private Vector3 move_direction = Vector3.zero;
    public GameObject point;
    private bool flag_move = false;
    private float move_speed = 0.01f;

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
        if (flag_move)
        {
            if ((transform.position - pcZ.gameObject.transform.position).magnitude > 15)//距離が離れすぎないように
            {
                move_direction -= (transform.position - pcZ.gameObject.transform.position).normalized / 10;
            }

            if (Input.GetKey(KeyCode.W))
            {
                /*move_direction.x += direction.x / move_time;
                move_direction.z += direction.z / move_time;*/
                //move_direction += pcZ.gameObject.transform.TransformDirection(Vector3.forward) * move_speed;
                move_direction += Vector3.forward * move_speed;

            }
            if (Input.GetKey(KeyCode.S))
            {
                /*move_direction.x -= direction.x / move_time;
                move_direction.z -= direction.z / move_time;*/
                //move_direction += pcZ.gameObject.transform.TransformDirection(Vector3.back) * move_speed; 
                move_direction += Vector3.back * move_speed;

            }
            if (Input.GetKey(KeyCode.A))
            {
                /*move_direction.x -= direction.z / move_time;
                move_direction.z += direction.x / move_time;*/
                //move_direction += pcZ.gameObject.transform.TransformDirection(Vector3.left) * move_speed;
                move_direction += Vector3.left * move_speed;

            }
            if (Input.GetKey(KeyCode.D))
            {
                /*move_direction.x += direction.z / move_time;
                move_direction.z -= direction.x / move_time;*/
                //move_direction += pcZ.gameObject.transform.TransformDirection(Vector3.right) * move_speed;
                move_direction += Vector3.right * move_speed;
            }

            direction = pcZ.gameObject.transform.TransformDirection(Vector3.forward);
            transform.localPosition = move_direction;
        }
        
    }

    void Hold()
    {
        point.SetActive(true);

        Parent.GetComponent<Character_Manager>().SetMovelock();

        flag_move = true;

    }

    void Fire()
    {
        flag_move = false;
        move_direction = Vector3.zero;
        point.SetActive(false);
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        Parent.GetComponent<Character_Manager>().SetKeylock();
        GameObject bullet;
        animator.SetTrigger("Shoot");

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

        Vector3 LineStart = new Vector3(bullet.transform.position.x, bullet.transform.position.y + 4 /*- zcamera.Target.transform.localScale.y / 2*/, bullet.transform.position.z);
        Vector3 LineDirection = Vector3.down;//下でおｋ

        if (Physics.Raycast(LineStart, LineDirection, out hit, 200))
        {
            hitObject = hit.collider.gameObject;//レイヤーがIgnoreLayerのObjectは弾かれる。

            //Debug.DrawLine(LineStart, hit.point, Color.blue);
            //Debug.Log(hitObject);

            //地面だったら
            if (hitObject.gameObject.name == "Terrain")
            {
                bullet.transform.position = transform.position - new Vector3(0, transform.position.y - hit.point.y, 0) - new Vector3(0, 4, 0);

            }
            else
            {
                bullet.transform.position = transform.position - new Vector3(0, transform.position.y - hit.point.y - hitObject.transform.localScale.y, 0) - new Vector3(0, 4, 0);
            }
        }
        else
        {
            bullet.transform.position = transform.position - new Vector3(0, 4, 0);
        }

        bullet.transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward).normalized);//回転させて弾頭を進行方向に向ける

        GameObject effect;
        effect = bullet.transform.FindChild("Spurt").gameObject;

        bullet.GetComponent<Rigidbody>().velocity = (Vector3.up * bullet.GetComponent<Attack_Parameter>().speed);

        yield return new WaitForSeconds(0.5f);//止める

        bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
        bullet.GetComponent<Rigidbody>().isKinematic = true;//固定
        Destroy(effect,1);
        bullet.GetComponent<Collider>().enabled = false;
        bullet.transform.FindChild("Wall_Model").gameObject.GetComponent<Collider>().enabled = true;
        Invoke("SEStop",1.3f);
        //bullet.GetComponent<MeshCollider>().isTrigger = false;

        Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());       

        yield return new WaitForSeconds(bullet.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Manager>().SetActive();

        transform.localPosition = Vector3.zero;//もとの位置に戻す

    }

    void SEStop()
    {
        SE.enabled = false;//SE止める
        CCZ.flag_quake = false;//揺れを止める
    }

}
