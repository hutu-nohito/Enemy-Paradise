using UnityEngine;
using System.Collections;

public class Snipe : Magic_Parameter {
    public GameObject bullet_Prefab;//弾のプレハブ

    private Magic_Controller MC;
    private Player_ControllerZ pcZ;

    private bool isSnipe;//スコープ状態かどうか
    private Vector3 CameraStartPos;//カメラ初期位置

    // Use this for initialization
    void Start()
    {
        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcZ = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerZ>();

        CameraStartPos = Camera.main.gameObject.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        if (isSnipe)
        {
            //スコープ状態解除
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Camera.main.gameObject.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;
                Camera.main.gameObject.transform.localPosition = CameraStartPos;
                //硬直を解除
                Parent.GetComponent<Character_Manager>().SetActive();
                isSnipe = false;
            }

            if (Input.GetAxis("Mouse ScrollWheel") == 0.1f)
            {
                if(transform.localPosition.z < 40)
                    transform.Translate(0,0,0.5f);
            }
            if (Input.GetAxis("Mouse ScrollWheel") == -0.1f)
            {
                if (transform.localPosition.z > 2)
                    transform.Translate(0, 0, -0.5f);
            }
        }
    }

    void Fire()
    {
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        //通常時にFireしたとき
        if (!isSnipe)
        {
            Camera.main.gameObject.transform.position = transform.position;
            Camera.main.gameObject.transform.parent = transform;
            Parent.GetComponent<Character_Manager>().SetMovelock();
            isSnipe = true;
            yield break;
        }

        //スコープ状態でFireしたとき

        Parent.GetComponent<Character_Manager>().flag_magic = false;

        GameObject bullet;

        bullet = GameObject.Instantiate(bullet_Prefab);//弾生成
        MC.AddExistBullet(bullet);//現在の弾数を増やす
        bullet.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

        //MPの処理
        pcZ.SetMP(pcZ.GetMP() - GetSMP());

        //弾を飛ばす処理
        bullet.transform.position = transform.position + Parent.GetComponent<Character_Parameters>().direction;//前方に飛ばす
        bullet.transform.rotation = Quaternion.LookRotation(-(Parent.GetComponent<MousePoint>().worldPoint - Parent.transform.position).normalized);//回転させて弾頭を進行方向に向ける
        bullet.GetComponent<Rigidbody>().velocity = ((Parent.GetComponent<MousePoint>().worldPoint - Parent.transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed);

        //効果音と演出
        /*if(!audioSource.isPlaying){
			
            audioSource.Play();//SE
			
        }*/

        Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

        yield return new WaitForSeconds(bullet.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        Parent.GetComponent<Character_Manager>().flag_magic = true;

    }
}
