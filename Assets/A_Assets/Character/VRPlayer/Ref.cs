using UnityEngine;
using System.Collections;

public class Ref : MonoBehaviour {

    private AudioSource SE;
    public AudioClip se;
    
	// Use this for initialization
	void Start () {

        SE = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Bullet")
        {

            col.GetComponent<Rigidbody>().velocity *= -1;//純粋な反射
            SE.PlayOneShot(se);
            //if (col.GetComponent<Attack_Parameter>().GetParent().tag == "Player")
            //{
            //    col.transform.rotation = Quaternion.LookRotation(-(DarkLilith.transform.position - col.transform.position).normalized);//回転させて弾頭を進行方向に向ける
            //    col.GetComponent<Rigidbody>().velocity = ((DarkLilith.transform.position - transform.position).normalized + Offset) * col.GetComponent<Rigidbody>().velocity.magnitude;
            //    col.GetComponent<Attack_Parameter>().SetParent(this.gameObject);
            //}

            //if (col.GetComponent<Attack_Parameter>().GetParent().tag == "Enemy")
            //{
            //    col.transform.rotation = Quaternion.LookRotation(-(Player.transform.position - col.transform.position).normalized);//回転させて弾頭を進行方向に向ける
            //    col.GetComponent<Rigidbody>().velocity = ((Player.transform.position - transform.position).normalized + Offset) * col.GetComponent<Rigidbody>().velocity.magnitude;
            //    //col.GetComponent<Attack_Parameter>().SetParent(this.gameObject);自分の弾に当たっても死なないように消しとく
            //}


        }
    }
}
