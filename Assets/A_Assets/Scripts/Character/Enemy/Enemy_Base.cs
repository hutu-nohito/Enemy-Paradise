using UnityEngine;
using System.Collections;

public class Enemy_Base : Character_Parameters
{

    /******************************************************************************/
    /** @brief 敵基底クラス
    * @date 2016/05/08
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    * @func BaseStart 共通の初期化(敵毎のStart()の中で呼び出す)
    * @func BaseUpdate 共通の処理(敵毎のUpdate()の中で呼び出す)
    */
    /******************************************************************************/
    /* 更新履歴
    *   どの敵にも必要な初期化、判定はこっち
    *   どんな敵にでもアニメーションはついてるのもとする(ない場合はダミーつけとく)
    */
    /******************************************************************************/

    [System.NonSerialized]
    public Animator animator;

    [System.NonSerialized]
    public GameObject Player;

    // Use this for initialization
    public void BaseStart () {

        //アニメーションセット
        animator = GetComponentInChildren<Animator>();

        //プレイヤをセット
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player");
        }
	}
	
	// Update is called once per frame
	public void BaseUpdate () {

    }
}
