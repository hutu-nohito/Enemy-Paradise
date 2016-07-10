using UnityEngine;
using System.Collections;
using System.Collections.Generic;//List用

public class FilterWall : MonoBehaviour {


    /******************************************************************************/
    /** @brief 特定のタグのオブジェクトだけ通す壁
    * @date 2016/06/29
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    */
    /******************************************************************************/
    /* 更新履歴
    *   敵や弾はいちいち探してると重そうなので、生成したらこれに送るようにする
    *   そのうちゲームマネージャで管理するようにする
    */
    /******************************************************************************/

    public bool ThroughEnemy = true;//敵貫通
    public bool ThroughBullet = true;//弾貫通
    public bool ThroughPlayer = true;//自分貫通

    private GameObject Player;
    private GameObject[] EnemyBox;
    private List<GameObject> Enemy = new List<GameObject>();//
    private GameObject[] BulletBox;
    private List<GameObject> Bullet = new List<GameObject>();//

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        //受け渡し
        EnemyBox = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < EnemyBox.Length; i++)
        {
            Enemy.Add(EnemyBox[i]);
        }

        //受け渡し
        BulletBox = GameObject.FindGameObjectsWithTag("Bullet");
        for (int i = 0; i < BulletBox.Length; i++)
        {
            Bullet.Add(BulletBox[i]);
        }

        if (ThroughPlayer)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), Player.GetComponent<CharacterController>(), true);//２つのコライダのあたり判定をなくす
        }
        if (ThroughEnemy)
        {
            for(int i = 0; i < Enemy.Count; i++)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), Enemy[i].GetComponent<Collider>(), true);//２つのコライダのあたり判定をなくす
            }
        }
        if (ThroughBullet)
        {
            for (int i = 0; i < Bullet.Count; i++)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), Bullet[i].GetComponent<Collider>(), true);//２つのコライダのあたり判定をなくす
            }
        }

    }
}
