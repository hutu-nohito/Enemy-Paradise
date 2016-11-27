using UnityEngine;
using System.Collections;

public class ViveGolem : Enemy_Base {


    /******************************************************************************/
    /** @brief ゴーレム　VR用AI
    * @date 2016/08/25
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    */
    /******************************************************************************/
    /* 更新履歴
    *   とげ攻撃、ハンマー攻撃、ジャンプ追加
    */
    /******************************************************************************/

    //キャラクタの状態
    public enum ActionState
    {
        Stop,//アニメーションが終わるまで待機(状態でなくアニメーションの整合性のために必要)
        HammerAttack,//ハンマー攻撃
        Idle,//待機
        Fight,//臨戦態勢
        SpikeAttack,//とげ攻撃
        HammerRot,//回転攻撃
        Jump,//跳ぶらしい
        Knockback,//必要なのか？

    }//intにすれば優先度にできる
    public ActionState state = ActionState.Stop;
    public ActionState GetState() { return state; }
    public void SetState(ActionState state) { this.state = state; }

    public GameObject[] AttackCol;//攻撃判定
    public Transform[] JumpPoint;//とりあえず決まったとこにジャンプ

    //演出
    //private Camera_ControllerZ CCZ;
    public GameObject diedGolem;//やられたとき

    private AudioSource Audio_Source;
    public AudioClip[] SE;

    /*
     * 0:ずっ・・・
     * 1:ドン
     * 
     */

    // Use this for initialization
    void Start()
    {
        base.BaseStart();

        Audio_Source = GetComponent<AudioSource>();

        //CCZ = Camera.main.gameObject.GetComponent<Camera_ControllerZ>();

        //初期状態セット
        coroutine = StartCoroutine(ChangeState(7.5f, ActionState.Fight));

        //SE = GetComponent<AudioSource>();

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
            StopAllCoroutines();//コルーチンを止める
            return;//これで行動不能になる(アニメーションは止まらない)
        }

        //HPがなくなった時の処理
        if (base.GetHP() <= 0)
        {
            if(flag_die == false)
            {
                Manager.GetComponent<QuestManager>().MonsterCount();
                flag_die = true;
                diedGolem.SetActive(true);
                for (int i = 0; i < Skins.Length; i++)
                {
                    Skins[i].enabled = false;
                }
                //flag_fade = true;
                state = ActionState.Stop;
                animState = (int)ActionState.Stop;
                StopAllCoroutines();//コルーチンを止める
                //Destroy(this.gameObject, 10);//とりあえず消す
            }
        }

        //相手がいなくなった時の処理
        if (base.Player.GetComponent<Character_Parameters>().GetHP() <= 0)
        {
            //StopAllCoroutines();
            state = ActionState.Stop;
            animState = (int)ActionState.Stop;
            ////勝利のポーズ
            //base.animator.SetTrigger("Win");
            //transform.Rotate(0, 10, 0);//とりあえず回転させとく
            
            //何もしない
        
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

        //待機
        if (state == ActionState.Idle)
        {
            if (animState != (int)ActionState.Idle)
            {
                base.animator.SetTrigger("Idle");
            }
        }

        //とげ攻撃
        if (state == ActionState.SpikeAttack)
        {
            coroutine = StartCoroutine(SpikeAttack());
        }

        //ハンマー攻撃
        if (state == ActionState.HammerAttack)
        {
            coroutine = StartCoroutine(HammerAttack());
        }

        //回転攻撃
        if (state == ActionState.HammerRot)
        {
            coroutine = StartCoroutine(HammerRot());
        }

        //ジャーんぷ
        if (state == ActionState.Jump)
        {
            coroutine = StartCoroutine(Jump());
        }

        if (state == ActionState.Fight)
        {
            //前を向ける
            //とりあえず向けとく
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(base.Player.transform.position - transform.position), 0.05f);

            //とりあえず追跡
            iTween.MoveUpdate(this.gameObject, iTween.Hash(
                "position", new Vector3(transform.position.x + (base.Player.transform.position - transform.position).normalized.x,
                                        transform.position.y,
                                        transform.position.z + (base.Player.transform.position - transform.position).normalized.z),
                "time", 10 / speed)
                );

            if (animState != (int)ActionState.Fight)
            {
                base.animator.SetTrigger("Walk");
            }


            if (GetDistanceP() < 10)
            {
                //たまーにとげ攻撃
                if ((int)Time.time % 10 == 0)//10秒ごと
                {
                    float randAt1 = Random.value;
                    float randAt2 = Random.value;

                    if (randAt1 > 0.9)
                    {
                        if (randAt2 < 0.3)
                        {
                            state = ActionState.SpikeAttack;
                        }
                    }
                }
            }
            //近づいたらハンマー攻撃
            if (GetDistanceP() < 5)
            {
                float randAt1 = Random.value;
                float randAt2 = Random.value;

                if (randAt1 < 0.7)
                {
                    state = ActionState.HammerAttack;
                }
                else
                {
                    state = ActionState.HammerRot;
                }                
            }

        }

