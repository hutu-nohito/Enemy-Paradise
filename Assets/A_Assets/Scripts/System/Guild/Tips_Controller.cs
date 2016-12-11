using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tips_Controller : MonoBehaviour {

    private Static _static;

    [SerializeField]
    private Sprite[] Tips;
    [SerializeField]
    private GameObject[] Runes;
    [SerializeField]
    private GameObject[] Magics;

    private Image Tip;

	// Use this for initialization
	void Start () {

        _static = GameObject.FindGameObjectWithTag("Manager").GetComponent<Static>();

        Tip = GetComponentInChildren<Image>();
        Tip.sprite = Tips[Random.Range(0, Tips.Length - 5)];//ピラーまで
        if (_static.guild_level >= 3)
        {
            Tip.sprite = Tips[Random.Range(0, Tips.Length - 3)];//キャリバーまで
            //Tip.sprite = Tips[0];
            Runes[0].SetActive(true);
            Magics[1].SetActive(true);
            Magics[2].SetActive(true);
        }
        if (_static.guild_level >= 6)
        {
            Tip.sprite = Tips[Random.Range(0, Tips.Length)];//全部
            Runes[1].SetActive(true);
            Magics[3].SetActive(true);
            Magics[4].SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
