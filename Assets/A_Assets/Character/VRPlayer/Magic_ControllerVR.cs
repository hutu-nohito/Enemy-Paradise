﻿using UnityEngine;
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

    //入力受付
    private bool flag_Input = false;//トリガーで発動しとく
    public float inputTime = 5.0f;//入力受付時間
    private float elapsedTime;

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
        //入力受付
        if (flag_Input)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > inputTime)
            {
                flag_Input = false;
                elapsedTime = 0;
                OldPos.Clear();
                Move.Clear();
            }
            if ((int)(elapsedTime % 0.5f) == 0)//たぶん0.5秒ごとに入るはず
            {
                CheckMove();
                OldPos.Add(transform.position);//0.5秒ごとに更新
                MoveCount++;//5秒のうちに増える
                CheckMagic();//魔法が完成してるかチェック
            }       
        }

        if (Pz.GetF_Magic())//魔法が打てる状態かどうかを確認
        {
            MagicFire();
        }
        
    }
    
    //コントローラの軌跡が魔法になってるかどうか
    void CheckMagic()
    {
        //ウェルオーウィスプ
        if(Move[MoveCount - 1] == MoveDirection.Front)//腕を前に突き出す
        {
            Debug.Log("www");
        }
    }

    //コントローラの軌跡を取得してとっとく
    void CheckMove()
    {
        //前///////////////////////////////////////////////////////////////////////////////////////////////
        if (transform.position.z - OldPos[OldPos.Count - 1].z >= 0.3f)//前へ
        {
            if (transform.position.y - OldPos[OldPos.Count - 1].y >= 0.3f)//上前
            {
                if (transform.position.x - OldPos[OldPos.Count - 1].x >= 0.3f)//右上前
                {
                    Move[MoveCount] = MoveDirection.FrontUpRight;
                }
                else if (transform.position.x - OldPos[OldPos.Count - 1].x > -0.3f)//上前
                {
                    Move[MoveCount] = MoveDirection.FrontUp;
                }
                else//左上前
                {
                    Move[MoveCount] = MoveDirection.FrontUpLeft;
                }
            }
            else if (transform.position.y - OldPos[OldPos.Count - 1].y > -0.3f)//真ん中
            {
                if (transform.position.x - OldPos[OldPos.Count - 1].x >= 0.3f)//右前
                {
                    Move[MoveCount] = MoveDirection.FrontRight;
                }
                else if (transform.position.x - OldPos[OldPos.Count - 1].x > -0.3f)//前
                {
                    Move[MoveCount] = MoveDirection.Front;
                }
                else//左前
                {
                    Move[MoveCount] = MoveDirection.FrontLeft;
                }
            }
            else//下前
            {
                if (transform.position.x - OldPos[OldPos.Count - 1].x >= 0.3f)//右下前
                {
                    Move[MoveCount] = MoveDirection.FrontDownRight;
                }
                else if (transform.position.x - OldPos[OldPos.Count - 1].x > -0.3f)//下前
                {
                    Move[MoveCount] = MoveDirection.FrontDown;
                }
                else//左下前
                {
                    Move[MoveCount] = MoveDirection.FrontDownLeft;
                }
            }
        }
        else if (transform.position.z - OldPos[OldPos.Count - 1].z >= -0.3f)//真ん中/////////////////////////////////////////////////////
        {
            if (transform.position.y - OldPos[OldPos.Count - 1].y >= 0.3f)//上
            {
                if (transform.position.x - OldPos[OldPos.Count - 1].x >= 0.3f)//右上
                {
                    Move[MoveCount] = MoveDirection.UpRight;
                }
                else if (transform.position.x - OldPos[OldPos.Count - 1].x > -0.3f)//上
                {
                    Move[MoveCount] = MoveDirection.Up;
                }
                else//左上
                {
                    Move[MoveCount] = MoveDirection.UpLeft;
                }
            }
            else if (transform.position.y - OldPos[OldPos.Count - 1].y > -0.3f)//真ん中
            {
                if (transform.position.x - OldPos[OldPos.Count - 1].x >= 0.3f)//右
                {
                    Move[MoveCount] = MoveDirection.Right;
                }
                else if (transform.position.x - OldPos[OldPos.Count - 1].x > -0.3f)//ホールド
                {
                    Move[MoveCount] = MoveDirection.Hold;
                }
                else//左
                {
                    Move[MoveCount] = MoveDirection.Left;
                }
            }
            else//下
            {
                if (transform.position.x - OldPos[OldPos.Count - 1].x >= 0.3f)//右下
                {
                    Move[MoveCount] = MoveDirection.DownRight;
                }
                else if (transform.position.x - OldPos[OldPos.Count - 1].x > -0.3f)//下
                {
                    Move[MoveCount] = MoveDirection.Down;
                }
                else//左下
                {
                    Move[MoveCount] = MoveDirection.DownLeft;
                }
            }
        }
        else//後ろ/////////////////////////////////////////////////////
        {
            if (transform.position.y - OldPos[OldPos.Count - 1].y >= 0.3f)//上
            {
                if (transform.position.x - OldPos[OldPos.Count - 1].x >= 0.3f)//右上後
                {
                    Move[MoveCount] = MoveDirection.BackUpRight;
                }
                else if (transform.position.x - OldPos[OldPos.Count - 1].x > -0.3f)//上後
                {
                    Move[MoveCount] = MoveDirection.BackUp;
                }
                else//左上後
                {
                    Move[MoveCount] = MoveDirection.BackUpLeft;
                }
            }
            else if (transform.position.y - OldPos[OldPos.Count - 1].y > -0.3f)//真ん中
            {
                if (transform.position.x - OldPos[OldPos.Count - 1].x >= 0.3f)//右後
                {
                    Move[MoveCount] = MoveDirection.BackRight;
                }
                else if (transform.position.x - OldPos[OldPos.Count - 1].x > -0.3f)//ホールド
                {
                    Move[MoveCount] = MoveDirection.Back;
                }
                else//左後
                {
                    Move[MoveCount] = MoveDirection.BackLeft;
                }
            }
            else//下
            {
                if (transform.position.x - OldPos[OldPos.Count - 1].x >= 0.3f)//右下後
                {
                    Move[MoveCount] = MoveDirection.BackDownRight;
                }
                else if (transform.position.x - OldPos[OldPos.Count - 1].x > -0.3f)//下後
                {
                    Move[MoveCount] = MoveDirection.BackDown;
                }
                else//左下後
                {
                    Move[MoveCount] = MoveDirection.BackDownLeft;
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
        //flag_Input = true;//なんか押されたらチェック
    }

    //トリガーを深く引くと呼ばれる（左右区別なし）
    public void PressTrigger()
    {
        //CalHandPosition();
        Debug.Log("トリガーを深く引いた");
    }

    public void UpTrigger()
    {
        //CalHandPosition();
        Debug.Log("トリガーを離した");
    }

    void MagicFire()
    {
        //ボタン毎に魔法を発動
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SelectMagic[0].SendMessage("Fire");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectMagic[1].SendMessage("Fire");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectMagic[2].SendMessage("Fire");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectMagic[3].SendMessage("Fire");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectMagic[4].SendMessage("Fire");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectMagic[5].SendMessage("Fire");
        }
    }

}
