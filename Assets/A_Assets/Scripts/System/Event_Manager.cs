﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Event_Manager : MonoBehaviour {

    /******************************************************************************/
    /** @brief イベント管理
    * @date 2016/08/18
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    */
    /******************************************************************************/
    /* 更新履歴
    *  イベントのフラグ管理
    *  裏庭のチュートリアル追加
    */
    /******************************************************************************/

    //特別なイベントがある場合にこれが呼び出される(Manager)
    //ついでにインフォメーションのON/OFF管理

    public TextAsset[] EventText;//ここにイベント用のテキストを格納
    private bool[] EventFlag = new bool[100];//イベントフラグ、使ったらおろしてく

    /*
        0:ホーム一日目
        1:ギルド一日目
    */

	public GameObject Canvas;//UI
    public GameObject Information;//情報表示
    private uGUI_Msg uGM;
    public GameObject[] YesNo;
    private int story = 0;//文章をどこまで読んだか

    private Static save;//日数、起動回数、HP、MP、名声、ボーナスポイント
    private SceneTransition ST;
    private Player_ControllerVR pcVR;

	//コルーチン
	private Coroutine coroutine;
	private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
	private bool isCoroutineH = false;
	private bool isCoroutineG = false;
    private bool isCoroutineBY = false;
    private bool olduGM = false;//メッセージウィンドウを開いていたかどうか

    //チュートリアル用（突貫　後でどうにかする）
    public int TutorialStep = 0;//チュートリアルのステップ数(頑張れば使いまわせそう)
    public GameObject[] TutorialCube;//とりあえず直入れ

    // Use this for initialization
    void Start() {

        save = GetComponent<Static>();
        ST = GetComponent<SceneTransition>();
        if (Canvas == null)
        {
            Canvas = GameObject.Find("Msg_Canvas");
        }

        //これはシーンが変わるごとに必要(とりあえずプレイヤーの動きはとめない)
        //pcVR = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerVR>();

        uGM = Canvas.GetComponent<uGUI_Msg>();
        uGM.enabled = false;//消しとく

        for (int i = 0; i < EventFlag.Length; i++)
        {
            EventFlag[i] = false;//初期化　イベントをセーブするようになったらその時考える
        }

        //デバッグ用
        if (SceneManager.GetActiveScene().name == "Backyard" || SceneManager.GetActiveScene().name == "BackyardVR")
        {
            //チュートリアル
            for (int i = 0; i < TutorialCube.Length; i++)
            {
                TutorialCube[i].SetActive(false);//消しとく
            }
            TutorialStep = 0;

            StartCoroutine(Backyard_T());
        }            

    }
	
	// Update is called once per frame
	void Update () {

        if (olduGM)
        {
            //メッセージが非表示になった瞬間
            if(olduGM != uGM.enabled)
            {
                TutorialClear();
            }
        }

        olduGM = uGM.enabled;

        //チュートリアルチェック
        
	}

    //シーンが変わったらイベントチェック
    void Check_Event()
    {
        if(SceneManager.GetActiveScene().name == "Home")
            StartCoroutine(Home_T());

        if (SceneManager.GetActiveScene().name == "Guild")
            StartCoroutine(guild_T());

        if (SceneManager.GetActiveScene().name == "title")
        {
            Information.SetActive(false);
        }
        else
        {
            //Information.SetActive(true);/消しとく
        }

    }

    public void Title_T(bool flag_true){//タイトルでのイベント
        
        uGM.enabled = true;//つける
        if (flag_true)
        {
            uGM.dispMessage(EventText[0]);//表示する
        }
        else
        {
            uGM.dispMessage(EventText[1]);//表示する
        }
        
    }
    IEnumerator Home_T(){//ホームでのイベント

        if (isCoroutineH){yield break;}
		isCoroutineH = true;

        //一日目
        if (!EventFlag[0])
        {
            if (save.GetDay() == 1)
            {
                uGM.enabled = true;//つける
                uGM.dispMessage(EventText[0]);//表示する
                EventFlag[0] = true;

            }
        }
		
		isCoroutineH = false;
		
	}
	IEnumerator guild_T(){//ギルドでのイベント
        
		if(isCoroutineG){yield break;}
		isCoroutineG = true;

        if (!EventFlag[1])
        {
            
            if(save.GetGL() == 0)//初級1のみの時
            {
                uGM.enabled = true;//つける
                uGM.dispMessage(EventText[2]);//表示する
                EventFlag[1] = true;
            }
            //if (save.GetDay() == 1)
            //{
            //    uGM.enabled = true;//つける
            //    uGM.dispMessage(EventText[1]);//表示する
            //    EventFlag[1] = true;

            //}
        }
            
		isCoroutineG = false;
		
	}

    public void storyCount()
    {
        story++;

        if (SceneManager.GetActiveScene().name != "BackyardVR")
        {
            //後でこっから移動させる
            if (story == 5)
            {
                YesNo[0].SetActive(true);//Yesボタン
                YesNo[1].SetActive(true);//Noボタン
            }
        }
            
    }
    public void storyReset()
    {
        story = 0;
        //TutorialStep++;
    }
    //イベントのはいいいえを選ぶボタン（今はチュートリアルを受けるかどうかだけ選択）
    public void YesButton()
    {
        YesNo[0].SetActive(false);//Yesボタン
        YesNo[1].SetActive(false);//Noボタン
        ST.Backyard();
    }
    public void NoButton()
    {
        YesNo[0].SetActive(false);//Yesボタン
        YesNo[1].SetActive(false);//Noボタン
        uGM.enabled = true;//つける
        uGM.dispMessage(EventText[3]);//表示する
    }

    IEnumerator Backyard_T()//裏庭でのイベント
    {
        if (isCoroutineBY) { yield break; }
        isCoroutineBY = true;

        
        //偶数番目で説明、奇数番目で実践(奇数の時はメッセージを表示しない)
        if (TutorialStep == 0)
        {
            //pcVR.SetKeylock();
            uGM.enabled = true;//つける
            uGM.dispMessage(EventText[4]);//表示する
            
        }
        if (TutorialStep == 1)//敵を探す
        {
            TutorialCube[0].SetActive(true);
            //pcVR.SetActive();
            
        }
        if (TutorialStep == 2)
        {
            TutorialCube[0].SetActive(false);
            //pcVR.SetKeylock();
            uGM.enabled = true;//つける
            uGM.dispMessage(EventText[5]);//表示する
        }
        if (TutorialStep == 3)//魔法陣を出す
        {
            //pcVR.SetActive();
            TutorialCube[1].SetActive(true);
        }
        if (TutorialStep == 4)
        {
            TutorialCube[1].SetActive(false);
            //pcVR.SetKeylock();
            uGM.enabled = true;//つける
            uGM.dispMessage(EventText[6]);//表示する
        }
        if (TutorialStep == 5)//魔法を作る
        {
            //pcVR.SetActive();
            TutorialCube[2].SetActive(true);
        }
        if (TutorialStep == 6)
        {
            TutorialCube[2].SetActive(false);
            //pcVR.SetKeylock();
            uGM.enabled = true;//つける
            uGM.dispMessage(EventText[7]);//表示する
        }
        if (TutorialStep == 7)
        {
            uGM.enabled = true;//つける
            uGM.dispMessage(EventText[8]);//表示する
            //pcVR.SetActive();
        }

        isCoroutineBY = false;
    }

    //チュートリアルのステップを外から進める
    public void TutorialClear()
    {
        TutorialStep++;

        if (SceneManager.GetActiveScene().name == "Backyard" || SceneManager.GetActiveScene().name == "BackyardVR")
        {
            StartCoroutine(Backyard_T());
        }
            
    }

}
