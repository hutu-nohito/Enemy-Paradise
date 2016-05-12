using UnityEngine;
using System.Collections;
using UnityEngine.UI;//これが必要

public class monster_Gauge : MonoBehaviour {

    new private Text guiText;
    public GameObject Monster;

    // Use this for initialization
    void Start () {

        guiText = this.GetComponent<Text>();

    }
	
	// Update is called once per frame
	void Update () {

        guiText.text = "Monster HP " + Monster.GetComponent<monster_Cont2>().H_point;

    }
}
