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

    [System.NonSerialized]
    public Vector3 Old_position;//計測用の1フレーム前の位置

    // Use this for initialization
    public void BaseStart () {

        //アニメーションセット
        animator = GetComponentInChildren<Animator>();

        //プレイヤをセット
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player");
        }

        //初期位置
        home_position = transform.position;//初期位置を
        Old_position = home_position;

	}
	
	// Update is called once per frame
	public void BaseUpdate () {

        //現在向いている方向をVector3で保持
        SetDirection(transform.TransformDirection(Vector3.forward).normalized);

        //進行方向を取る
        if (transform.position != Old_position)//位置が変化していたら
        {
            SetMove((transform.position - Old_position).normalized);//進行方向の向きベクトルを渡す
        }

        Old_position = transform.position;//OldPosを更新しないと動きません
    }
}
