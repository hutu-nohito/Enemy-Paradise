using UnityEngine;
using System.Collections;

public class Wall_Bullet : Attack_Parameter {

    /*public bool flag_LerpMove = false;//動かすかどうか

    public Vector3 StartPos;
    public Vector3 EndPos;
    public float Movetime = 2;
    private Vector3 deltaPos;

    private float elapsedTime;

    //public GameObject Damage_H;//ダメージ判定　プレハブ
    public GameObject Effect;//光る　プレハブ
    //private GameObject damage_H;//消すよう
    private GameObject effect;//消すよう
    private Vector3 Offset = new Vector3(0, 5, 0);//位置調整用
    */

    // Use this for initialization
    void Start()
    {
        //effect = Instantiate(Effect, transform.position + Offset, Quaternion.identity) as GameObject;
        /*StartPos = transform.localPosition;
        EndPos += StartPos;
        deltaPos = (EndPos - StartPos) / Movetime;
        elapsedTime = 0;

        Invoke("MoveStart", 0.5f);*/

    }

    // Update is called once per frame
    void Update()
    {

        /*if (flag_LerpMove)
        {
            transform.localPosition += deltaPos * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            if (elapsedTime > Movetime)
            {

                //transform.localPosition = EndPos;

                elapsedTime = 0;
                flag_LerpMove = false;

            }
        }*/
    }

    /*
    void MoveStart()
    {
        flag_LerpMove = true;
        //damage_H = Instantiate(Damage_H, transform.position + Offset, Quaternion.identity) as GameObject;
        //damage_H.GetComponent<Attack_Parameter>().SetParent(DarkLilith);
        Invoke("DirectCut", 1);
    }

    void DirectCut()
    {
        //Destroy(damage_H);
        //Destroy(effect);
        this.GetComponent<Collider>().enabled = false;
        transform.FindChild("Wall_Model").gameObject.GetComponent<Collider>().enabled = true;
    }
    */
}
