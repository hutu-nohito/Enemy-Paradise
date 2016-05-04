﻿using UnityEngine;
using System.Collections;

public class monster_Cont : Enemy_Parameter{

    /*
        (Serch) → Player → (威嚇) → 距離10m以下 → (踊る) → HP減る → (攻撃) 
                               ↓                       ↓                  ↓
                           距離10m以上              距離10m以上
                            　 ↓                       ↓
          ↑                   ←                    がっかり               ←

    */

    private Animator animator;
    private int animState = 0;//アニメータのパラメタが取得できないのでとりあえずこれで代用
    /*
        アニメーションの番号割り振り

        0   idle                            
        1   walk
        2   fight

    */                         

    //汎用
    private float time = 0;//使ったら戻す
    private Coroutine coroutine;//一度に動かすコルーチンは1つ ここでとっとけば止めるのが楽
    private bool isCoroutine = false;//コルーチンを止めるときにはfalseに戻すこと

    private int priority = 0;//状態の優先度
    /*
    Stop    0
    Walk    1
    Fight   2
    Attack  3
    
    */

    //キャラクタの状態
    public enum ActionState
    {
        Stop,//止まっている状態
        Walk,//プレイヤを見つけて近づいてる
        Fight,//臨戦態勢
        Attack,//攻撃
    }
    public ActionState state = ActionState.Fight;
    public ActionState GetState() { return state; }
    public void SetState(ActionState state) { this.state = state; }

    private ActionState oldstate = ActionState.Fight;

    //追加中/////////////////////////////////////////////////////////
    private GameObject Player;

    private bool event_find = false;

    public float rotSpeed = 5;

    // Use this for initialization
    void Start()
    {
        animator = GetComponentInChildren<Animator>();


        priority = 1;//最初はwalk

        Player = GameObject.FindWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {

        //アニメーションを取得してみる
        AnimatorStateInfo anim = animator.GetCurrentAnimatorStateInfo(0);

        //動きが止まっている状態。アニメは前状態で決めとく
        if (state == ActionState.Stop)
        {
            priority = 0;
        }

        if (state == ActionState.Walk)
        {
            priority = 1;

            if (animState != 1)
            {
                animator.SetTrigger("Run");
                animState = 1;
            }

            //前を向ける
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Player.transform.position - transform.position), 0.05f);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            iTween.MoveUpdate(this.gameObject, iTween.Hash(
                "position", new Vector3(Player.transform.position.x,transform.position.y, Player.transform.position.z),
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

        if (state == ActionState.Fight)
        {
            priority = 2;

            if (animState != 2)
            {
                animator.SetTrigger("Fight");
                animState = 2;
            }

            //前を向ける
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Player.transform.position - transform.position), 0.05f);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            //アフィン変換　回転
            Vector3 LocalPos = transform.position - Player.transform.position;//ターゲットに対するローカル座標に変換
            Vector3 cal = new Vector3(0,transform.position.y,0);
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
                coroutine = StartCoroutine(ChangeState(1.4f, ActionState.Walk));

            }

        }

        //状態が変化したら前の状態のいどうは中断
        if (oldstate != state)
        {
            time = 0;
            
        }
        oldstate = state;

    }

    /////////////////////////////////////
    //少し待ってから状態を遷移する
    IEnumerator ChangeState(float waitTime , ActionState nextState)
    {
        yield return new WaitForSeconds(waitTime);

        state = nextState;
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
