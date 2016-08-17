using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour {

    private GameObject Manager;

    public GameObject[] Button;
    public GameObject RightButton;
    public GameObject LeftButton;
    public GameObject save_text;
    public GameObject bed_text;

    private int Rotnum = 0;//今どっちを向いてるか
    private bool isRot = false;
    private float RotAng = 72;
    private Vector3 newAng = Vector3.zero;//新しいのとっとく
    public float RotTime = 1.0f;
    private float elapsedTime = 0.0f; 

	// Use this for initialization
	void Start () {
        
        Manager = GameObject.FindGameObjectWithTag("Manager");
        //newAng = new Vector3(0, Camera.main.transform.eulerAngles.y + 72, 0);

        //一応初期化
        Button[0].SetActive(true);
        for (int i = 1;i < Button.Length;i++)
        {

            Button[i].SetActive(false);

        }

	}
	
	// Update is called once per frame
	void Update () {

        //カメラを回す
        if (isRot)
        {
            for (int i = 0; i < Button.Length; i++)
            {

                Button[i].SetActive(false);

            }

            elapsedTime += Time.deltaTime;
            Camera.main.transform.Rotate(new Vector3(0f, RotAng * Time.deltaTime, 0f));
            
            if(elapsedTime > RotTime)
            {
                Camera.main.transform.eulerAngles = newAng;
                elapsedTime = 0.0f;
                isRot = false;
            }
            //
        }
        else
        {
            switch (Rotnum)
            {
                case 0:
                    Button[0].SetActive(true);
                    break;
                case 1:
                    Button[1].SetActive(true);
                    break;
                case 2:
                    Button[2].SetActive(true);
                    break;
                case 3:
                    Button[3].SetActive(true);
                    break;
                case 4:
                    Button[4].SetActive(true);
                    break;
            }
        }    
        
    }

    public void Guild()
    {
        Manager.GetComponent<SceneTransition>().Guild();
    }
    public void SaveText()
    {
        //確認を出す

        Button[1].SetActive(false);
        save_text.SetActive(true);
    }
    public void Save()
    {
        //ゲームをやめる
        Application.Quit();
        //Save
        /*Manager.GetComponent<Static>().Save();
        Manager.GetComponent<SceneManager>().Fade();*/
        //Button[1].SetActive(true);
    }
    public void Bed()
    {
        //確認を出す
        
        //これで暗転して戻るはず
        Manager.GetComponent<SceneTransition>().Fade();
        Manager.GetComponent<Static>().SetDay(0.5f);//半日進める
        Manager.GetComponent<Static>().SetHP(100);//体力全快

        //音鳴らす
    }
    public void Sound()
    {
        //サウンドテストのシーンへ
        //Manager.GetComponent<SceneManager>().Guild();
    }
    public void Sukima()
    {
        //スキマのシーンへ
        Manager.GetComponent<SceneTransition>().Sukima();
    }

    public void Right()
    {
        if (!isRot)
        {
            RotAng = 72;
            newAng = new Vector3(0, Camera.main.transform.eulerAngles.y + RotAng, 0);
            isRot = true;

            if(Rotnum < 4)
            {
                Rotnum++;
            }
            else
            {
                Rotnum = 0;
            }
            
        }
        
    }

    public void Left()
    {
        if (!isRot)
        {
            RotAng = -72;
            newAng = new Vector3(0, Camera.main.transform.eulerAngles.y + RotAng, 0);
            isRot = true;
            if (Rotnum > 0)
            {
                Rotnum--;
            }
            else
            {
                Rotnum = 4;
            }
        }
    }
}
