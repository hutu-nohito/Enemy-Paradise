using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class QuestManager : Quest_Parameter {

	//クエストクリア・クエスト失敗
    //今んとこ敵を軟体倒す系のクエしかできない
	
	//クエストのパラメタ//////////////////////////////////////////////////////////
	public int clear_count = 3;
	
	public int now_count = 0;//使い終わったら戻す！

    //クエストの状態//////////////////////////////////////////////////////////
    /*	private　bool quest_count = false;//クエスト識別用・数
        private　bool quest_time = false;//クエスト識別用・時間
        private　bool quest_position = false;//クエスト識別用・位置*/

    public bool isQuest = false;//ただいまクエスト中 ギルドで管理

	//Script//////////////////////////////////////////////////////////
	private Static _static;
    private SceneTransition ST;
    private uGUI_Msg GUImsg;//メッセージ操作用

    //コルーチン用
    private Coroutine coroutine;
    private bool isCoroutine = false;

    //カメラ操作用
    //private Camera F_camera;

    //文字操作用
    public GameObject Ready;
    public GameObject Go;
    public GameObject Clear;

    //Player
    public GameObject Player;

    //クエストマネージャはManagerについてるのでStartは基本使わない
    // Use this for initialization
    void Start()
    {

        _static = GetComponent<Static>();
        ST = GetComponent<SceneTransition>();

    }

    //クエストスタート時///////////////////////////////////////////////////////////

    public void QuestStart()
    {
        
        coroutine = StartCoroutine(C_QuestStart());

    }
    IEnumerator C_QuestStart()
    {
        yield return new WaitForSeconds(2.1f);//シーン切り替わり待ち

        Player = GameObject.FindGameObjectWithTag("Player");//切り替わってからでないと読めない
        //F_camera = Player.transform.FindChild("FrontCamera").GetComponent<Camera>();
        _static = GetComponent<Static>();
        ST = GetComponent<SceneTransition>();
        clear_count = clear_num;

        //敵やらなんやら配置構成 全部アクティブにしておく///////////////////////////////////////////////////////////////////////////////////////////

        //eBで判断すれば、敵一体一体を識別できるはず
        //これで配列に入るらしい。順番はわからん
        Enemy_Base[] eB = GameObject.FindObjectsOfType<Enemy_Base>();
        //敵
        for (int i = 0;i < eB.Length;i++)
        {
           if(eB[i].GetQuestStage() != queststageID)
            {
                eB[i].gameObject.SetActive(false);
            }
        }


        //UI関連処理////////////////////////////////////////////////////////////////////////////////////////////////////////
        //重い
        //Ready = GameObject.Find("Text_Ready");
        //Go = GameObject.Find("Text_Go");
        //Clear = GameObject.Find("Text_Clear");

        //GameObjectはアクティブでないと探せないので探したら消す
        Ready.SetActive(false);
        Go.SetActive(false);
        Clear.SetActive(false);

        Ready.SetActive(true);
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Player.GetComponent<Player_ControllerZ>().SetKeylock();

        yield return new WaitForSeconds(3);//表示に時間がかかる可能性を考えて少したってから行動できるようにしておく

        Ready.SetActive(false);
        Go.SetActive(true);
        //Player.GetComponent<Player_ControllerZ>().SetActive();

        yield return new WaitForSeconds(2);//ちょっとしたらGoを消す
        Go.SetActive(false);

    }

    //クエスト内/////////////////////////////////////////////////////////////////////
    //ここにクエスト内でのクリア条件に関する処理を書く
    void Update()
    {

        //Count
        if (now_count >= clear_count)
        {

            Clear_Count();

        }

        //F12を押すとクエストクリア
        if (Input.GetKeyDown(KeyCode.F12))
        {
            coroutine = StartCoroutine(QuestClear());
        }
    }

    public void SetCount(string CharaName)
    {
        for(int i = 0;i < quest_Target.Length ; i++){

            if (CharaName == quest_Target[i])//ターゲットに同じ文字列が入ってるとその数分だけカウントされるので注意
            {
                now_count++;
            }

        }
        
    }

    public void SaisyuCount()//採取用のカウント
    {
        if (quest_Target[0] == "0" || quest_Target[0] == "4")//採取クエの時だけカウント
        {
            now_count++;
        }
        
    }

	//クエスト終了/////////////////////////////////////////////////////////////////

	//ここにクリアした時の処理を書く。使ったものは戻す。

    IEnumerator QuestClear(){

        if (isCoroutine) { yield break; }
        isCoroutine = true;

        //クリア後だからたぶんほっといても大丈夫
        //Player.GetComponent<Player_ControllerZ>().SetKeylock();
        //Camera.main.enabled = false;
        //F_camera.enabled = true;

		//クリア時のHP、MPを引き継がせる
		//_static.SetHP(Player.GetComponent<Player_ControllerZ> ().GetHP ());

        yield return new WaitForSeconds(0.5f);//カメラ切り替えの間

        Clear.SetActive(true);

        yield return new WaitForSeconds(3);//クリアを見せる

        Clear.SetActive(false);
        //一応戻しとく
        //Player.GetComponent<Player_ControllerZ>().SetActive();
        //Camera.main.enabled = true;//カメラは保持してないのでないと取り込めない
        //F_camera.enabled = false;

        isCoroutine = false;

        //クエストが終わったら特別なことがない限りギルドへ
        ST.Guild();

    }

	void Clear_Count () {

        now_count = 0;//使い終わったら戻す
        coroutine = StartCoroutine(QuestClear());
		
	}

    //失敗したときの処理。使ったものは戻す

    public void Questfailure()
    {

        coroutine = StartCoroutine(C_Questfailere());

    }

    IEnumerator C_Questfailere()
    {

        if (isCoroutine) { yield break; }
        isCoroutine = true;

        now_count = 0;//使い終わったら戻す

        //クリア後だからたぶんほっといても大丈夫
        Player.GetComponent<Player_ControllerZ>().SetKeylock();
        //Camera.main.enabled = false;
        //F_camera.enabled = true;

        //死んでるかもだからHPを回復させて1日たたせる
        //_static.SetHP(Player.GetComponent<Player_ControllerZ>().GetHP());
        _static.day += 0.5f;//強制的に寝たことにする
        _static.SetHP(100);

        //一応戻しとく
        Player.GetComponent<Player_ControllerZ>().SetActive();
        isCoroutine = false;

        //クエストが終わったら特別なことがない限りギルドへ
        ST.Guild();
        yield return new WaitForSeconds(0);//ないとコルーチンにできない

    }
}
