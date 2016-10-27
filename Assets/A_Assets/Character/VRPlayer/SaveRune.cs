using UnityEngine;
using System.Collections;
using System.Collections.Generic;//List用
using UnityEngine.UI;//UI用
using UnityEngine.SceneManagement;//シーン関連

public class SaveRune : MonoBehaviour
{
    /******************************************************************************/
    /** @brief パスワードシステム
    * @date 2016/10/27
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    * 
    */
    /******************************************************************************/
    /* 更新履歴
    *   魔法の決定を引用、整理
    */
    /******************************************************************************/

    private Static saveObj;//ギルドレベルを渡す

    //変数(ex:time)////////////////////////////////
    //コルーチン
    private Coroutine coroutine;
    //private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
    private bool isCoroutine = false;

    //VR
    public GameObject[] JinPoint;//魔法陣の通過点
    public GameObject[] JinUI;//魔法陣のUI
    private bool flag_Jin = false;//魔法陣受付中

    private List<int> RuneList = new List<int>();//点つなぎの番号
    //めんどくさいので魔法はここに直打ち(数は覚えとく)
    private int[] correctRuneArray = new int[20]
    {
        0,          //(0)初級2
        1,          //(1)初級3
        2,4,        //(2,3)中級1
        3,1,        //(4,5)中級2
        0,0,        //(6,7)中級3
        1,2,3,4,    //(8,11)上級1
        0,0,0,0,    //(12,15)上級2
        2,0,1,0     //(16,19)上級3
    };

    //コントローラのボタン検知
    public enum VRButton
    {
        TriggerTouchDown,
        TriggerPressDown,
        TriggerUp,
        GripDown,
        GripUp,
    }
    //おしっぱなし
    private bool TriggerTouch = false;
    private bool TriggerPress = false;
    private bool Grip = false;

    void Awake()
    {

        saveObj = GetComponent<Static>();

    }
    
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "title")//タイトルシーンだったら
        {
            MagicCircle();//魔法陣の処理
        }
        
    }

    void MagicCircle()//魔法陣の処理
    {
        if (flag_Jin)
        {
            for (int i = 0; i < JinPoint.Length; i++)
            {
                JinPoint[i].SetActive(true);
                JinUI[i].SetActive(true);
            }
            //HPgauge.SetActive(true);
        }
        else
        {
            for (int i = 0; i < JinPoint.Length; i++)
            {
                JinPoint[i].SetActive(false);
                JinUI[i].SetActive(false);
                JinUI[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.4f);//戻しとく
            }
        }

    }

    //コントローラ管理///////////////////////////////////////////////////////////
    //コントローラのボタンを押すと呼ばれる（数字がボタンの種類）
    public void ControllerPulse(VRButton kind, bool right)
    {
        switch (kind)
        {
            case VRButton.TriggerTouchDown:
                break;
            case VRButton.TriggerPressDown:
                if (RuneList.Count > 0)
                {
                    CheckRuneList();//ここでルーンが完成しているかを判定
                }
                break;
            case VRButton.TriggerUp:
                break;
            case VRButton.GripDown:
                if (RuneList.Count > 0)
                {
                    CheckRuneList();//ここでルーンが完成しているかを判定
                }
                break;
            case VRButton.GripUp:
                break;
        }
    }
    public void DotToDot(int point)
    {
        RuneList.Add(point);
    }

    //押っぱの
    public void ControllerTrigger(bool right)
    {
        if (right)
        {
            //なんか必要だったら
        }
    }

    //杖を前に掲げたら
    public void SetRod()
    {
        if (!TriggerPress)//セイバー使ってる時に暴発しないように
        {
            flag_Jin = true;
        }
    }

    public void PutRod()
    {
        //stockMagicが2以上の時は消さないとかにしてもいいかも

        flag_Jin = false;
        RuneList.Clear();//チェックしたらクリア
    }

    ////////////////////////////////////////////////////////////////////////////////////

    //配列が長い魔法からチェックする
    //配列を後ろからチェック
    void CheckRuneList()
    {
        
        if (RuneList.Count >= 4)//4個以上つなぐ
        {

            //上級3
            if (RuneList[RuneList.Count - 1] == correctRuneArray[19])
            {
                if (RuneList[RuneList.Count - 2] == correctRuneArray[18])
                {
                    if (RuneList[RuneList.Count - 3] == correctRuneArray[17])
                    {
                        if (RuneList[RuneList.Count - 4] == correctRuneArray[16])
                        {
                            saveObj.SetGL(8);
                            RuneList.Clear();//チェックしたらクリア
                            return;
                        }
                    }
                }
            }

            //上級2
            if (RuneList[RuneList.Count - 1] == correctRuneArray[15])
            {
                if (RuneList[RuneList.Count - 2] == correctRuneArray[14])
                {
                    if (RuneList[RuneList.Count - 3] == correctRuneArray[13])
                    {
                        if (RuneList[RuneList.Count - 4] == correctRuneArray[12])
                        {
                            saveObj.SetGL(7);
                            RuneList.Clear();//チェックしたらクリア
                            return;
                        }
                    }
                }
            }

            //上級1
            if (RuneList[RuneList.Count - 1] == correctRuneArray[11])
            {
                if (RuneList[RuneList.Count - 2] == correctRuneArray[10])
                {
                    if (RuneList[RuneList.Count - 3] == correctRuneArray[9])
                    {
                        if (RuneList[RuneList.Count - 4] == correctRuneArray[8])
                        {
                            saveObj.SetGL(6);
                            RuneList.Clear();//チェックしたらクリア
                            return;
                        }
                    }
                }
            }

        }
        
        if (RuneList.Count >= 2)//2個以上つなぐ
        {
            //中級3
            if (RuneList[RuneList.Count - 1] == correctRuneArray[7])
            {
                if (RuneList[RuneList.Count - 2] == correctRuneArray[6])
                {
                    saveObj.SetGL(5);
                    RuneList.Clear();//チェックしたらクリア
                    return;

                }
            }

            //中級2
            if (RuneList[RuneList.Count - 1] == correctRuneArray[5])
            {
                if (RuneList[RuneList.Count - 2] == correctRuneArray[4])
                {
                    saveObj.SetGL(4);
                    RuneList.Clear();//チェックしたらクリア
                    return;

                }
            }

            //中級1
            if (RuneList[RuneList.Count - 1] == correctRuneArray[3])
            {
                if (RuneList[RuneList.Count - 2] == correctRuneArray[2])
                {
                    saveObj.SetGL(3);
                    RuneList.Clear();//チェックしたらクリア
                    return;

                }
            }
        }

        //初級3
        if (RuneList[RuneList.Count - 1] == correctRuneArray[1])
        {
            saveObj.SetGL(2);
            RuneList.Clear();//チェックしたらクリア
            return;
        }

        //初級2
        if (RuneList[RuneList.Count - 1] == correctRuneArray[0])
        {
            saveObj.SetGL(1);
            Debug.Log("www");
            RuneList.Clear();//チェックしたらクリア
            return;
        }

        //初級1は最初から出てる

        RuneList.Clear();//チェックしたらクリア
    }

}
