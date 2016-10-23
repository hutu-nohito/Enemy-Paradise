//using UnityEngine;
//using System.Collections;

//public class SaveRune : MonoBehaviour {
//    /******************************************************************************/
//    /** @brief VRプレイヤーの魔法管理
//    * @date 2016/07/19
//    * @author 石川
//    * @param[in] m_fringe 干渉縞の計算結果を格納
//    * 
//    */
//    /******************************************************************************/
//    /* 更新履歴
//    *  魔法の選択
//    *  魔法の発動
//    *  魔法は選択されているものを打つのでなく操作方法で使い分ける
//    *  クールタイムは魔法個別で止める
//    *  点つなぎ法導入
//    *  UIもこっちでできるように
//    */
//    /******************************************************************************/
    
//    private Player_ControllerVR Pz;

//    //変数(ex:time)////////////////////////////////

//    public int[] selectmagic = new int[6];//選ばれた魔法の番号

//    //GameObject/////////////////////////////////////////////
//    public GameObject[] Magic;//魔法の大本。PlayerのみこれをMuzzleとして使う
//    public GameObject[] SelectMagic = new GameObject[6];//隙間にセットされた魔法

//    //コルーチン
//    private Coroutine coroutine;
//    //private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
//    private bool isCoroutine = false;

//    //使うもの

//    private Magic_Parameter.InputType InputType;

//    //VR
//    public bool flag_VR = false;
//    public GameObject HandR, HandL;
//    public GameObject[] JinPoint;//魔法陣の通過点
//    public GameObject[] JinUI;//魔法陣のUI
//    public Sprite[] MagicTexture;//
//    public GameObject[] MagicIcon;//魔法のアイコン
//    //public GameObject HPgauge;//体力ゲージ
//    private bool flag_Jin = false;//魔法陣受付中

//    private List<int> MagicList = new List<int>();//点つなぎの番号
//    //めんどくさいので魔法はここに直打ち(数は覚えとく)
//    private int[] correctMagicArray = new int[19]
//    {
//        //1,0,4,2,0,3,   //(0,2 3,5)セイバー
//        //0,1,2,0,        //(6,9)アイシクルレイン
//        //0,              //(10)ウェルオーウィスプ
//        //0,3,4,0,2,      //(11,15)ボム
//        //3,1,4,2,       //(16,17 18,19)フレイムピラー
//        //1,2,4,3         //(20,23)バリア

//        //3,              //(0)バブルダンス
//        3,0,2,          //(1,3)セイバー
//        0,1,2,0,        //(4,7)レイン
//        2,              //(8)フレイム
//        0,3,4,0,2,      //(9,13)ボム
//        4,2,            //(14,15)ピラー
//        //1,              //(16)ブーメラン
//        1,2,4,3         //(17,20)バリア

//    };
//    private List<int> stockMagic = new List<int>();//ストックする魔法(何も入ってないとき先頭は100)(特に何もなければ先頭を使う)(今は3つまで！)
//    public int Max_stockMagic = 3;
//    private int MagicLevel = 1;//いったん点つなぎを切るときよう
    
//    //コントローラのボタン検知
//    public enum VRButton
//    {
//        TriggerTouchDown,
//        TriggerPressDown,
//        TriggerUp,
//        GripDown,
//        GripUp,
//    }
//    //おしっぱなし
//    private bool TriggerTouch = false;
//    private bool TriggerPress = false;
//    private bool Grip = false;

//    void Awake()
//    {

//        Pz = GetComponent<Player_ControllerVR>();

//        //選択されてる魔法の番号を渡す。(スキマが間に合わないのでとりあえず魔法は固定)
//        MagicSet(0, 1, 2, 3, 4, 5/*, 6, 7*/);
//        /*
//        MagicSet(
//            _static.SelectMagicID[0],
//            _static.SelectMagicID[1],
//            _static.SelectMagicID[2], 
//            _static.SelectMagicID[3],
//            _static.SelectMagicID[4]);
//        */

