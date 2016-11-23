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
        TutorialStep = 8;

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
            if (EM.uGM.lengthSecenario == 2)//文章の何段落目か
            {
                TutorialImage[1].transform.parent.gameObject.SetActive(true);// 親もつける
                TutorialImage[5].SetActive(true);//HPタッチ
            }
        }

        if (TutorialStep == 8)//魔法を放つ
        {
            if (EM.uGM.lengthSecenario == 2)//文章の何段落目か
            {
                TutorialImage[1].transform.parent.gameObject.SetActive(true);// 親もつける
                TutorialImage[6].SetActive(true);//HPタッチ
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

            yield return new WaitForSeconds(1);//文章が消えるまで

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

            yield return new WaitForSeconds(1);//文章が消えるまで

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
            yield return new WaitForSeconds(1);//文章が消えるまで

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
        if (TutorialStep == 7)//魔法をストック
        {

            yield return new WaitForSeconds(1);//文章が消えるまで

            PcVR.Reverse_Magic();
            TutorialCube[3].SetActive(true);
        }
        if (TutorialStep == 8)
        {
            //ちょっと観察できる時間を作る
            TutorialImage[5].SetActive(false);//
            TutorialImage[1].transform.parent.gameObject.SetActive(false);//親も消しとくと安心
            TutorialCube[3].SetActive(false);

            yield return new WaitForSeconds(3);

            PcVR.Reverse_Magic();
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[8]);//表示する
        }
        if (TutorialStep == 9)//魔法を撃つ
        {

            yield return new WaitForSeconds(1);//文章が消えるまで

            PcVR.Reverse_Magic();
            Invoke("TutorialClear",7);//7秒遊んでもらう
        }
        if (TutorialStep == 10)
        {
            //ちょっと観察できる時間を作る
            TutorialImage[6].SetActive(false);//
            TutorialImage[1].transform.parent.gameObject.SetActive(false);//親も消しとくと安心
            
            yield return new WaitForSeconds(3);

            PcVR.Reverse_Magic();
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[9]);//表示する
        }
        if (TutorialStep == 11)//魔法を撃つ
        {

            yield return new WaitForSeconds(1);//文章が消えるまで

            PcVR.Reverse_Magic();
            Invoke("TutorialClear", 30);//30秒遊んでもらう
        }
        if (TutorialStep == 12)
        {
            //倒せても倒せなくても進む
            PcVR.Reverse_Magic();
            EM.uGM.enabled = true;//つける
            EM.uGM.dispMessage(EM.EventText[10]);//表示する
        }

        if (TutorialStep == 13)
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
