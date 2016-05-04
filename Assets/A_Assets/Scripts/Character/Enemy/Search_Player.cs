using UnityEngine;
using System.Collections;

public class Search_Player : MonoBehaviour {

	private Enemy_ControllerZ ecZ;//敵の状態
    private GameObject Player;

	// Use this for initialization
	void Start () {

        ecZ = GetComponentInParent<Enemy_ControllerZ>();
        Player = GameObject.FindGameObjectWithTag("Player");

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider col){

		if(col.tag == "Player"){
            //トリガだけ渡す
            ecZ.Find();
        }
        if (col.tag == "Bullet")
        {
            if(col.GetComponent<Attack_Parameter>().GetParent() == Player)
                if(ecZ.GetSearch() == Enemy_ControllerZ.Enemy_Search.Magic) ecZ.Find();
        }
	}

	void OnTriggerExit (Collider col){

        if (col.tag == "Player")
        {
            //トリガだけ渡す
            ecZ.NotFind();
        }
	}
}
