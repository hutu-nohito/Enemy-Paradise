﻿using UnityEngine;
using System.Collections;

public class monster_Cont2 : Enemy_Parameter
{

    /******************************************************************************/
    /** @brief モンスターAIテスト
    * @date 2016/05/08
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    */
    /******************************************************************************/
    /* 更新履歴
    *   攻撃はイベント
        まつ必要があるアニメーションはイベントを送るのを少し遅らせたほうがいい
        見える弾を避ける
        Itweenの調整
    */
    /******************************************************************************/

    //キャラクタの状態
    public enum ActionState
    {
        Stop,//アニメーションが終わるまで待機(状態でなく網目ーションの整合性のために必要)
        Block,//盾で防御
        Avoid,//避ける
        Attack,//攻撃
        Fight,//臨戦態勢
        Run,//プレイヤを見つけて近づいてる

    }//intにすれば優先度にできる
    public ActionState state = ActionState.Stop;
    public ActionState GetState() { return state; }
    public void SetState(ActionState state) { this.state = state; }
    
    //追加中/////////////////////////////////////////////////////////
    private GameObject Player;

    private bool event_find = false;
    private bool event_damage = false;

    public float rotSpeed = 5;

    private SimpleMove SM;

    // Use this for initialization
    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        coroutine = StartCoroutine(ChangeState(1.0f, ActionState.Run));

        Player = GameObject.FindWithTag("Player");
        
        SM = GetComponent<SimpleMove>();
        SM.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //HPがなくなった時の処理
        if(GetHP() <= 0)
        {
            Destroy(this.gameObject);//とりあえず消す
        }

        //何もしない
        if (state == ActionState.Stop)
        {
            //何もしない
        }

        //防御
        if(state == ActionState.Block)
        {
            coroutine = StartCoroutine(Block());
            
        }

        //避ける
        if (state == ActionState.Avoid)
        {
            if (animState != (int)ActionState.Stop)
            {
                animator.SetTrigger("Idle");
            }

            //避ける動作
            iTween.MoveUpdate(this.gameObject, iTween.Hash(
                    "position", new Vector3(
                    transform.position.x + ((transform.position - Player.transform.position).z / (transform.position - Player.transform.position).magnitude) * 2,
                    transform.position.y,
                    transform.position.z + (-(transform.position - Player.transform.position).x / (transform.position - Player.transform.position).magnitude) * 2
                    ),
                    "time", 0.5f,
                    "easetype", "easeInOutBack"//全然利いてない
                    )
                    );
            transform.Rotate(0,24,0);

            coroutine = StartCoroutine(ChangeState(0.25f, ActionState.Run));

        }

        //アタックするだけ
        if (state == ActionState.Attack)
        {
            coroutine = StartCoroutine(Attack());
        }

