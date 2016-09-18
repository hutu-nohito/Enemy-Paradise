using UnityEngine;
using System.Collections;
using System.Collections.Generic;//List用

public class Magic_ControllerVR : MonoBehaviour {

    /******************************************************************************/
    /** @brief VRプレイヤーの魔法管理
    * @date 2016/07/19
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    * 
    */
    /******************************************************************************/
    /* 更新履歴
    *  魔法の選択
    *  魔法の発動
    *  魔法は選択されているものを打つのでなく操作方法で使い分ける
    *  クールタイムは魔法個別で止める
    *  点つなぎ法導入
    */
    /******************************************************************************/

    private Player_ControllerVR Pz;

    //変数(ex:time)////////////////////////////////

    public int[] selectmagic = new int[6];//選ばれた魔法の番号

    //GameObject/////////////////////////////////////////////
    public GameObject[] Magic;//魔法の大本。PlayerのみこれをMuzzleとして使う
    public GameObject[] SelectMagic = new GameObject[6];//隙間にセットされた魔法

    //コルーチン
    private Coroutine coroutine;
    //private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
    private bool isCoroutine = false;

    //使うもの

    private Magic_Parameter.InputType InputType;

    //VR
    public bool flag_VR = false;
    public GameObject HandR,HandL;
    public GameObject[] JinPoint;//魔法陣の通過点
    public GameObject[] JinUI;//魔法陣のUI
    public GameObject[] MagicUI;//魔法アイコン
    private bool flag_Jin = false;//魔法陣受付中
    private List<int> MagicList = new List<int>();//点つなぎの番号
    //めんどくさいので魔法はここに直打ち(数は覚えとく)
    private int[] correctMagicArray = new int[24]
    {
        2,0,3, 1,0,4,   //(0,2 3,5)セイバー
        0,1,2,0,        //(6,9)アイシクルレイン
        0,              //(10)ウェルオーウィスプ
        0,3,4,0,2,      //(11,15)ボム
        3,1, 4,2,       //(16,17 18,19)フレイムピラー
        1,2,4,3         //(20,23)バリア
    };
    private int stockMagic = 100;//ストックする魔法(とりあえず100に戻す)
    private int MagicLevel = 1;//いったん点つなぎを切るときよう

    //入力受付
    private bool flag_Input = false;//トリガーで発動しとく
    public float inputTime = 5.0f;//入力受付時間
    private float elapsedTime = 0;

    //コントローラの動き検知（精度が良すぎて使えないので要改良）
    private List<Vector3> OldPos = new List<Vector3>();//計算用
    private List<MoveDirection> Move = new List<MoveDirection>();//コントローラの軌跡をとる
    private int MoveCount = 0;//何個Moveを拾ってるか
    private enum MoveDirection
    {
        Hold,
        Front,
        Right,
        Back,
        Left,
        Up,
        Down,
        UpRight,
        UpLeft,
        DownRight,
        DownLeft,
        FrontUp,
        FrontDown,
        FrontRight,
        FrontLeft,
        FrontUpRight,
        FrontUpLeft,
        FrontDownRight,
        FrontDownLeft,
        BackUp,
        BackDown,
        BackRight,
        BackLeft,
        BackUpRight,
        BackUpLeft,
        BackDownRight,
        BackDownLeft,
    }

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

        Pz = GetComponent<Player_ControllerVR>();
        
        //選択されてる魔法の番号を渡す。(スキマが間に合わないのでとりあえず魔法は固定)
        MagicSet(0, 1, 2, 3, 4, 5);
        /*
        MagicSet(
            _static.SelectMagicID[0],
            _static.SelectMagicID[1],
            _static.SelectMagicID[2], 
            _static.SelectMagicID[3],
            _static.SelectMagicID[4]);
        */

