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
    public GameObject[] TutorialImage;//後でもっとうまい方法を考える

    //登場シーン用（後で何とかする）
    public GameObject AnimCamera;//とりあえず映す
    public GameObject AnimMoji;
    public GameObject AnimHaniwonder;
    public GameObject AnimGolem;//今はダミー

    // Use this for initialization
    void Start () {

        //シーンが変わるごとにゲームマネージャを更新
        Manager = GameObject.FindWithTag("Manager");
        EM = Manager.GetComponent<Event_Manager>();
        ST = Manager.GetComponent<SceneTransition>();
        PcVR = GameObject.FindWithTag("Player").GetComponent<Player_ControllerVR>();
        TutorialStep = 0;

        //これはそのうちEMから呼び出せるようにしたい
        //シーン遷移時の関数の呼び出し順が難しいので気を付ける
        if (SceneManager.GetActiveScene().name == "BackyardVR")
        {
            StartCoroutine(Backyard_T());
        }

        //めんどくさいからとりあえずなんか入れとけ
        //if(TutorialCube[0] == null)
        //{

        //}
            
    }
	
	// Update is called once per frame
	void Update () {
        if(TutorialStep == 2)//魔法陣を出す
        {
            if (EM.uGM.lengthSecenario == 3)//文章の何段落目か
            {
                TutorialImage[0].SetActive(true);//下げ
                Invoke("Age",0.5f);//後で何とかする
            }
        }

        if (TutorialStep == 4)//魔法陣発動
        {
            if (EM.uGM.lengthSecenario == 5)//文章の何段落目か
            {
                TutorialImage[1].transform.parent.gameObject.SetActive(true);// 親もつける
                TutorialImage[2].SetActive(true);//触らん
                Invoke("Age2", 0.5f);//後で何とかする
            }
        }

        if (TutorialStep == 6)//魔法をストック
        {
            if (EM.uGM.lengthSecenario == 1)//文章の何段落目か
            {
                TutorialImage[1].transform.parent.gameObject.SetActive(true);// 親もつける
                TutorialImage[3].SetActive(true);//グリップ
            }
        }


    }

    //後で何とかする
    void Age()
    {
        TutorialImage[1].SetActive(true);//上げ
    }

    void Age2()
    {
        TutorialImage[3].SetActive(true);//赤触る
    }


    IEnumerator Backyard_T()//裏庭でのイベント
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
            //ちょっと観察できる時間を作る
            TutorialImage[0].SetActive(false);//
            TutorialImage[1].SetActive(false);//
            TutorialImage[1].transform.parent.gameObject.SetActive(false);//親も消しとくと安心
            TutorialCube[1].SetActive(false);

            yield return new WaitForSeconds(3);

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
            //ちょっと観察できる時間を作る
            TutorialImage[2].SetActive(false);//
            TutorialImage[3].SetActive(false);//
            TutorialImage[1].transform.parent.gameObject.SetActive(false);//親も消しとくと安心
            TutorialCube[2].SetActive(false);

            yield return new WaitForSeconds(3);

            PcVR.Reverse_Magic();
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[7]);//表示する
        }
        if (TutorialStep == 7)
        {
            TutorialImage[3].SetActive(false);//グリップ
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
                StartCoroutine(Backyard_T());
            }
        }

    }
}
