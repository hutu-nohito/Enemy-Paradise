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
    */
    /******************************************************************************/

    private GameObject Manager;
    private Event_Manager EM;

    //チュートリアル用（突貫　後でどうにかする）
    public int TutorialStep = 0;//チュートリアルのステップ数(頑張れば使いまわせそう)
    public GameObject[] TutorialCube;//とりあえず直入れ
    
    // Use this for initialization
    void Start () {

        //シーンが変わるごとにゲームマネージャを更新
        Manager = GameObject.FindWithTag("Manager");
        EM = Manager.GetComponent<Event_Manager>();
	}
	
	// Update is called once per frame
	void Update () {

        
    }


    public void Backyard_T()//裏庭でのイベント
    {

        //偶数番目で説明、奇数番目で実践(奇数の時はメッセージを表示しない)
        if (TutorialStep == 0)
        {
            
            //pcVR.SetKeylock();
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[4]);//表示する

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
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[5]);//表示する
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
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[6]);//表示する
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
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[7]);//表示する
        }
        if (TutorialStep == 7)
        {
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[8]);//表示する
            //pcVR.SetActive();
        }
    }

    //チュートリアルのステップを外から進める
    public void TutorialClear()
    {
        TutorialStep++;

        if (SceneManager.GetActiveScene().name == "Backyard" || SceneManager.GetActiveScene().name == "BackyardVR")
        {
            Backyard_T();
        }

    }
}