//        for (int i = 0; i < Magic.Length; i++)
//        {

//            Magic[i].GetComponent<Magic_Parameter>().SetParent(this.gameObject);//親はプレイヤー

//        }

//        stockMagic.Add(100);

//    }

//    void MagicSet(int a, int b, int c, int d, int e, int f /*,int g, int h*/)
//    {
//        /*
//         * 
//         * Magic[0]にもらったIDと同じ魔法をInstanseして格納
//         * Magic[0]をPlayerの子オブジェクトに
//         * インスタンスしたものにPlayerが親だと伝える
//         * 
//         */

//        //選ばれた魔法を格納
//        SelectMagic[0] = Magic[a];
//        SelectMagic[1] = Magic[b];
//        SelectMagic[2] = Magic[c];
//        SelectMagic[3] = Magic[d];
//        SelectMagic[4] = Magic[e];
//        SelectMagic[5] = Magic[f];
//        /*
//        SelectMagic[6] = Magic[g];
//        SelectMagic[7] = Magic[h];
//        */

//        selectmagic[0] = a;
//        selectmagic[1] = b;
//        selectmagic[2] = c;
//        selectmagic[3] = d;
//        selectmagic[4] = e;
//        selectmagic[5] = f;
//        /*
//        selectmagic[6] = g;
//        selectmagic[7] = h;
//        */

//    }

//    //魔法を増やすとき
//    void AddStockMagic(int addMagic)
//    {
//        if (stockMagic[0] == 100)//ストックのない状態
//        {
//            stockMagic[0] = addMagic;
//        }
//        else if (stockMagic.Count < Max_stockMagic)
//        {
//            stockMagic.Add(addMagic);//末尾につけ足し
//        }
//    }

//    //魔法を減らすとき
//    void DisplaceStockMagic()
//    {
//        if (stockMagic.Count > 1)//魔法が2つ以上ある
//        {
//            for (int i = 0; i < stockMagic.Count - 1; i++)
//            {
//                stockMagic[i] = stockMagic[i + 1];//要素を詰める(先頭からやれば消えない)
//            }
//            stockMagic.RemoveAt(stockMagic.Count - 1);//末尾を消す
//        }
//        else//ストックに魔法が1つの場合
//        {
//            stockMagic[0] = 100;
//        }
//    }

//    void Update()
//    {
//        MagicCircle();//魔法陣の処理
//    }

//    void MagicCircle()//魔法陣の処理
//    {
//        if (flag_Jin)
//        {
//            for (int i = 0; i < JinPoint.Length; i++)
//            {
//                JinPoint[i].SetActive(true);
//                JinUI[i].SetActive(true);
//            }
//            //HPgauge.SetActive(true);
//        }
//        else
//        {
//            for (int i = 0; i < JinPoint.Length; i++)
//            {
//                JinPoint[i].SetActive(false);
//                JinUI[i].SetActive(false);
//                JinUI[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.4f);//戻しとく
//            }
//            //HPgauge.SetActive(false);
//        }
        
//    }
    
//    //トリガーを深く引くと呼ばれる（左右区別なし）
//    public void PressTrigger()
//    {
//        if (!flag_Input)
//        {
//            flag_Input = true;//なんか押されたらチェック
//            CalHandPosition();
//        }

//        Debug.Log("トリガーを深く引いた");
//    }

//    public void UpTrigger()
//    {
//        if (!flag_Input)
//        {
//            //flag_Input = true;//なんか押されたらチェック
//        }

//        //Debug.Log("トリガーを離した");
//    }

