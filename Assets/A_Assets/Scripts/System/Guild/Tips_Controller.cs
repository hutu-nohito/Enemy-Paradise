using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tips_Controller : MonoBehaviour {

    private Static _static;

    public Sprite[] Tips;//0と1がセーブルーン
    private Image Tip;

	// Use this for initialization
	void Start () {

        _static = GameObject.FindGameObjectWithTag("Manager").GetComponent<Static>();

        Tip = GetComponentInChildren<Image>();
        Tip.sprite = Tips[Random.Range(2, Tips.Length + 1)];
        if (_static.guild_level == 3)
        {
            Tip.sprite = Tips[0];
        }
        if (_static.guild_level == 6)
        {
            Tip.sprite = Tips[1];
        }
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
