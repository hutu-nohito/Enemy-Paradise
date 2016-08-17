using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI用

public class uGUI_Msg : MonoBehaviour {

    /*
        テキストに入ってるものを表示
    */

    new private Text guiText;
    public GameObject UI_Window;//表示するWindow
    public GameObject UI_Text;//表示するテキストの入れ物

    [Range(0, 1)]
    public float Text_spead = 0.9f;//文字送りの速さ

    public TextAsset Text;
    public static TextAsset _Text;//テキストファイル

    // Use this for initialization
    void Start()
    {
        _Text = Text;
        guiText = UI_Text.GetComponent<Text>();
        readMap(_Text);
        flgDisp = flag;

    }

    //元からあったの
    public static string[] dispMsg;//テキスト1文(メッセージ)
    public static int lengthMsg;			//メッセージの何文字めか

    public bool flag = false;
    public static bool flgDisp = false;	//表示フラグ
    public static float waitTime = 0;//メッセージが消えるまでの待ち時間

    //自分でつくったの
    public static int lengthSecenario = 0;//テキスト全体(シナリオ)の何段目か

    float nextTime = 0;//次の文字の表示間隔

    public GameObject TextFinish;//テキストを表示し終わったときにちかちかするアイコン

    //テキスト読み込み。もらってきた今表示したいテキストを分割して配列に格納
    static void readMap(TextAsset readText)
    {

        _Text = readText;
        char[] kugiri = { '#' };//テキストを区切る記号設定
        //テキストを一行づつ区切る
        dispMsg = _Text.text.Split(kugiri);

    }

    // Update is called once per frame
    void Update()
    {
        
        //メセージ表示状態時のみ処理を行う
        if (flgDisp == true)
        {
            UI_Window.SetActive(true);
            //何段目の何文字目を表示するか
            guiText.text = dispMsg[lengthSecenario].Substring(0, lengthMsg);

            //nextTimeになるのを待ってから1文字増やして表示
            if (Time.time > nextTime)
            {			//次の文字の表示時間を越えたら

                if (lengthMsg < dispMsg[lengthSecenario].Length)
                {	//文字の長さが最大でなければ

                    lengthMsg++;//次の文字へ

                }

                nextTime = Time.time + (1.0f - Text_spead);	//次の文字の表示間隔
            }

            //一文の末尾まで行ったら
            if (lengthMsg >= dispMsg[lengthSecenario].Length)
            {       //メッセージを全部表示していたら 

                TextFinish.SetActive(true);

                if (Input.GetMouseButtonDown(0))
                {

                    lengthSecenario++;
                    Debug.Log(lengthSecenario);
                    lengthMsg = 0;		//0文字目にリセット
                    TextFinish.SetActive(false);

                }

                //文末まで行ったら
                if (lengthSecenario > dispMsg.Length - 1)
                {

                    flgDisp = false;		//非表示
                    UI_Window.SetActive(false);
                    lengthMsg = 0;		//0文字目にリセット
                    lengthSecenario = 0;//そのシナリオの1文目から
                    this.enabled = false;//消しとく

                }
            }
        }

    }

    //メッセージを表示したいときにはこの関数を呼び出す
    public void dispMessage(TextAsset gaveText)
    {

        readMap(gaveText);		//メッセージを代入
        lengthMsg = 0;		//0文字目にリセット
        lengthSecenario = 0;//そのシナリオの1文目から
        flgDisp = true;		//表示

    }

}