//    //コントローラ管理///////////////////////////////////////////////////////////
//    //コントローラのボタンを押すと呼ばれる（数字がボタンの種類）
//    public void ControllerPulse(VRButton kind, bool right)
//    {
//        switch (kind)
//        {
//            case VRButton.TriggerTouchDown:
//                break;
//            case VRButton.TriggerPressDown:
//                if (right)//右トリガーで魔法発動
//                {
//                    /*
//                    if(stockMagic[0] == 1)//セイバー
//                    {
//                        SelectMagic[stockMagic[0]].SendMessage("Fire");
//                        break;
//                    }
//                    if(stockMagic[0] == 4)//ボム
//                    {
//                        SelectMagic[stockMagic[0]].SendMessage("Fire");
//                        break;
//                    }
//                    if (stockMagic[0] == 6)//ブーメラン
//                    {
//                        SelectMagic[stockMagic[0]].SendMessage("Fire");
//                        break;
//                    }
//                    */
//                    if (stockMagic[0] == 0)//セイバー
//                    {
//                        SelectMagic[stockMagic[0]].SendMessage("Fire");
//                        break;
//                    }
//                    if (stockMagic[0] == 3)//ボム
//                    {
//                        SelectMagic[stockMagic[0]].SendMessage("Fire");
//                        break;
//                    }
//                    if (stockMagic[0] != 100)//それ以外
//                    {
//                        SelectMagic[stockMagic[0]].SendMessage("Fire");
//                        //DisplaceStockMagic();                        
//                    }
//                }
//                if (!right)//左トリガーでレベルアップ
//                {
//                    //次のステップ
//                }
//                break;
//            case VRButton.TriggerUp:
//                if (right)
//                {
//                    /*
//                    if (stockMagic[0] == 0)//バブル
//                    {
//                        //使用済み
//                        DisplaceStockMagic();
//                        break;
//                    }
//                    if (stockMagic[0] == 1)//セイバー
//                    {
//                        //剣を壊す
//                        SelectMagic[stockMagic[0]].SendMessage("Break");
//                        DisplaceStockMagic();
//                        Debug.Log("www");
//                        break;
//                    }
//                    if (stockMagic[0] == 4)//ボム
//                    {
//                        //ボムを投げる
//                        SelectMagic[stockMagic[0]].SendMessage("Throw");
//                        DisplaceStockMagic();
//                        break;
//                    }
//                    if (stockMagic[0] == 6)//ブーメラン
//                    {
//                        //投げる
//                        SelectMagic[stockMagic[0]].SendMessage("Throw");
//                        DisplaceStockMagic();
//                        break;
//                    }
//                    */
//                    if (stockMagic[0] == 0)//セイバー
//                    {
//                        //剣を壊す
//                        SelectMagic[stockMagic[0]].SendMessage("Break");
//                        DisplaceStockMagic();
//                        break;
//                    }
//                    if (stockMagic[0] == 3)//ボム
//                    {
//                        //ボムを投げる
//                        SelectMagic[stockMagic[0]].SendMessage("Throw");
//                        DisplaceStockMagic();
//                        break;
//                    }
//                    if (stockMagic[0] != 100)//それ以外
//                    {
//                        //SelectMagic[stockMagic[0]].SendMessage("Fire");
//                        DisplaceStockMagic();
//                    }
//                }
//                break;
//            case VRButton.GripDown:
//                if (!right)//左グリップで魔法陣発動
//                {
//                    //if (!TriggerPress)//セイバー使ってる時に暴発しないように
//                    //{
//                    //    flag_Jin = true;
//                    //}
//                    //flag_Jin = false;
//                    if (MagicList.Count > 0)
//                    {
//                        CheckMagicList();//ここで魔法が完成しているかを判定
//                    }

//                }
//                break;
//            case VRButton.GripUp:
//                if (!right)//左グリップ離して魔法陣しまう
//                {
//                    //flag_Jin = false;
//                    //if(MagicList.Count > 0)
//                    //{
//                    //    CheckMagicList();//ここで魔法が完成しているかを判定
//                    //}

//                }
//                break;
//        }
//    }

