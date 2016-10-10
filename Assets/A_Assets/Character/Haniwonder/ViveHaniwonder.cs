﻿using UnityEngine;
using System.Collections;

public class ViveHaniwonder : Enemy_Base
{

    /******************************************************************************/
    /** @brief 埴輪AI Vive用
    * @date 2016/06/29
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    */
    /******************************************************************************/
    /* 更新履歴
    *   パターンで動く
    *   熱血を先に作る
    */
    /******************************************************************************/

    /*
     * level1 はにわんだー
     * level2 はにまんさー
     * level3 熱血はにわんだー
     */

    private AudioSource SE;//音
    public AudioClip[] cv;//黒ちゃんの声
    
    //弾
    public GameObject[] Bullet;//攻撃
    public Transform[] Muzzle;//攻撃が出てくる場所
    public GameObject Avatar;//分身

    //キャラクタの状態
    public enum ActionState
    {
        Stop,//アニメーションが終わるまで待機(状態でなくアニメーションの整合性のために必要)
        Guard,//回避行動全般
        Idle,
        Run,//純粋に走ってる状態？
        Tackle,//体当たり
        Beam,//ビーム
        Headbutt,//頭突き
        Counter,//攻撃に対してカウンターしてくる
        Exercise,//体操
        AfterImage,//影分身して突進
        Knockback//

    }//intにすれば優先度にできる
    public ActionState state = ActionState.Stop;
    public ActionState GetState() { return state; }
    public void SetState(ActionState state) { this.state = state; }

    public GameObject TacleCol;//攻撃判定

