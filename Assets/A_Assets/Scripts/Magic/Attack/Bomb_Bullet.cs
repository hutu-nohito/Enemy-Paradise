using UnityEngine;
using System.Collections;

public class Bomb_Bullet : Attack_Parameter {

    public GameObject Blast_Bullet;

    // Use this for initialization
    void Start()
    {

        Invoke("Bomb_Blast", GetA_Time());

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "Vision" || col.gameObject.name == "Search" || col.gameObject.name == "Territory")
        {
            return;
        }
        if (col.tag == "Reflect" || col.tag == "HitPlayer" || col.tag == "Player")
        {
            return;
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Event"))//イベントレイヤ
        {
            return;
        }
        

        Bomb_Blast();

    }

    void Bomb_Blast()
    {

        GameObject Blast = Instantiate(Blast_Bullet);
        Blast.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある
        Blast.transform.position = this.transform.position;
        Destroy(Blast, Blast.GetComponent<Attack_Parameter>().GetA_Time());
        Destroy(this.gameObject);

    }
}
