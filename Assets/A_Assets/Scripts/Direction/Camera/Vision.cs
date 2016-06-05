using UnityEngine;
using System.Collections;
using System.Collections.Generic;//List用

public class Vision : MonoBehaviour {

    /*
    注目側の機能
        */

    /// <summary>
    /// 今んとこVisibleは使わないほうがよさげ
    /// </summary>
    //リストのURL　可変長配列でゲームが動いてる間にサイズを変えるならこれ
    //http://ft-lab.ne.jp/cgi-bin-unity/wiki.cgi?page=unity_cs_list
    private List<GameObject> Target = new List<GameObject>();//Vision 内の敵をすべて格納
    private GameObject Player;

    public GameObject nearTarget = null;//一番近い敵
    public float ReleaseLength = 30;//注目が途切れる距離

    // Use this for initialization
    void Start () {

        Player = GameObject.FindGameObjectWithTag("Player");

	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (Target.Count > 0)
        {

            MeasureTarget();//近い敵を探す

        }
        else//ターゲットがいなかったらマーカを出さない
        {
            
        }  

	}

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Enemy"){

            for (int i = 0; i < Target.Count; i++)
            {
                //同じターゲットを重複させないようにしてる
                if (col.gameObject == Target[i])
                {
                    return;//これでよさそう

                }
            }

            Target.Add(col.gameObject);

        }

    }

    void ReleaseTarget(int num)//距離が一定以上離れた敵を配列から外す
    {

        Target.RemoveAt(num);//遠いターゲットをリリース

    }

    void MeasureTarget()//一番近い敵を探す
    {
        nearTarget = null;//一番近い敵を注目に渡す
        float length = 0;//敵との距離
        float near = 100;//一番近い距離

        for (int i = 0; i < Target.Count; i++)
        {
            //ターゲットが消滅したら消す
            if (Target[i] == null)
            {
                ReleaseTarget(i);
                break;
            }
            length = (Player.transform.position - Target[i].transform.position).magnitude;
            if ((Player.transform.position - Target[i].transform.position).magnitude > ReleaseLength) { ReleaseTarget(i); }//Playerからの距離でいい

            if (length < near)
            {//近かったら

                near = length;
                if(Target.Count > 0)nearTarget = Target[i];//条件入れとかないと0でも処理が入る

            }
        }

        Visible();//見えてるかどうかチェック
        SetTarget(near);
    }

    void Visible()//TargetとPlayerの間に何もないことを確認。なんかあったら近くの敵から外す 自分で打った球も遮蔽物になるので注意
    {
        RaycastHit hit;

        GameObject hitObject;
        

        if (nearTarget != null)
        {
			Vector3 LineStart = new Vector3(Player.transform.position.x, Player.transform.position.y + 1.5f, Player.transform.position.z);
			Vector3 LineDirection = new Vector3 (nearTarget.transform.position.x - Player.transform.position.x,
			                                     nearTarget.transform.position.y - (Player.transform.position.y + 1.0f),//Playerのy軸は少し上げておく
			                                     nearTarget.transform.position.z - Player.transform.position.z);

            //マスクしたレイヤとのみ反応する(1はデフォルト)
			if (Physics.Raycast(LineStart, LineDirection, out hit, 1000, 1))
            {
				hitObject = hit.collider.gameObject;//レイヤーがIgnoreLayerのObjectは弾かれる。

				Debug.DrawLine(LineStart, hit.point, Color.red);
                //Debug.Log(hitObject);
                
                if (hitObject.gameObject.tag != "Enemy")
                {

                    nearTarget = null;

                }
            }
        }

    }

    void SetTarget(float near)
    {
        if (Target.Count == 0) { Camera.main.GetComponent<Z_Camera>().SetTarget(null,near); }
        else
        {
            Camera.main.GetComponent<Z_Camera>().SetTarget(nearTarget,near);

        }

        
    }
}