    // Use this for initialization
    void Start()
    {
        base.BaseStart();

        //初期状態セット
        coroutine = StartCoroutine(ChangeState(15.0f, ActionState.Idle));

        SE = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        base.BaseUpdate();//基本機能

        //状態以外で変化するもの(今んとこ移植で補う)/////////////////////////////////////////////////////////

        //行動を止める
        if (!GetF_Move())
        {
            //コルーチンは最後まで流さないと状態がおかしくなるかも(ムービーとかでは素直に時間止めたほうがいいかも)
            //StopAllCoroutines();//コルーチンはどうやって止めたらいいんだろう
            return;//これで行動不能になる(アニメーションは止まらない)
        }

        //HPがなくなった時の処理
        if (GetHP() <= 0)
        {
            if (!flag_die)
            {
                StopAllCoroutines();//コルーチンはどうやって止めたらいいんだろう
                state = ActionState.Stop;
                animState = (int)ActionState.Stop;
                flag_die = true;
                base.animator.SetTrigger("Die");
                Destroy(this.gameObject, 7);//とりあえず消す
                SE.PlayOneShot(cv[1]);//SE
                //flag_fade = true;
                state = ActionState.Stop;
                animState = (int)ActionState.Stop;
                
            }
            

        }

        //相手がいなくなった時の処理
        if (base.Player.GetComponent<Character_Parameters>().GetHP() <= 0)
        {
            //StopAllCoroutines();
            if (!flag_win)//一回だけの処理
            {
                StopAllCoroutines();//コルーチンはどうやって止めたらいいんだろう
                flag_win = true;
                state = ActionState.Stop;
                animState = (int)ActionState.Run;
                ReverseAfterImage();//残像
                if (level == 1)//はにわんだー
                {
                    base.animator.SetTrigger("Run");
                }
                if (level == 3)//熱血
                {
                    base.animator.SetTrigger("Dance");
                }
            }

            if(level == 1)//はにわんだー
            {
                //勝利のポーズ(相手の周りを走り回る)
                transform.position = base.AffineRot(transform.position, 2000);
                //前を向ける(進行方向)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(GetMove()), 0.5f);
            }
            
        }

        //空中判定
        RaycastHit groundHit;
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, 2))
        {

            if (groundHit.distance > 0.5f)//長さで見る
            {
                flag_ground = false;
            }

        }

        //状態遷移//////////////////////////////////////////////////////////////////////////////////

        //何もしない
        if (state == ActionState.Stop)
        {
            //アニメーションの最中など動かしたくないときに用いる
        }

        if (state == ActionState.Idle)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(base.Player.transform.position - transform.position), 0.5f);
            if (animState != (int)ActionState.Idle)
            {
                base.animator.SetTrigger("Idle");
            }

            //たまーにとげ攻撃
            if ((int)Time.time % 7 == 0)//7秒ごと
            {
                float randAt1 = Random.value;
                float randAt2 = Random.value;

                if (randAt1 > 0.9)
                {
                    if (randAt2 < 0.3)
                    {
                        state = ActionState.Tackle;
                    }
                }
            }
        }

        //カウンター
        if (state == ActionState.Counter)
        {
            coroutine = StartCoroutine(Counter());
        }

        //アタックするだけ
        if (state == ActionState.Tackle)
        {
            coroutine = StartCoroutine(Tackle());
        }

        //ビーム
        if (state == ActionState.Beam)
        {
            coroutine = StartCoroutine(Beam());
        }

        //体操
        if (state == ActionState.Exercise)
        {

            if (animState != (int)ActionState.Exercise)
            {
                base.animator.SetTrigger("Exercise");
            }
            
        }

        //パターン//////////////////////////////////////////////

        if(state == ActionState.AfterImage)
        {
            
            coroutine = StartCoroutine(AvatarAttack());
        }

    }

    /////////////////////////////////////
    //少し待ってから状態を遷移する(移せなかった)
    IEnumerator ChangeState(float waitTime, ActionState nextState)
    {
        yield return new WaitForSeconds(waitTime);

        state = nextState;

    }

    //イベントが起きた時/////////////////////

    //汎用
    private Coroutine coroutine;//一度に動かすコルーチンは1つ ここでとっとけば止めるのが楽
    private bool isCoroutine = false;//コルーチンを止めるときにはfalseに戻すこと

    //体当たり
    IEnumerator Tackle()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        base.animator.SetTrigger("Yell");

        yield return new WaitForSeconds(2.0f);

        base.animator.SetTrigger("Run");
        TacleCol.SetActive(true);

        //プレイヤに突進
        ReverseAfterImage();//残像
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (Player.transform.position - transform.position).normalized.x * 30,//定数が突進距離
                "z", transform.position.z + (Player.transform.position - transform.position).normalized.z * 30,//定数が突進距離
                "time", 3.0f,
                "easetype", iTween.EaseType.easeInOutBack)

                );

        yield return new WaitForSeconds(0.1f);

        //前を向ける
        iTween.RotateTo(this.gameObject, iTween.Hash(
                "y", Mathf.Repeat(Quaternion.LookRotation(-GetMove()).eulerAngles.y, 360.0f),//(たまにおかしくなるので後で検証)
                "time", 0.25f,
                "easetype", iTween.EaseType.linear)

                );

        yield return new WaitForSeconds(4);
        ReverseAfterImage();//残像

        TacleCol.SetActive(false);
        state = ActionState.Idle;
        isCoroutine = false;
    }

    //ビーム
    IEnumerator Beam()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        //前を向ける
        iTween.RotateTo(this.gameObject, iTween.Hash(
                "y", Mathf.Repeat(Quaternion.LookRotation(Player.transform.position - Muzzle[0].transform.position).eulerAngles.y, 360.0f),//(たまにおかしくなるので後で検証),
                "time", 0.2f,
                "easetype", iTween.EaseType.linear)

                );

        yield return new WaitForSeconds(0.25f);//

        SE.PlayOneShot(cv[1]);//SE

        Bullet[0].SetActive(true);

        Bullet[0].transform.Rotate(-Mathf.Atan((Player.transform.position.y - transform.position.y) / (Player.transform.position - transform.position).magnitude) * (180 / Mathf.PI), 0, 0);

        //効果音と演出
        SE.PlayOneShot(cv[0]);//SE

        //yield return new WaitForSeconds(0.7f);//撃った後の硬直
        yield return new WaitForSeconds(2.0f);//撃った後の硬直

        Bullet[0].SetActive(false);
        Bullet[0].transform.rotation = new Quaternion(0, 0, 0, 0);

        //state = ActionState.Exercise;
        state = ActionState.Beam;
        isCoroutine = false;
    }

    //カウンター
    IEnumerator Counter()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        base.animator.SetTrigger("Run");
        TacleCol.SetActive(true);

        float randomdist = Random.Range(-3.0f, 3.0f);

        //ジグザグ突進
        ReverseAfterImage();//残像
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (Player.transform.position.x - transform.position.x + randomdist) * randomdist * 0.8f,
                "z", transform.position.z + (Player.transform.position.z - transform.position.z - randomdist) * randomdist * 0.8f,//定数が突進距離
                "time", Mathf.Abs(randomdist / 3),
                "easetype", iTween.EaseType.linear)

                );


        yield return new WaitForSeconds(0.1f);

        //前を向ける
        iTween.RotateTo(this.gameObject, iTween.Hash(
                "y", Mathf.Repeat(Quaternion.LookRotation(GetMove()).eulerAngles.y, 360.0f),//(たまにおかしくなるので後で検証)
                "time", 0.25f,
                "easetype", iTween.EaseType.linear)

                );

        yield return new WaitForSeconds(Mathf.Abs(randomdist / 3));

        ReverseAfterImage();//残像

        //iTween.MoveTo(this.gameObject, iTween.Hash(
        //        "x", transform.position.x + (Player.transform.position.x - transform.position.x - 5) * (1 - randomdist) * 2,
        //        "z", transform.position.z + (Player.transform.position.z - transform.position.z - 5) * randomdist * 2,//定数が突進距離
        //        "time", (1 - randomdist),
        //        "easetype", iTween.EaseType.easeOutBack)

        //        );


        //yield return new WaitForSeconds(0.1f);

        ////前を向ける
        //iTween.RotateTo(this.gameObject, iTween.Hash(
        //        "y", Mathf.Repeat(Quaternion.LookRotation(-GetMove()).eulerAngles.y, 360.0f),//(たまにおかしくなるので後で検証)
        //        "time", 0.25f,
        //        "easetype", iTween.EaseType.linear)

        //        );

        //yield return new WaitForSeconds(1 - randomdist);

        TacleCol.SetActive(false);
        state = ActionState.Idle;
        isCoroutine = false;
    }

    //影分身突進
    //分身はBullet扱い
    IEnumerator AvatarAttack()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        GameObject[] Avatars = new GameObject[5];

        float Angle = GetAngleP();
        if(GetPositionP().z < 0)
        {
            Angle = -Angle;
        }
        //分身を4体出す
        for(int i = 0; i < Avatars.Length / 2; i++)//半分より前
        {
            //分身ははじめから走ってる
            Avatars[i] = GameObject.Instantiate(Avatar);
            //Avatars[i].GetComponentInChildren<Haniwonder>().animator.SetTrigger("Run");
            //Avatars[i].GetComponentInChildren<Haniwonder>().TacleCol.SetActive(true);
            Avatars[i].transform.position = new Vector3(
                Player.transform.position.x + GetDistanceP() * Mathf.Cos(Angle + ((i - 2) * 30 * Mathf.PI / 180)),
                transform.position.y,
                Player.transform.position.z + GetDistanceP() * (Mathf.Sin(Angle + ((i - 2) * 30 * Mathf.PI / 180)))
                );
            //Avatars[i].transform.position = AffineRot(Avatars[i].transform.position);//playerとの位置関係で変換
                Avatars[i].transform.LookAt(Player.transform.position);
            Avatars[i].GetComponentInChildren<Attack_Parameter>().SetParent(this.gameObject);//親を設定
        }
        for (int i = Avatars.Length / 2; i < Avatars.Length - 1; i++)//半分より後
        {
            //分身ははじめから走ってる
            Avatars[i] = GameObject.Instantiate(Avatar);
            //Avatars[i].GetComponentInChildren<Haniwonder>().animator.SetTrigger("Run");
            //Avatars[i].GetComponentInChildren<Haniwonder>().TacleCol.SetActive(true);
            Avatars[i].transform.position = new Vector3(
                Player.transform.position.x + GetDistanceP() * Mathf.Cos(Angle + ((i - 1) * 30 * Mathf.PI / 180)),
                transform.position.y,
                Player.transform.position.z + GetDistanceP() * (Mathf.Sin(Angle + ((i - 1) * 30 * Mathf.PI / 180)))
                );
            //Avatars[i].transform.position = AffineRot(Avatars[i].transform.position);//playerとの位置関係で変換
            Avatars[i].transform.LookAt(Player.transform.position);
            Avatars[i].GetComponentInChildren<Attack_Parameter>().SetParent(this.gameObject);//親を設定
        }

        Avatars[4] = this.gameObject;//５番目が自分自身

        transform.LookAt(Player.transform.position);//方向のみを合わせたいならXとYを0に
        base.animator.SetTrigger("Run");
        TacleCol.SetActive(true);

        int random;
        int[] number = new int[5];//入れ替え用
        //まず0～4の配列を作る
        for (int i = 0; i < number.Length; i++)
        {
            number[i] = i;
        }
        //シャッフル
        for (int i = 0; i < number.Length; i++)
        {
            random = Random.Range(0, 5);
            int temp = number[i];
            number[i] = number[random];
            number[random] = temp;
        }

        yield return new WaitForSeconds(3);

        for (int i = 0; i < 5; i++)
        {         
            
            //プレイヤに突進
            iTween.MoveTo(Avatars[number[i]], iTween.Hash(
                    "x", Avatars[number[i]].transform.position.x + (Player.transform.position - Avatars[number[i]].transform.position).normalized.x * 30,//定数が突進距離
                    "z", Avatars[number[i]].transform.position.z + (Player.transform.position - Avatars[number[i]].transform.position).normalized.z * 30,//定数が突進距離
                    "time", 1.5f,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            yield return new WaitForSeconds(0.1f);

            //前を向ける
            if (number[i] != 4)
            {
                Avatars[number[i]].transform.LookAt(Player.transform.position);
            }
            else
            {
                ReverseAfterImage();//残像
                iTween.RotateTo(Avatars[number[i]], iTween.Hash(
                    "y", Mathf.Repeat(Quaternion.LookRotation(-GetMove()).eulerAngles.y, 360.0f),//(たまにおかしくなるので後で検証)
                    "time", 0.25f,
                    "easetype", iTween.EaseType.linear)

                    );
                Invoke("ReverseAfterImage", 1.5f);//残像が消せるかな？

            }

            //本体じゃなかったら突進後消す
            if (number[i] != 4)
            {
                Destroy(Avatars[number[i]], 1.5f);
            }

            yield return new WaitForSeconds(1);

        }

        TacleCol.SetActive(false);

        yield return new WaitForSeconds(3);

        state = ActionState.Idle;

        isCoroutine = false;
    }

    IEnumerator Knockback()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        if (animState != (int)ActionState.Knockback)
        {
            base.animator.SetTrigger("Knockback");
        }

        //プレイヤから遠ざかる方向
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x - (Player.transform.position - transform.position).normalized.x * 2,//定数が突進距離
                "z", transform.position.z - (Player.transform.position - transform.position).normalized.z * 2,//定数が突進距離
                "time", 1.0f,
                "easetype", iTween.EaseType.easeOutBack)

                );

        yield return new WaitForSeconds(1.0f);//

        base.animator.SetTrigger("Yell");

        yield return new WaitForSeconds(1.5f);//攻撃後のため

        state = ActionState.Idle;
        isCoroutine = false;
    }

    public void Damage()
    {
        //イベント側で優先度を確認すればよい
        if (GetHP() >= 0)
        {
            if (state > ActionState.Guard)//体操より優先順位が下
            {
                state = ActionState.Knockback;

                //いろいろ消さないと・・・(後処理を別でできるといいかも)
                StopAllCoroutines();//コルーチンはどうやって止めたらいいんだろう
                TacleCol.SetActive(false);
                isCoroutine = false;
                Bullet[0].SetActive(false);
                Bullet[0].transform.rotation = new Quaternion(0, 0, 0, 0);

                StartCoroutine(Knockback());
            }
        }
    }

    //接地判定欲しい
    void OnCollisionEnter()
    {
        RaycastHit groundHit;
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, 0.2f))
        {
            flag_ground = true;
        }
    }

    //アニメーション///////////////////////////////////////////////////////
    private int animState = 0;//アニメータのパラメタが取得できないのでとりあえずこれで代用

    public void AnimIdle()
    {
        animState = (int)ActionState.Idle;
    }

    public void AnimExercise()
    {
        animState = (int)ActionState.Exercise;
    }

    public void AnimRun()
    {
        animState = (int)ActionState.Run;
    }

    public void AnimHeadbutt()
    {
        animState = (int)ActionState.Headbutt;
    }

    public void AnimBeam()
    {
        animState = (int)ActionState.Beam;
    }

    public void AnimAvatar()
    {
        animState = (int)ActionState.AfterImage;
    }

    public void AnimGuard()
    {
        animState = (int)ActionState.Guard;
    }

    public void AnimKnockback()
    {
        animState = (int)ActionState.Knockback;
    }

}