        for (int i = 0; i < Magic.Length; i++)
        {

            Magic[i].GetComponent<Magic_Parameter>().SetParent(this.gameObject);//親はプレイヤー

        }

    }

    void MagicSet(int a, int b, int c, int d, int e, int f)
    {
        /*
         * 
         * Magic[0]にもらったIDと同じ魔法をInstanseして格納
         * Magic[0]をPlayerの子オブジェクトに
         * インスタンスしたものにPlayerが親だと伝える
         * 
         */

        //選ばれた魔法を格納
        SelectMagic[0] = Magic[a];
        SelectMagic[1] = Magic[b];
        SelectMagic[2] = Magic[c];
        SelectMagic[3] = Magic[d];
        SelectMagic[4] = Magic[e];
        SelectMagic[5] = Magic[f];

        selectmagic[0] = a;
        selectmagic[1] = b;
        selectmagic[2] = c;
        selectmagic[3] = d;
        selectmagic[4] = e;
        selectmagic[5] = f;

    }

    void Update()
    {
        MagicCircle();//魔法陣の処理

        //入力受付
        if (flag_Input)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > inputTime)
            {
                flag_Input = false;
                elapsedTime = 0;
                MoveCount = 0;
                OldPos.Clear();
                Move.Clear();
            }
            if ((int)((elapsedTime * 100) % 30) == 0)//たぶん0.5秒ごとに入るはず
            {
                Debug.Log(elapsedTime);
                CalHandPosition();//ハンドポジを計算
                CheckMove();
                
                MoveCount++;//5秒のうちに増える
                CheckMagic();//魔法が完成してるかチェック
            }       
        }

        if (Pz.GetF_Magic())//魔法が打てる状態かどうかを確認
        {
            MagicFire();
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
        }
        else
        {
            for (int i = 0; i < JinPoint.Length; i++)
            {
                JinPoint[i].SetActive(false);
                JinUI[i].SetActive(false);
            }
        }

        if(stockMagic != 100)
        {
            MagicUI[stockMagic].SetActive(true);
        }else
        {
            for (int i = 0;i < MagicUI.Length; i++)
            {
                MagicUI[i].SetActive(false);
            }
            
        }
    }

    //コントローラの軌跡が魔法になってるかどうか
    void CheckMagic()
    {
        if(Move.Count <= 1)
        {
            return;
        }

        //ウェルオーウィスプ
        //if(Move[Move.Count - 1] == MoveDirection.Front)//腕を前に突き出す
        //{
        //    Debug.Log("前");
        //    flag_Input = false;
        //    elapsedTime = 0;
        //    MoveCount = 0;
        //    OldPos.Clear();
        //    Move.Clear();
        //    SelectMagic[2].SendMessage("Fire");
        //}
        //if (Move[Move.Count - 1] == MoveDirection.FrontUp)//腕を前に突き出す
        //{
        //    Debug.Log("上前");
        //    flag_Input = false;
        //    elapsedTime = 0;
        //    MoveCount = 0;
        //    OldPos.Clear();
        //    Move.Clear();
        //    SelectMagic[2].SendMessage("Fire");
        //}
        //if (Move[Move.Count - 1] == MoveDirection.FrontUpRight)//腕を前に突き出す
        //{
        //    Debug.Log("右上前");
        //    flag_Input = false;
        //    elapsedTime = 0;
        //    MoveCount = 0;
        //    OldPos.Clear();
        //    Move.Clear();
        //    SelectMagic[2].SendMessage("Fire");
        //}
        //if (Move[Move.Count - 1] == MoveDirection.FrontUpLeft)//腕を前に突き出す
        //{
        //    Debug.Log("左上前");
        //    flag_Input = false;
        //    elapsedTime = 0;
        //    MoveCount = 0;
        //    OldPos.Clear();
        //    Move.Clear();
        //    SelectMagic[2].SendMessage("Fire");
        //}
        //if (Move[Move.Count - 1] == MoveDirection.FrontLeft)//腕を前に突き出す
        //{
        //    Debug.Log("左前");
        //    flag_Input = false;
        //    elapsedTime = 0;
        //    MoveCount = 0;
        //    OldPos.Clear();
        //    Move.Clear();
        //    SelectMagic[2].SendMessage("Fire");
        //}
        //if (Move[Move.Count - 1] == MoveDirection.FrontRight)//腕を前に突き出す
        //{
        //    Debug.Log("右前");
        //    flag_Input = false;
        //    elapsedTime = 0;
        //    MoveCount = 0;
        //    OldPos.Clear();
        //    Move.Clear();
        //    SelectMagic[2].SendMessage("Fire");
        //}

        if (Move[Move.Count - 1] == MoveDirection.Up)//腕を前に突き出す
        {
            Debug.Log("上");
            flag_Input = false;
            elapsedTime = 0;
            MoveCount = 0;
            OldPos.Clear();
            Move.Clear();
            SelectMagic[3].SendMessage("Fire");
            if (Move[Move.Count - 1] == MoveDirection.Front)
            {
                Debug.Log("www");
                flag_Input = false;
                elapsedTime = 0;
                MoveCount = 0;
                OldPos.Clear();
                Move.Clear();
                SelectMagic[3].SendMessage("Fire");
            }
               
        }


    }

    //コントローラの軌跡を取得してとっとく
    void CheckMove()
    {
        //前///////////////////////////////////////////////////////////////////////////////////////////////
        if (OldPos[OldPos.Count - 1].z - OldPos[OldPos.Count - 2].z >= 0.3f)//前へ
        {
            if (OldPos[OldPos.Count - 1].y - OldPos[OldPos.Count - 2].y >= 0.3f)//上前
            {
                if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x >= 0.3f)//右上前
                {
                    Move.Add(MoveDirection.FrontUpRight);
                }
                else if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x > -0.3f)//上前
                {
                    Move.Add(MoveDirection.FrontUp);
                }
                else//左上前
                {
                    Move.Add(MoveDirection.FrontUpLeft);
                }
            }
            else if (OldPos[OldPos.Count - 1].y - OldPos[OldPos.Count - 2].y > -0.3f)//真ん中
            {
                if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x >= 0.3f)//右前
                {
                    Move.Add(MoveDirection.FrontRight);
                }
                else if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x > -0.3f)//前
                {
                    Move.Add(MoveDirection.Front);
                }
                else//左前
                {
                    Move.Add(MoveDirection.FrontLeft);
                }
            }
            else//下前
            {
                if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x >= 0.3f)//右下前
                {
                    Move.Add(MoveDirection.FrontDownRight);
                }
                else if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x > -0.3f)//下前
                {
                    Move.Add(MoveDirection.FrontDown);
                }
                else//左下前
                {
                    Move.Add(MoveDirection.FrontDownLeft);
                }
            }
        }
        else if (OldPos[OldPos.Count - 1].z - OldPos[OldPos.Count - 2].z >= -0.3f)//真ん中/////////////////////////////////////////////////////
        {
            if (OldPos[OldPos.Count - 1].y - OldPos[OldPos.Count - 2].y >= 0.3f)//上
            {
                if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x >= 0.3f)//右上
                {
                    Move.Add(MoveDirection.UpRight);
                }
                else if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x > -0.3f)//上
                {
                    Move.Add(MoveDirection.Up);
                }
                else//左上
                {
                    Move.Add(MoveDirection.UpLeft);
                }
            }
            else if (OldPos[OldPos.Count - 1].y - OldPos[OldPos.Count - 2].y > -0.3f)//真ん中
            {
                if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x >= 0.3f)//右
                {
                    Move.Add(MoveDirection.Right);
                }
                else if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x > -0.3f)//ホールド
                {
                    Move.Add(MoveDirection.Hold);
                }
                else//左
                {
                    Move.Add(MoveDirection.Left);
                }
            }
            else//下
            {
                if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x >= 0.3f)//右下
                {
                    Move.Add(MoveDirection.DownRight);
                }
                else if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x > -0.3f)//下
                {
                    Move.Add(MoveDirection.Down);
                }
                else//左下
                {
                    Move.Add(MoveDirection.DownLeft);
                }
            }
        }
        else//後ろ/////////////////////////////////////////////////////
        {
            if (OldPos[OldPos.Count - 1].y - OldPos[OldPos.Count - 2].y >= 0.3f)//上
            {
                if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x >= 0.3f)//右上後
                {
                    Move.Add(MoveDirection.BackUpRight);
                }
                else if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x > -0.3f)//上後
                {
                    Move.Add(MoveDirection.BackUp);
                }
                else//左上後
                {
                    Move.Add(MoveDirection.BackUpLeft);
                }
            }
            else if (OldPos[OldPos.Count - 1].y - OldPos[OldPos.Count - 2].y > -0.3f)//真ん中
            {
                if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x >= 0.3f)//右後
                {
                    Move.Add(MoveDirection.BackRight);
                }
                else if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x > -0.3f)//ホールド
                {
                    Move.Add(MoveDirection.Back);
                }
                else//左後
                {
                    Move.Add(MoveDirection.BackLeft);
                }
            }
            else//下
            {
                if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x >= 0.3f)//右下後
                {
                    Move.Add(MoveDirection.BackDownRight);
                }
                else if (OldPos[OldPos.Count - 1].x - OldPos[OldPos.Count - 2].x > -0.3f)//下後
                {
                    Move.Add(MoveDirection.BackDown);
                }
                else//左下後
                {
                    Move.Add(MoveDirection.BackDownLeft);
                }
            }
        }
    }

    void CalHandPosition()//手の位置を計算
    {
        //コントローラとカメラの位置関係で何が発動するか決まる
        var HandRPos = HandR.transform.position - Camera.main.transform.position;//右手の相対位置
        Vector3 calHandRPos = Vector3.zero;//カメラから見た右手の位置

        //x,zはカメラの向きによって入れ替える
        int thetaDegree = (int)(Camera.main.transform.eulerAngles.y);
        if (thetaDegree <= 45 || thetaDegree > 315)//正面
        {
            calHandRPos.x = HandRPos.x;
            calHandRPos.z = HandRPos.z;
        }
        if (thetaDegree > 45 && thetaDegree <= 135)//右
        {
            calHandRPos.x = -HandRPos.z;
            calHandRPos.z = HandRPos.x;
        }
        if (thetaDegree > 135 && thetaDegree <= 225)//後ろ
        {
            calHandRPos.x = -HandRPos.x;
            calHandRPos.z = -HandRPos.z;
        }
        if (thetaDegree > 225 && thetaDegree <= 315)//左
        {
            calHandRPos.x = HandRPos.z;
            calHandRPos.z = -HandRPos.x;
        }
        calHandRPos.y = HandRPos.y;

        OldPos.Add(calHandRPos);
        Debug.Log(OldPos[OldPos.Count - 1]);

    }

    //トリガーを深く引くと呼ばれる（左右区別なし）
    public void PressTrigger()
    {
        if (!flag_Input)
        {
            flag_Input = true;//なんか押されたらチェック
            CalHandPosition();
        }
        
        Debug.Log("トリガーを深く引いた");
    }

    public void UpTrigger()
    {
        if (!flag_Input)
        {
            //flag_Input = true;//なんか押されたらチェック
        }

        //Debug.Log("トリガーを離した");
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
                if (right)//右トリガーで魔法発動
                {
                    if(stockMagic == 0)//セイバー
                    {
                        //こっちは送らないようにする
                    }
                    else if(stockMagic == 3)//ボム
                    {
                        SelectMagic[stockMagic].SendMessage("Fire");
                    }
                    else if(stockMagic != 100)//それ以外
                    {
                        SelectMagic[stockMagic].SendMessage("Fire");
                        stockMagic = 100;
                    }
                }
                if (!right)//左トリガーでレベルアップ
                {
                    //次のステップ
                }
                break;
            case VRButton.TriggerUp:
                if (stockMagic == 0)//セイバー
                {
                    //剣を壊す
                    SelectMagic[stockMagic].SendMessage("Break");
                    stockMagic = 100;
                }
                else if (stockMagic == 3)//ボム
                {
                    //ボムを投げる
                    SelectMagic[stockMagic].SendMessage("Throw");
                    stockMagic = 100;
                }
                else if (stockMagic != 100)//それ以外
                {
                    SelectMagic[stockMagic].SendMessage("Fire");
                    stockMagic = 100;
                }
                break;
            case VRButton.GripDown:
                if (!right)//左グリップで魔法陣発動
                {
                    if (!TriggerPress)//セイバー使ってる時に暴発しないように
                    {
                        flag_Jin = true;
                    }
                    
                }
                break;
            case VRButton.GripUp:
                if (!right)//左グリップ離して魔法陣しまう
                {
                    flag_Jin = false;
                    if(MagicList.Count > 0)
                    {
                        CheckMagicList();//ここで魔法が完成しているかを判定
                    }
                    
                }
                break;
        }
    }

    public void ControllerTrigger(bool right)
    {
        if (right)
        {
            if (stockMagic == 0)//セイバー
            {
                SelectMagic[stockMagic].SendMessage("Hold");
                //セイバーが壊れた時にstockmagicを100にする処理を入れる
            }
            if (stockMagic == 3)
            {
                SelectMagic[stockMagic].SendMessage("Hold");
                //ボムを離した時にstockmagicを100にする処理を入れる
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////

    public void DotToDot(int point)
    {
        MagicList.Add(point);
    }

    //配列が長い魔法からチェックする
    //配列を後ろからチェック
    void CheckMagicList()
    {
        //セイバー
        //if (MagicList[MagicList.Count - 1] == correctMagicArray[2])
        //{
        //    if (MagicList[MagicList.Count - 2] == correctMagicArray[1])
        //    {
        //        if (MagicList[MagicList.Count - 3] == correctMagicArray[0])
        //        {
        //            //レベル2へ（保留）
        //        }
        //    }
        //}
        //if(MagicLevel == 2)
        //{
        //    //(保留)
        //}

        //ボム
        if (MagicList[MagicList.Count - 1] == correctMagicArray[15])
        {
            if (MagicList[MagicList.Count - 2] == correctMagicArray[14])
            {
                if (MagicList[MagicList.Count - 3] == correctMagicArray[13])
                {
                    if (MagicList[MagicList.Count - 4] == correctMagicArray[12])
                    {
                        if (MagicList[MagicList.Count - 5] == correctMagicArray[11])
                        {
                            stockMagic = 3;
                            MagicList.Clear();//チェックしたらクリア
                            return;
                        }
                    }
                }
            }
        }

        //アイシクルレイン
        if (MagicList[MagicList.Count - 1] == correctMagicArray[9])
        {
            if (MagicList[MagicList.Count - 2] == correctMagicArray[8])
            {
                if (MagicList[MagicList.Count - 3] == correctMagicArray[7])
                {
                    if (MagicList[MagicList.Count - 4] == correctMagicArray[6])
                    {
                        stockMagic = 1;
                        MagicList.Clear();//チェックしたらクリア
                        return;
                    }
                }
            }
        }

        //フレイムピラー
        //if (MagicList[MagicList.Count - 1] == correctMagicArray[9])
        //{
        //    if (MagicList[MagicList.Count - 2] == correctMagicArray[8])
        //    {
        //        if (MagicList[MagicList.Count - 3] == correctMagicArray[7])
        //        {
        //            if (MagicList[MagicList.Count - 4] == correctMagicArray[6])
        //            {

        //            }
        //        }
        //    }
        //}

        //バリア
        if (MagicList[MagicList.Count - 1] == correctMagicArray[23])
        {
            if (MagicList[MagicList.Count - 2] == correctMagicArray[22])
            {
                if (MagicList[MagicList.Count - 3] == correctMagicArray[21])
                {
                    if (MagicList[MagicList.Count - 4] == correctMagicArray[20])
                    {
                        stockMagic = 5;
                        MagicList.Clear();//チェックしたらクリア
                        return;
                    }
                }
            }
        }

        //ウェルオーウィスプ
        if (MagicList[MagicList.Count - 1] == correctMagicArray[0])
        {
            stockMagic = 2;
            MagicList.Clear();//チェックしたらクリア
            return;
        }

        MagicList.Clear();//チェックしたらクリア
    }

    void MagicFire()
    {
        //ボタン毎に魔法を発動
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            //アイスセイバー
            SelectMagic[0].SendMessage("Fire");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //アイシクルレイン
            SelectMagic[1].SendMessage("Fire");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //ウェルオーウィスプ
            SelectMagic[2].SendMessage("Fire");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //ボム
            SelectMagic[3].SendMessage("Fire");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //ピラー
            SelectMagic[4].SendMessage("Fire");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            //バリア
            SelectMagic[5].SendMessage("Fire");
        }
    }

}
