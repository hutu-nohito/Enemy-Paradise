using UnityEngine;
using System.Collections;

public class Hit : MonoBehaviour {

    public GameObject Parent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Bullet")
        {
            Debug.Log("www");
            if(col.gameObject.GetComponent<Attack_Parameter>().Parent.name == "Player")
            {
                Parent.GetComponent<monster_Cont2>().Mikiri();
            }
        }
    }
}
