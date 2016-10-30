using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Event_Controller : MonoBehaviour {

    /******************************************************************************/
    /** @brief シーンごとにイベントの進行
    * @date 2016/08/18
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    */
    /******************************************************************************/
    /* 更新履歴
    *  裏庭のチュートリアル追加
    *  シーン遷移もここでやる
    */
    /******************************************************************************/

    private GameObject Manager;
    private Event_Manager EM;
    private SceneTransition ST;
    private Player_ControllerVR PcVR;
    private bool flag_tutorial = false;

    //チュートリアル用（突貫　後でどうにかする）
    public int TutorialStep = 100;//チュートリアルのステップ数(頑張れば使いまわせそう)
    public GameObject[] TutorialCube;//とりあえず直入れ
    
    // Use this for initialization
    void Start () {

        //シーンが変わるごとにゲームマネージャを更新
        Manager = GameObject.FindWithTag("Manager");
        EM = Manager.GetComponent<Event_Manager>();
        ST = Manager.GetComponent<SceneTransition>();
        PcVR = GameObject.FindWithTag("Player").GetComponent<Player_ControllerVR>();
        TutorialStep = 0;
        //シーン遷移時の関数の呼び出し順が難しいので気を付ける
        if (SceneManager.GetActiveScene().name == "BackyardVR")
        {
            Backyard_T();
        }
            
    }
	
	// Update is called once per frame
	void Update () {

        
    }


    public void Backyard_T()//裏庭でのイベント
    {
        flag_tutorial = true;

        //偶数番目で説明、奇数番目で実践(奇数の時はメッセージを表示しない)
        if (TutorialStep == 0)
        {
            
            PcVR.Reverse_Magic();
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[4]);//表示する

        }
        if (TutorialStep == 1)//敵を探す
        {
            PcVR.Reverse_Magic();
            TutorialCube[0].SetActive(true);

        }
        if (TutorialStep == 2)
        {
            TutorialCube[0].SetActive(false);
            PcVR.Reverse_Magic();
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[5]);//表示する
        }
        if (TutorialStep == 3)//魔法陣を出す
        {
            PcVR.Reverse_Magic();
            TutorialCube[1].SetActive(true);
        }
        if (TutorialStep == 4)
        {
            TutorialCube[1].SetActive(false);
            PcVR.Reverse_Magic();
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[6]);//表示する
        }
        if (TutorialStep == 5)//魔法を作る
        {
            PcVR.Reverse_Magic();
            TutorialCube[2].SetActive(true);
        }
        if (TutorialStep == 6)
        {
            TutorialCube[2].SetActive(false);
            PcVR.Reverse_Magic();
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[7]);//表示する
        }
        if (TutorialStep == 7)
        {
            PcVR.Reverse_Magic();
            Invoke("TutorialClear", 10);//とりあえず
        }
        if (TutorialStep == 8)
        {
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[8]);//表示する
        }
        if (TutorialStep == 9)
        {
            flag_tutorial = false;//チュートリアルを途中でやめるとバグる気がする
            ST.Guild();
        }
    }

    //チュートリアルのステップを外から進める
    public void TutorialClear()
    {
        if (flag_tutorial)
        {
            TutorialStep++;

            if (SceneManager.GetActiveScene().name == "Backyard" || SceneManager.GetActiveScene().name == "BackyardVR")
            {
                Backyard_T();
            }
        }

    }
}