        //相手の周りを旋回
        if (state == ActionState.Fight)
        {

            if (animState != (int)ActionState.Fight)
            {
                animator.SetTrigger("Fight");
            }

            //前を向ける
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Player.transform.position - transform.position), 0.05f);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            //アフィン変換　回転
            Vector3 LocalPos = transform.position - Player.transform.position;//ターゲットに対するローカル座標に変換
            Vector3 cal = new Vector3(0, transform.position.y, 0);
            float degreeTheta = -0.1f;
            degreeTheta = -speed * rotSpeed / LocalPos.magnitude * Time.deltaTime;//動かすときはスペックで違いが出ないようにDeltatime
            cal.x = LocalPos.x * Mathf.Cos(degreeTheta * Mathf.Deg2Rad) - LocalPos.z * Mathf.Sin(degreeTheta * Mathf.Deg2Rad);
            cal.z = LocalPos.x * Mathf.Sin(degreeTheta * Mathf.Deg2Rad) + LocalPos.z * Mathf.Cos(degreeTheta * Mathf.Deg2Rad);
            LocalPos = cal;//座標に代入
            LocalPos.y = transform.position.y - Player.transform.position.y;//ジャンプさせるからYの値は変えない
            transform.position = LocalPos + Player.transform.position;//ワールド座標に直す*/

            //離れたら追いかける
            if ((transform.position - Player.transform.position).magnitude > 10)
            {
                animator.SetTrigger("H_Weapon");
                state = ActionState.Stop;
                coroutine = StartCoroutine(ChangeState(1.4f, ActionState.Run));

            }

            //ランダムで攻撃
            if ((int)Time.time % 5 == 0)//5秒ごと
            {
                float randAt1 = Random.value;
                float randAt2 = Random.value;
                
                if (randAt1 > 0.9)
                {
                    if (randAt2 < 0.2)
                    {
                        state = ActionState.Attack;
                    }
                }
            }
        }

        //見つけて近づいてる
        if (state == ActionState.Run)
        {
            
            if (animState != (int)ActionState.Run)
            {
                animator.SetTrigger("Run");
            }

            //前を向ける
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Player.transform.position - transform.position), 0.05f);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            iTween.MoveUpdate(this.gameObject, iTween.Hash(
                "position", new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z),
                "time", 20 / speed)
                );

            //近づいたら戦闘状態に移行
            if ((transform.position - Player.transform.position).magnitude < 5)
            {
                animator.SetTrigger("G_Weapon");
                state = ActionState.Stop;
                coroutine = StartCoroutine(ChangeState(1.6f, ActionState.Fight));
            }

        }

        
    }

    /////////////////////////////////////
    //少し待ってから状態を遷移する
    IEnumerator ChangeState(float waitTime, ActionState nextState)
    {
        yield return new WaitForSeconds(waitTime);

        state = nextState;
        SM.enabled = false;

    }

    //イベントが起きた時/////////////////////

    //汎用
    private float time = 0;//使ったら戻す
    private Coroutine coroutine;//一度に動かすコルーチンは1つ ここでとっとけば止めるのが楽
    private bool isCoroutine = false;//コルーチンを止めるときにはfalseに戻すこと


    //防御
    IEnumerator Block()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        //回転してほしくない
        shield.gameObject.GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = true;
        state = ActionState.Stop;//とりあえず動きを止める
        animator.SetTrigger("Block");

        //防御してる時間
        float rand = Random.Range(1,2);
        yield return new WaitForSeconds(rand);//こーゆーパラメータもインスペクタで決めるべきだと思うけどごちゃごちゃしそうでいや

        //重力に従ってほしい
        shield.gameObject.GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;

        //coroutine = StartCoroutine(ChangeState(1.0f, ActionState.Fight));
        animator.SetTrigger("Fight");
        state = ActionState.Fight;

        isCoroutine = false;
    }

    //避け
    IEnumerator Avoid()
    {
        if (isCoroutine) yield break;
        isCoroutine = true; 
        
        isCoroutine = false;

    }

    //攻撃
    IEnumerator Attack()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        //攻撃前のため
        yield return new WaitForSeconds(0.5f);//こーゆーパラメータもインスペクタで決めるべきだと思うけどごちゃごちゃしそうでいや

        Vector3 OldPlayerPos = Player.transform.position;

        //とびかかる
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "position", new Vector3(
                transform.position.x + (-(transform.position - Player.transform.position).x /* / (transform.position - Player.transform.position).magnitude */) * (transform.position - Player.transform.position).magnitude * 0.1f,
                transform.position.y + 1,
                transform.position.z + (-(transform.position - Player.transform.position).z /* / (transform.position - Player.transform.position).magnitude */) * (transform.position - Player.transform.position).magnitude * 0.1f
                ),
                "time", 0.5f,
                "easetype", iTween.EaseType.easeInOutBack)
                );

        //武器を振り下ろすまで
        yield return new WaitForSeconds(0.4f);//こーゆーパラメータもインスペクタで決めるべきだと思うけどごちゃごちゃしそうでいや

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.8f);
        //ちょい戻る
        animator.SetTrigger("Fight");
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "position", new Vector3(
                transform.position.x + ((transform.position - OldPlayerPos).x /* / (transform.position - Player.transform.position).magnitude */) / (transform.position - OldPlayerPos).magnitude * 2,
                transform.position.y + 0.5f,
                transform.position.z + ((transform.position - OldPlayerPos).z /* / (transform.position - Player.transform.position).magnitude */) / (transform.position - OldPlayerPos).magnitude * 2
                ),
                "time", 0.5f,
                "easetype", iTween.EaseType.easeInBack)
                );

        yield return new WaitForSeconds(0.8f);

        state = ActionState.Fight;
        isCoroutine = false;
    }

    public void Damage()
    {
        //イベント側で優先度を確認すればよい
        if (state > ActionState.Fight)//これで臨戦態勢より高ければやってくれる
        {
            switch (state)//今の状態によって行動を変化
            {
                case ActionState.Attack:
                    break;
                default:
                    break;
            }
        }
    }

    public void FindDamage()
    {
        if (state == ActionState.Run)
        {
            
            //避ける前のため
            coroutine = StartCoroutine(ChangeState(0.1f, ActionState.Avoid));
            
        }
    }

    public void Mikiri()
    {
        if(state == ActionState.Fight)
        {
            state = ActionState.Block;
        }
        
    }

    //アニメーション
    private Animator animator;
    private int animState = 0;//アニメータのパラメタが取得できないのでとりあえずこれで代用

    public void AnimIdle()
    {
        animState = (int)ActionState.Stop;

    }

    public void AnimBlock()
    {
        animState = (int)ActionState.Block;
        
    }

    public void AnimAttack()
    {
        //animState = 1;
        animState = (int)ActionState.Attack;

    }

    public void AnimRun()
    {
        //animState = 2;
        animState = (int)ActionState.Run;
    }

    public void Animfight()
    {
        //animState = 3;
        animState = (int)ActionState.Fight;
    }

    //武器を持つよう////////////////////////
    public Transform shield;
    public Transform weapon;
    public Transform lefthandpos;
    public Transform righthandpos;
    public Transform chestposshield;
    public Transform chestposweapon;
    private bool fightmodus = false;

    void grabshield()
    {
        shield.parent = lefthandpos;
        shield.position = lefthandpos.position;
        shield.rotation = lefthandpos.rotation;
        fightmodus = true;
    }

    void grabweapon()
    {
        weapon.parent = righthandpos;
        weapon.position = righthandpos.position;
        weapon.rotation = righthandpos.rotation;

    }

    void holstershield()
    {
        shield.parent = chestposshield;
        shield.position = chestposshield.position;
        shield.rotation = chestposshield.rotation;

    }

    void holsterweapon()
    {
        fightmodus = false;
        weapon.parent = chestposweapon;
        weapon.position = chestposweapon.position;
        weapon.rotation = chestposweapon.rotation;
    }
}
