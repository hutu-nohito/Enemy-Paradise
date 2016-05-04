using UnityEngine;
using System.Collections;

public class Sippuu : Magic_Parameter {

    public GameObject bullet_Prefab;//弾のプレハブ

    private Magic_Controller MC;
    private Player_ControllerZ pcZ;
    private Z_Camera z_camera;

    private GameObject bullet;
    private Vector3 StartPos;

    //動かすよう
    private bool flag_Sippuu = false;//動いてるとき
    public Vector3 EndPos;
    private Vector3 deltaPos;

    private float elapsedTime;

    //入力受付
    private bool flag_Input = false;
    public float inputTime = 0.5f;//入力受付時間
    private int inputCount = 0;//入力は一回まで

    // Use this for initialization
    void Start()
    {
        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
        pcZ = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerZ>();
        z_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Z_Camera>();
        bullet = GameObject.Instantiate(bullet_Prefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (flag_Sippuu)
        {
            Parent.transform.position += deltaPos * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            if (elapsedTime > bullet.GetComponent<Attack_Parameter>().GetR_Time())
            {

                //Parent.transform.position = EndPos;//正確に動かす必要はない

                elapsedTime = 0;
                if (inputCount < 1)
                {
                    flag_Input = true;
                }
                else
                {
                    Invoke("subbullet", bullet.GetComponent<Attack_Parameter>().GetA_Time());
                    inputCount--;
                }
                flag_Sippuu = false;
                
            }
        }

        if (flag_Input)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > inputTime)
            {
                flag_Input = false;
                elapsedTime = 0;
                Invoke("subbullet", bullet.GetComponent<Attack_Parameter>().GetA_Time());
            }
            if (Input.GetKeyDown(KeyCode.A))//左
            {
                StartPos = Parent.transform.position;
                EndPos = StartPos + Parent.transform.TransformDirection(Vector3.left) * bullet.GetComponent<Attack_Parameter>().GetSpeed();
                deltaPos = (EndPos - StartPos) / bullet.GetComponent<Attack_Parameter>().GetR_Time();
                //高さ方向は移動しない
                deltaPos.y = 0;
                EndPos.y = 0;
                elapsedTime = 0;
                inputCount++;
                flag_Sippuu = true;
                flag_Input = false;
            }
            if (Input.GetKeyDown(KeyCode.D))//右
            {
                StartPos = Parent.transform.position;
                EndPos = StartPos + Parent.transform.TransformDirection(Vector3.right) * bullet.GetComponent<Attack_Parameter>().GetSpeed();
                deltaPos = (EndPos - StartPos) / bullet.GetComponent<Attack_Parameter>().GetR_Time();
                //高さ方向は移動しない
                deltaPos.y = 0;
                EndPos.y = 0;
                elapsedTime = 0;
                inputCount++;
                flag_Sippuu = true;
                flag_Input = false;
            }
            if (Input.GetKeyDown(KeyCode.S))//後ろ
            {
                EndPos = StartPos;
                StartPos = Parent.transform.position;
                deltaPos = (EndPos - StartPos) / bullet.GetComponent<Attack_Parameter>().GetR_Time();
                //高さ方向は移動しない
                deltaPos.y = 0;
                EndPos.y = 0;
                elapsedTime = 0;
                inputCount++;
                flag_Sippuu = true;
                flag_Input = false;
            }
        }

    }

    void Fire()
    {
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        Parent.GetComponent<Character_Manager>().SetKeylock();
        
        //MC.AddExistBullet(bullet);//現在の弾数を増やす
        bullet.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

        pcZ.SetMP(pcZ.GetMP() - GetSMP());

        StartPos = Parent.transform.position;//もとの位置

        if (Parent.GetComponent<Player_ControllerZ>().GetF_Watch())
        {
            //これで敵の目の前に行ける
            EndPos = StartPos + new Vector3(z_camera.Target.transform.position.x - Parent.transform.position.x - Parent.GetComponent<Player_ControllerZ>().GetDirection().x * z_camera.Target.transform.localScale.x,
                                                    z_camera.Target.transform.position.y - (Parent.transform.position.y + 0.8f) - Parent.GetComponent<Player_ControllerZ>().GetDirection().y * z_camera.Target.transform.localScale.y,
                                                    z_camera.Target.transform.position.z - Parent.transform.position.z - Parent.GetComponent<Player_ControllerZ>().GetDirection().z * z_camera.Target.transform.localScale.z);

            //Parent.transform.position += Iti;
        }
        else//注目してなかったら前へ
        {
            
            EndPos = StartPos + Parent.transform.TransformDirection(Vector3.forward) * bullet.GetComponent<Attack_Parameter>().GetSpeed();
            
        }

        deltaPos = (EndPos - StartPos) / bullet.GetComponent<Attack_Parameter>().GetR_Time();
        //高さ方向は移動しない
        deltaPos.y = 0;
        EndPos.y = 0;
        flag_Sippuu = true;
        /*if(!audioSource.isPlaying){
			
            audioSource.Play();
			
        }*/

        yield return new WaitForSeconds(bullet.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Manager>().SetActive();

    }
}