        //パターン//////////////////////////////////////////////

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

    //とげ攻撃
    IEnumerator SpikeAttack()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        var currentPos = transform.position;
        base.animator.SetTrigger("SpikeAttack");
        //前を向ける
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(base.Player.transform.position - transform.position), 0.05f);
        //transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        transform.LookAt(Player.transform);

        //プレイヤに突進
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (Player.transform.position - transform.position).normalized.x * (GetDistanceP() - 3),//定数が突進距離
                "z", transform.position.z + (Player.transform.position - transform.position).normalized.z * (GetDistanceP() - 3),//定数が突進距離
                "time", 0.5f,
                "easetype", iTween.EaseType.easeInBack)

                );

        yield return new WaitForSeconds(0.5f);//予備動作

        AttackCol[0].SetActive(true);

        yield return new WaitForSeconds(0.5f);//攻撃後のため

        //元の位置へ
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (currentPos - transform.position).x,//定数が突進距離
                "z", transform.position.z + (currentPos - transform.position).z,//定数が突進距離
                "time", 0.5f,
                "easetype", iTween.EaseType.easeInBack)

                );

        yield return new WaitForSeconds(1.0f);//攻撃後のため

        AttackCol[0].SetActive(false);
        state = ActionState.Fight;
        isCoroutine = false;
    }

    //ハンマー攻撃
    IEnumerator HammerAttack()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        base.animator.SetTrigger("HammerAttack");
        //前を向ける
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(base.Player.transform.position - transform.position), 0.5f);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        base.animator.speed = 0.2f;//振り上げ
        yield return new WaitForSeconds(2.5f);//予備動作

        AttackCol[1].tag = ("Bullet");//攻撃にする
        AttackCol[1].GetComponents<CapsuleCollider>()[1].enabled = true;//攻撃判定
        AttackCol[1].GetComponents<CapsuleCollider>()[0].enabled = false;//接触判定

        //base.animator.speed = 1.5f;//振り降ろし
        base.animator.speed = 3.0f;//振り降ろし
        yield return new WaitForSeconds(0.1f);//たたいた瞬間
        base.animator.speed = 1.0f;//振り降ろし
        Audio_Source.PlayOneShot(SE[1]);//ドン

        //CCZ.flag_quake = true;//Viveのカメラを揺らすのは難しいので別の方法を考える
        //Player.GetComponent<Player_ControllerVR>().SetKeylock();

        yield return new WaitForSeconds(0.3f);//揺れが収まる
        
        //CCZ.flag_quake = false;

        yield return new WaitForSeconds(0.5f);//攻撃後のため

        Player.GetComponent<Player_ControllerVR>().SetActive();
        AttackCol[1].tag = ("Untagged");//攻撃でなくす
        AttackCol[1].GetComponents<CapsuleCollider>()[1].enabled = false;//攻撃判定なくす
        AttackCol[1].GetComponent<CapsuleCollider>().enabled = true;//接触判定つける

        yield return new WaitForSeconds(3.0f);//次の攻撃まで間を置く

        state = ActionState.Fight;
        isCoroutine = false;
    }

    //回転攻撃
    IEnumerator HammerRot()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        base.animator.SetTrigger("HammerRot");
        //前を向ける
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(base.Player.transform.position - transform.position), 0.5f);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        //base.animator.speed = 0.2f;//振り上げ
        yield return new WaitForSeconds(0.5f);//予備動作

        AttackCol[1].tag = ("Bullet");//攻撃にする
        AttackCol[1].GetComponents<CapsuleCollider>()[1].enabled = true;//攻撃判定
        AttackCol[1].GetComponents<CapsuleCollider>()[0].enabled = false;//接触判定

        Audio_Source.PlayOneShot(SE[2]);//くるくるー

        //base.animator.speed = 1.5f;//振り降ろし
        base.animator.speed = 3.0f;//振り降ろし
        yield return new WaitForSeconds(0.1f);//たたいた瞬間
        base.animator.speed = 1.0f;//振り降ろし

        //Player.GetComponent<Player_ControllerVR>().SetKeylock();

        yield return new WaitForSeconds(6.5f);//攻撃後のため

        Player.GetComponent<Player_ControllerVR>().SetActive();
        AttackCol[1].tag = ("Untagged");//攻撃でなくす
        AttackCol[1].GetComponents<CapsuleCollider>()[1].enabled = false;//攻撃判定なくす
        AttackCol[1].GetComponent<CapsuleCollider>().enabled = true;//接触判定つける

        yield return new WaitForSeconds(3.0f);//次の攻撃まで間を置く

        state = ActionState.Fight;
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
        //iTween.MoveTo(this.gameObject, iTween.Hash(
        //        "x", transform.position.x - (Player.transform.position - transform.position).normalized.x * 3,//定数が突進距離
        //        "z", transform.position.z - (Player.transform.position - transform.position).normalized.z * 3,//定数が突進距離
        //        "time", 1.5f,
        //        "easetype", iTween.EaseType.easeOutBack)

        //        );

        yield return new WaitForSeconds(1.5f);//ノックバック時間
        
        state = ActionState.Fight;
        isCoroutine = false;
    }

    //ジャンプ(行って戻ってくる)
    IEnumerator Jump()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        base.animator.SetTrigger("preJump");
        //前を向ける
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(base.Player.transform.position - transform.position), 0.05f);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        yield return new WaitForSeconds(0.5f);//予備動作

        base.animator.SetTrigger("Jump");

        //上
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (JumpPoint[0].position - transform.position).x,//定数が突進距離
                "y", transform.position.y + (JumpPoint[0].position - transform.position).y,//定数が突進距離
                "z", transform.position.z + (JumpPoint[0].position - transform.position).z,//定数が突進距離
                "time", 0.5f,
                "easetype", iTween.EaseType.linear)

                );
        
        yield return new WaitForSeconds(0.5f);//攻撃後のため

        //下
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (JumpPoint[1].position - transform.position).x,//定数が突進距離
                "y", transform.position.y + (JumpPoint[1].position - transform.position).y,//定数が突進距離
                "z", transform.position.z + (JumpPoint[1].position - transform.position).z,//定数が突進距離
                "time", 0.5f,
                "easetype", iTween.EaseType.easeInBack)

                );

        base.animator.SetTrigger("postJump");

        yield return new WaitForSeconds(0.5f);//予備動作

        //CCZ.flag_quake = true;

        yield return new WaitForSeconds(1.0f);//予備動作

        //CCZ.flag_quake = false;

        base.animator.SetTrigger("preJump");
        //前を向ける
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(base.Player.transform.position - transform.position), 0.05f);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        yield return new WaitForSeconds(0.5f);//予備動作

        base.animator.SetTrigger("Jump");

        //上
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (JumpPoint[0].position - transform.position).x,//定数が突進距離
                "y", transform.position.y + (JumpPoint[0].position - transform.position).y,//定数が突進距離
                "z", transform.position.z + (JumpPoint[0].position - transform.position).z,//定数が突進距離
                "time", 0.5f,
                "easetype", iTween.EaseType.linear)

                );

        yield return new WaitForSeconds(0.5f);//攻撃後のため

        //下
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (JumpPoint[2].position - transform.position).x,//定数が突進距離
                "y", transform.position.y + (JumpPoint[2].position - transform.position).y,//定数が突進距離
                "z", transform.position.z + (JumpPoint[2].position - transform.position).z,//定数が突進距離
                "time", 0.5f,
                "easetype", iTween.EaseType.easeInBack)

                );

        base.animator.SetTrigger("postJump");

        yield return new WaitForSeconds(0.5f);//予備動作

        //CCZ.flag_quake = true;//Viveではできない。別の方法を模索

        yield return new WaitForSeconds(1.0f);//予備動作

        //CCZ.flag_quake = false;

        state = ActionState.SpikeAttack;
        isCoroutine = false;
    }

    public void Damage()
    {
        //イベント側で優先度を確認すればよい
        if (state >= ActionState.Fight)//歩きより優先順位が下
        {
            //CCZ.flag_quake = false;
            base.animator.speed = 1.0f;
            state = ActionState.Knockback;
            StopAllCoroutines();//コルーチンを止める
            isCoroutine = false;
            StartCoroutine(Knockback());
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

    public void AnimWalk()
    {
        animState = (int)ActionState.Fight;
    }

    public void AnimKnockback()
    {
        animState = (int)ActionState.Knockback;
    }

    public void AnimSpikeAttack()
    {
        animState = (int)ActionState.SpikeAttack;
    }

    public void AnimHammerAttack()
    {
        animState = (int)ActionState.HammerAttack;
    }

    public void AnimHammerRot()
    {
        animState = (int)ActionState.HammerRot;
    }

    public void AnimJump()
    {
        animState = (int)ActionState.Jump;
    }

}
