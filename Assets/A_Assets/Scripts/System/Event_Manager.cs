using UnityEngine;
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
        1:ホーム一日目
        2:ギルド一日目
    */

	public GameObject Canvas;//UI
    public GameObject Information;//情報表示
    private uGUI_Msg uGM;

    private Static save;//日数、起動回数、HP、MP、名声、ボーナスポイント
    private SceneManager SM;
    private Player_ControllerVR pcVR;

	//コルーチン
	private Coroutine coroutine;
	private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
	private bool isCoroutineH = false;
	private bool isCoroutineG = false;
    private bool isCoroutineBY = false;
    private int TutorialStep = 0;//チュートリアルのステップ数(頑張れば使いまわせそう)
    private bool olduGM = false;//メッセージウィンドウを開いていたかどうか

    // Use this for initialization
    void Start () {
	
        save = GetComponent<Static>();
        SM = GetComponent<SceneManager>();
		Canvas = GameObject.Find ("Msg_Canvas");
        pcVR = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerVR>();

        uGM = Canvas.GetComponent<uGUI_Msg>();
        uGM.enabled = false;//消しとく

        for(int i = 0;i < EventFlag.Length; i++)
        {
            EventFlag[i] = false;//初期化　イベントをセーブするようになったらその時考える
        }

        //デバッグ用
        StartCoroutine(Backyard_T());

    }
	
	// Update is called once per frame
	void Update () {

        if (olduGM)
        {
            //たぶんメッセージが非表示になった瞬間
            if(olduGM != uGM.enabled)
            {
                Debug.Log("www");
            }
        }

        olduGM = uGM.enabled;
	}

    //シーンが変わったらイベントチェック
    void Check_Event()
    {
        if(Application.loadedLevelName == "Home")
            StartCoroutine(Home_T());

        if (Application.loadedLevelName == "guild")
            StartCoroutine(guild_T());

        if (Application.loadedLevelName == "Title")
        {
            Information.SetActive(false);
        }
        else
        {
            Information.SetActive(true);
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
            

            if (save.GetDay() == 1)
            {
                uGM.enabled = true;//つける
                uGM.dispMessage(EventText[1]);//表示する
                EventFlag[1] = true;

            }
        }
            
		isCoroutineG = false;
		
	}

    IEnumerator Backyard_T()//裏庭でのイベント
    {
        if (isCoroutineBY) { yield break; }
        isCoroutineBY = true;

        while (true)
        {
            //偶数番目で説明、奇数番目で実践(奇数の時はメッセージを表示しない)
            if(TutorialStep == 0)
            {
                pcVR.SetKeylock();
                uGM.enabled = true;//つける
                uGM.dispMessage(EventText[2]);//表示する
                TutorialStep++;
            }
            if (TutorialStep == 1)
            {
                pcVR.SetActive();
            }
                if (TutorialStep == 2)
            {
                pcVR.SetKeylock();
                uGM.enabled = true;//つける
                uGM.dispMessage(EventText[3]);//表示する
                TutorialStep++;
            }
            if (TutorialStep == 3)
            {
                pcVR.SetActive();
            }

            if (Application.loadedLevelName != "Backyard")//裏庭から移動したら抜ける
            {
                break;
            }
        }
    }

    //チュートリアルのステップを外から進める
    public void TutorialClear()
    {
        TutorialStep++;
    }

}
