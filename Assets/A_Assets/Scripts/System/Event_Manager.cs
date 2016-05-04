using UnityEngine;
using System.Collections;

public class Event_Manager : MonoBehaviour {

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
    public uGUI_Msg uGM;

    private Static save;//日数、起動回数、HP、MP、名声、ボーナスポイント
    private SceneManager SM;

	//コルーチン
	private Coroutine coroutine;
	private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
	private bool isCoroutineH = false;
	private bool isCoroutineG = false;

	// Use this for initialization
	void Start () {
	
        save = GetComponent<Static>();
        SM = GetComponent<SceneManager>();
		Canvas = GameObject.Find ("Msg_Canvas");

        uGM = Canvas.GetComponent<uGUI_Msg>();
        uGM.enabled = false;//消しとく

        for(int i = 0;i < EventFlag.Length; i++)
        {
            EventFlag[i] = false;//初期化　イベントをセーブするようになったらその時考える
        }

	}
	
	// Update is called once per frame
	void Update () {

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

}
