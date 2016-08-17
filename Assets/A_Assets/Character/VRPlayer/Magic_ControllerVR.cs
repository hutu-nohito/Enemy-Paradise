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
    */
    /******************************************************************************/

    private Player_ControllerVR Pz;

    //変数(ex:time)////////////////////////////////

    public int[] selectmagic = new int[5];//選ばれた魔法の番号

    //GameObject/////////////////////////////////////////////
    public GameObject[] Magic;//魔法の大本。PlayerのみこれをMuzzleとして使う
    public GameObject[] SelectMagic = new GameObject[5];//隙間にセットされた魔法

    //コルーチン
    private Coroutine coroutine;
    //private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
    private bool isCoroutine = false;

    //使うもの

    private Magic_Parameter.InputType InputType;

    void Awake()
    {

        Pz = GetComponent<Player_ControllerVR>();
        
        //選択されてる魔法の番号を渡す。(スキマが間に合わないのでとりあえず魔法は固定)
        MagicSet(0, 1, 2, 3, 4);
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

    void MagicSet(int a, int b, int c, int d, int e)
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

        selectmagic[0] = a;
        selectmagic[1] = b;
        selectmagic[2] = c;
        selectmagic[3] = d;
        selectmagic[4] = e;

    }

    void Update()
    {
        
        if (Pz.GetF_Magic())//魔法が打てる状態かどうかを確認
        {
            MagicFire();
        }
        
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
    }
}
