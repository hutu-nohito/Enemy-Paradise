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
    *  イベントをシーンごとに分割、E_contと連携
    */
    /******************************************************************************/

    //特別なイベントがある場合にこれが呼び出される(Manager)
    //ついでにインフォメーションのON/OFF管理

    public TextAsset[] EventText;//ここにイベント用のテキストを格納
    private bool[] EventFlag = new bool[100];//イベントフラグ、使ったらおろしてく

    /*
        0:ホーム一日目
        1:ギルド一日目
        2:チュートリアル
    */

	public GameObject Canvas;//UI
    public GameObject Information;//情報表示

    [System.NonSerialized]
    public uGUI_Msg uGM;//呼び出し用にpublic
    //uGMのlengthSecenarioが文章の何段目かを示す

    public GameObject[] YesNo;
    public Event_Controller EC;//Event_Controller

    private Static save;//日数、起動回数、HP、MP、名声、ボーナスポイント
    private SceneTransition ST;
    private Player_ControllerVR pcVR;

	//コルーチン
	private Coroutine coroutine;
	private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
	private bool isCoroutineH = false;
	private bool isCoroutineG = false;
    private bool olduGM = false;//メッセージウィンドウを開いていたかどうか

    void Awake()
    {
        uGM = Canvas.GetComponent<uGUI_Msg>();
        uGM.enabled = false;

    }
    // Use this for initialization
    void Start() {

        save = GetComponent<Static>();
        ST = GetComponent<SceneTransition>();
        if (Canvas == null)
        {
            Canvas = GameObject.Find("Msg_Canvas");
        }
        if (EC == null)
        {
            EC = GameObject.Find("Event_Controller").GetComponent<Event_Controller>();
        }

        //これはシーンが変わるごとに必要(とりあえずプレイヤーの動きはとめない)
        //pcVR = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerVR>();

        for (int i = 0; i < EventFlag.Length; i++)
        {
            EventFlag[i] = false;//初期化　イベントをセーブするようになったらその時考える
        }


        //デバッグ用
        if (SceneManager.GetActiveScene().name == "Backyard" || SceneManager.GetActiveScene().name == "BackyardVR")
        {

            //EC.Backyard_T();
        }


    }

    // Update is called once per frame
    void Update () {

        if (olduGM)
        {
            //メッセージが非表示になった瞬間
            if (olduGM != uGM.enabled)
            {
                //チュートリアル以外でも使えるようにする
                EC.TutorialClear();
            }
        }

        olduGM = uGM.enabled;

        //後で移動(テキスト名とかとるべき？)
        if (SceneManager.GetActiveScene().name == "Guild")
        {
            if (!EventFlag[2])
            {
                if (uGM.lengthSecenario == 5)//文章の何段落目か
                {
                    YesNo[0].SetActive(true);//Yesボタン
                    YesNo[1].SetActive(true);//Noボタン
                    EventFlag[2] = true;
                }
            }
        }
        
    }

    //シーンが変わったらイベントチェック
    void Check_Event()
    {
        if(SceneManager.GetActiveScene().name == "Home")
        {
            //StartCoroutine(Home_T());
        }


        if (SceneManager.GetActiveScene().name == "Guild")
        {
            StartCoroutine(guild_T());
        }

        if (SceneManager.GetActiveScene().name == "title")
        {
            //Information.SetActive(false);
        }
        if(SceneManager.GetActiveScene().name == "BackyardVR")
        {
            
            //チュートリアル
            //EC.Backyard_T();
        }
        else
        {
            //Information.SetActive(true);/消しとく
        }

        if (SceneManager.GetActiveScene().name == "lilith2 battle field")
        {
            StartCoroutine(battle_T());
        }

        //イベントコントローラをそのシーンのものに更新
        EC = GameObject.Find("Event_Controller").GetComponent<Event_Controller>();

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
        if (EventFlag[3])
        {
            uGM.enabled = true;//つける
            uGM.dispMessage(EventText[11]);//表示する
            EventFlag[3] = false;
        }
        if (EventFlag[4])
        {
            uGM.enabled = true;//つける
            uGM.dispMessage(EventText[12]);//表示する
            EventFlag[4] = false;
        }
        if (EventFlag[5])
        {
            uGM.enabled = true;//つける
            uGM.dispMessage(EventText[13]);//表示する
            EventFlag[5] = false;
        }

        isCoroutineG = false;
		
	}

    IEnumerator battle_T()
    {//コロシアムでのイベント

        
        yield return new WaitForSeconds(1);//

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

    //管理するフラグと立てるか下ろすかを選択
    public void EventFlagSet(int num ,bool flag)
    {
        EventFlag[num] = flag;
    }

}