//    //押っぱの
//    public void ControllerTrigger(bool right)
//    {
//        if (right)
//        {
//            /*
//            if (stockMagic[0] == 0)//バブル
//            {
//                SelectMagic[stockMagic[0]].SendMessage("Hold");
//                //離した時にstockmagicを100にする処理を入れる
//            }
//            if (stockMagic[0] == 1)//セイバー
//            {
                
//                SelectMagic[stockMagic[0]].SendMessage("Hold");
//                //セイバーが壊れた時にstockmagicを100にする処理を入れる
//            }
//            if (stockMagic[0] == 4)//ボム
//            {
//                SelectMagic[stockMagic[0]].SendMessage("Hold");
//                //ボムを離した時にstockmagicを100にする処理を入れる
//            }
//            if (stockMagic[0] == 6)//ブーメラン
//            {
//                SelectMagic[stockMagic[0]].SendMessage("Hold");
//                //離した時にstockmagicを100にする処理を入れる
//            }
//            */
//            if (stockMagic[0] == 0)//セイバー
//            {

//                SelectMagic[stockMagic[0]].SendMessage("Hold");
//                //セイバーが壊れた時にstockmagicを100にする処理を入れる
//            }
//            if (stockMagic[0] == 3)//ボム
//            {
//                SelectMagic[stockMagic[0]].SendMessage("Hold");
//                //ボムを離した時にstockmagicを100にする処理を入れる
//            }
//        }
//    }

//    //杖を前に掲げたら
//    public void SetRod()
//    {
//        if (!TriggerPress)//セイバー使ってる時に暴発しないように
//        {
//            flag_Jin = true;
//        }
//    }

//    public void PutRod()
//    {
//        //stockMagicが2以上の時は消さないとかにしてもいいかも

//        flag_Jin = false;
//        MagicList.Clear();//チェックしたらクリア
//    }

//    ////////////////////////////////////////////////////////////////////////////////////

//    public void DotToDot(int point)
//    {
//        MagicList.Add(point);
//    }

//    //配列が長い魔法からチェックする
//    //配列を後ろからチェック
//    void CheckMagicList()
//    {
//        //セイバー
//        //if (MagicList[MagicList.Count - 1] == correctMagicArray[2])
//        //{
//        //    if (MagicList[MagicList.Count - 2] == correctMagicArray[1])
//        //    {
//        //        if (MagicList[MagicList.Count - 3] == correctMagicArray[0])
//        //        {
//        //            //レベル2へ（保留）
//        //        }
//        //    }
//        //}
//        //if(MagicLevel == 2)
//        //{
//        //    //(保留)
//        //}

//        if (MagicList.Count >= 5)//5個以上つなぐ
//        {
//            ////セイバー
//            //if (MagicList[MagicList.Count - 1] == correctMagicArray[5])
//            //{
//            //    if (MagicList[MagicList.Count - 2] == correctMagicArray[4])
//            //    {
//            //        if (MagicList[MagicList.Count - 3] == correctMagicArray[3])
//            //        {
//            //            if (MagicList[MagicList.Count - 4] == correctMagicArray[2])
//            //            {
//            //                if (MagicList[MagicList.Count - 5] == correctMagicArray[1])
//            //                {
//            //                    if (MagicList[MagicList.Count - 6] == correctMagicArray[0])
//            //                    {
//            //                        AddStockMagic(0);
//            //                        MagicList.Clear();//チェックしたらクリア
//            //                        return;
//            //                    }
//            //                }
//            //            }
//            //        }
//            //    }
//            //}

//            //ボム
//            if (MagicList[MagicList.Count - 1] == correctMagicArray[12])
//            {
//                if (MagicList[MagicList.Count - 2] == correctMagicArray[11])
//                {
//                    if (MagicList[MagicList.Count - 3] == correctMagicArray[10])
//                    {
//                        if (MagicList[MagicList.Count - 4] == correctMagicArray[9])
//                        {
//                            if (MagicList[MagicList.Count - 5] == correctMagicArray[8])
//                            {
//                                AddStockMagic(3);
//                                MagicList.Clear();//チェックしたらクリア
//                                return;
//                            }
//                        }
//                    }
//                }
//            }

