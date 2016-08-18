using UnityEngine;
using System.Collections;
using UnityEngine.UI;//これが必要

public class monster_Gauge : MonoBehaviour {

    new private Text guiText;
    public GameObject Monster;
    public bool Player;

    // Use this for initialization
    void Start () {

        guiText = this.GetComponent<Text>();

        if (!Player)
        {
            Invoke("MonsterChange", 1.0f);
        }
    }

    void MonsterChange()
    {
        Monster = GameObject.FindGameObjectWithTag("Enemy");
    }
	
	// Update is called once per frame
	void Update () {

        if(Monster != null)
        {
            guiText.text = "Monster HP " + Monster.GetComponent<Character_Parameters>().H_point;
        }
        
    }
}
