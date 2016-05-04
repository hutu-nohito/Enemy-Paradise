using UnityEngine;
using System.Collections;

public class Block_Move : MonoBehaviour {

    public bool flag_LerpMove = false;//動かすかどうか
    public Vector3 MoveDirection;//動かすスピードになると思う
    public float MoveTime = 0;//動かす時間

    public Vector3 StartPos;
    public Vector3 EndPos;
    public float time = 10;
    private Vector3 deltaPos;

    private float elapsedTime;

    private GameObject DarkLilith;

    public GameObject Damage_H;//ダメージ判定　プレハブ
    public GameObject Effect;//光る　プレハブ
    private GameObject damage_H;//消すよう
    private GameObject effect;//消すよう
    private Vector3 Offset = new Vector3(0, 2, 0);//位置調整用

    // Use this for initialization
    void Start () {

        DarkLilith = GameObject.Find("Boss_DarkLilith");
        StartPos = transform.localPosition;
        deltaPos = (EndPos - StartPos) / time;
        elapsedTime = 0;

    }
	
	// Update is called once per frame
	void Update () {
	
        if(flag_LerpMove)
        {
                transform.localPosition += deltaPos * Time.deltaTime;
                elapsedTime += Time.deltaTime;
                if (elapsedTime > time)
                {

                    transform.localPosition = EndPos;

                    elapsedTime = 0;
                flag_LerpMove = false;

                }
           /* transform.position += new Vector3( MoveDirection.x,
                MoveDirection.y,
                MoveDirection.z) * Time.deltaTime;//時間で足し算でいいや。直せそうならLerpに直す
                */


        }
	}

     public void LerpMove(Vector3 MoveDirection,float MoveTime)
    {
        //this.MoveDirection = MoveDirection / 30;//FPSがたぶん30。実際の値を使った方がいい
        this.MoveDirection = MoveDirection;//FPSがたぶん30。実際の値を使った方がいい
        this.MoveTime = MoveTime;
        effect = Instantiate(Effect, transform.position + Offset, Quaternion.identity) as GameObject;

        StartPos = transform.localPosition;
        EndPos = StartPos + MoveDirection;
        deltaPos = (EndPos - StartPos) / MoveTime;
        time = MoveTime;

        Invoke("MoveStart",1);

    }

    void MoveStart()
    {
        flag_LerpMove = true;
        damage_H = Instantiate(Damage_H, transform.position + Offset, Quaternion.identity) as GameObject;
        damage_H.GetComponent<Attack_Parameter>().SetParent(DarkLilith);
        Invoke("DirectCut",1);
    }

    void DirectCut()
    {
        Destroy(damage_H);
        Destroy(effect);
    }

     public void Stop()
     {
        flag_LerpMove = false;
     }
}
