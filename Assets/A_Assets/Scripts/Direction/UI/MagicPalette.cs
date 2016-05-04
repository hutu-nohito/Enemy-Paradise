using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MagicPalette : MonoBehaviour {

    private Magic_Controller MC;
    public GameObject[] Icon;
    public Sprite[] IconTexture;
    public GameObject Waku;

	// Use this for initialization
	void Start () {

        MC = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();

        for(int i = 0;i < Icon.Length; i++)
        {
            Icon[i].GetComponent<Image>().sprite = IconTexture[MC.selectmagic[i]];
        }
        

	}
	
	// Update is called once per frame
	void Update () {

        //枠式
        
        switch (MC.magic_num)
        {
            case 0:
                Waku.transform.position = Icon[0].transform.position;
                break;
            case 1:
                Waku.transform.position = Icon[1].transform.position;
                break;
            case 2:
                Waku.transform.position = Icon[2].transform.position;
                break;
            case 3:
                Waku.transform.position = Icon[3].transform.position;
                break;
            case 4:
                Waku.transform.position = Icon[4].transform.position;
                break;
        }
        

        //大きくする
        /*
        switch (MC.magic_num)
        {
            case 0:
                Waku.transform.position = Icon[0].transform.position;
                break;
            case 1:
                Waku.transform.position = Icon[1].transform.position;
                break;
            case 2:
                Waku.transform.position = Icon[2].transform.position;
                break;
            case 3:
                Waku.transform.position = Icon[3].transform.position;
                break;
            case 4:
                Waku.transform.position = Icon[4].transform.position;
                break;
        }
        */

        //選択されたアイコンに回転
        /*
        switch (MC.magic_num)
        {
            case 0:
                transform.Rotate(0, 72, 0);

                break;
            case 1:
                transform.Rotate(0, 72, 0);

                break;
            case 2:
                transform.Rotate(0, 72, 0);

                break;
            case 3:
                transform.Rotate(0, 72, 0);

                break;
            case 4:
                transform.Rotate(0, 72, 0);

                break;
        }
        */


    }
}
