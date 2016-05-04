using UnityEngine;
using System.Collections;
using System.Collections.Generic;//List用

public class Territory : MonoBehaviour {

    //縄張り。縄張りの概念がなくても動く敵にはつけとかないとどっか行っちゃう

    public List<GameObject> Enemies = new List<GameObject>();//縄張りは共有するかもなので一応Listにしとく

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerExit(Collider col)
    {
        for (int i = 0;i < Enemies.Count; i++)
        {
            if(col.gameObject == Enemies[i])
            {
                Enemies[i].GetComponent<Enemy_ControllerZ>().Return();//縄張りの中央に戻らせる
                
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            if (col.gameObject == Enemies[i])
            {
                Enemies[i].GetComponent<Enemy_ControllerZ>().NotReturn();

            }
        }
    }
}