//        }

//        if (MagicList.Count >= 4)//4個以上つなぐ
//        {

//            //アイシクルレイン
//            if (MagicList[MagicList.Count - 1] == correctMagicArray[6])
//            {
//                if (MagicList[MagicList.Count - 2] == correctMagicArray[5])
//                {
//                    if (MagicList[MagicList.Count - 3] == correctMagicArray[4])
//                    {
//                        if (MagicList[MagicList.Count - 4] == correctMagicArray[3])
//                        {
//                            AddStockMagic(1);
//                            MagicList.Clear();//チェックしたらクリア
//                            return;
//                        }
//                    }
//                }
//            }

//            //フレイムピラー
//            //if (MagicList[MagicList.Count - 1] == correctMagicArray[19])
//            //{
//            //    if (MagicList[MagicList.Count - 2] == correctMagicArray[18])
//            //    {
//            //        if (MagicList[MagicList.Count - 3] == correctMagicArray[17])
//            //        {
//            //            if (MagicList[MagicList.Count - 4] == correctMagicArray[16])
//            //            {
//            //                AddStockMagic(4);
//            //                MagicList.Clear();//チェックしたらクリア
//            //                return;
//            //            }
//            //        }
//            //    }
//            //}

//            //バリア
//            if (MagicList[MagicList.Count - 1] == correctMagicArray[18])
//            {
//                if (MagicList[MagicList.Count - 2] == correctMagicArray[17])
//                {
//                    if (MagicList[MagicList.Count - 3] == correctMagicArray[16])
//                    {
//                        if (MagicList[MagicList.Count - 4] == correctMagicArray[15])
//                        {
//                            AddStockMagic(5);
//                            MagicList.Clear();//チェックしたらクリア
//                            return;
//                        }
//                    }
//                }
//            }

//        }

//        if (MagicList.Count >= 3)//3個以上つなぐ
//        {
//            //セイバー
//            if (MagicList[MagicList.Count - 1] == correctMagicArray[2])
//            {
//                if (MagicList[MagicList.Count - 2] == correctMagicArray[1])
//                {
//                    if (MagicList[MagicList.Count - 3] == correctMagicArray[0])
//                    {
//                        AddStockMagic(0);
//                        MagicList.Clear();//チェックしたらクリア
//                        return;

//                    }
//                }
//            }

//        }

//        if (MagicList.Count >= 2)//2個以上つなぐ
//        {
//            //ピラー
//            if (MagicList[MagicList.Count - 1] == correctMagicArray[14])
//            {
//                if (MagicList[MagicList.Count - 2] == correctMagicArray[13])
//                {
//                    AddStockMagic(4);
//                    MagicList.Clear();//チェックしたらクリア
//                    return;

//                }
//            }
//        }

//        /*
//        //バブルダンス
//        if (MagicList[MagicList.Count - 1] == correctMagicArray[0])
//        {
//            AddStockMagic(0);
//            MagicList.Clear();//チェックしたらクリア
//            return;
//        }
//        */

//        //ウェルオーウィスプ
//        if (MagicList[MagicList.Count - 1] == correctMagicArray[7])
//        {
//            AddStockMagic(2);
//            MagicList.Clear();//チェックしたらクリア
//            return;
//        }

//        /*
//        //ブーメラン
//        if (MagicList[MagicList.Count - 1] == correctMagicArray[16])
//        {
//            AddStockMagic(6);
//            MagicList.Clear();//チェックしたらクリア
//            return;
//        }
//        */

//        MagicList.Clear();//チェックしたらクリア
//    }
    
//}
