using UnityEngine;
using System.Collections;

public class SimpleCol : MonoBehaviour {

    public enum Type
    {
        Destroy,//消す
        Through//貫通
    }
    public Type type = Type.Destroy;

    public bool ThroughEnemy = true;//敵貫通
    public bool ThroughBullet = true;//弾貫通
    public bool ThroughPlayer = true;//自分貫通
    public bool ThroughTerrain = true;//地形貫通

    private Magic_Controller MC;

    // Use this for initialization
    void Start () {

        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();

	}

    void OnTriggerEnter(Collider col)
    {
        //Debug.Log(col.gameObject.name);
        //設定で当たらなくした
        /*if (col.gameObject.name == "Vision" || col.gameObject.name == "Search" || col.gameObject.name == "Territory")
        {
            return;
        }*/
        if (col.tag == "Reflect")
        {
            return;
        }
        switch (type){
            case Type.Destroy:

                Destroy(this.gameObject);
                break;
            case Type.Through:

                //貫通するかどうか
                if (col.tag == "Player")
                {
                    if (ThroughPlayer) break;
                    else
                    {
                        Destroy(this.gameObject);
                    }
                }
                if (col.tag == "Enemy")
                {
                    if (ThroughEnemy) break;
                    else
                    {
                        Destroy(this.gameObject);
                    }
                }

                if (col.tag == "Bullet")//相殺するかどうか
                {
                    if (ThroughBullet) break;
                    else
                    {
                        Destroy(this.gameObject);
                    }
                }
                
                //地形貫通
                if (col.gameObject.layer == 1)//レイやがDefaultの時(地形とキャラ)
                {
                    if (ThroughTerrain) break;
                    else
                    {
                        Destroy(this.gameObject);
                    }
                }
                break;
                    default:
                break;
        }
    }
}
