using UnityEngine;
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

    */
    /******************************************************************************/

    //キャラクタの状態
    public enum ActionState
    {
        Stop,//アニメーションが終わるまで待機(状態でなく網目ーションの整合性のために必要)
        Attack,//攻撃
        Walk,//プレイヤを見つけて近づいてる
        Fight,//臨戦態勢
        
    }//intにすれば優先度にできる
    public ActionState state = ActionState.Fight;
    public ActionState GetState() { return state; }
    public void SetState(ActionState state) { this.state = state; }
    
    //追加中/////////////////////////////////////////////////////////
    private GameObject Player;

    private bool event_find = false;
    private bool event_damage = false;

    public float rotSpeed = 5;

    // Use this for initialization
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        
        Player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        //アニメーションを取得してみる
        AnimatorStateInfo anim = animator.GetCurrentAnimatorStateInfo(0);

        //何もしない
        if (state == ActionState.Stop)
        {
            //何もしない
        }

        //アタックするだけ
        if (state == ActionState.Attack)
        {
            coroutine = StartCoroutine(Attack());
        }

        if (state == ActionState.Walk)
        {
            
            if (animState != 2)
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

        if (state == ActionState.Fight)
        {

            if (animState != 3)
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
                coroutine = StartCoroutine(ChangeState(1.4f, ActionState.Walk));

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
                        coroutine = StartCoroutine(Attack());
                    }
                }
            }
        }
    }

    /////////////////////////////////////
    //少し待ってから状態を遷移する
    IEnumerator ChangeState(float waitTime, ActionState nextState)
    {
        yield return new WaitForSeconds(waitTime);

        state = nextState;
    }

    //イベントが起きた時/////////////////////

    //汎用
    private float time = 0;//使ったら戻す
    private Coroutine coroutine;//一度に動かすコルーチンは1つ ここでとっとけば止めるのが楽
    private bool isCoroutine = false;//コルーチンを止めるときにはfalseに戻すこと

    //攻撃
    IEnumerator Attack()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        state = ActionState.Stop;//とりあえず動きを止める

        //攻撃前のため
        yield return new WaitForSeconds(0.5f);//こーゆーパラメータもインスペクタで決めるべきだと思うけどごちゃごちゃしそうでいや

        animator.SetTrigger("Attack");
        isCoroutine = false;
        coroutine = StartCoroutine(ChangeState(1.6f, ActionState.Fight));

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

    //アニメーション
    private Animator animator;
    private int animState = 0;//アニメータのパラメタが取得できないのでとりあえずこれで代用
    /*
    アニメーションの番号割り振り          
        1   attack
        2   walk
        3   fight
    */
    
    public void AnimAttack()
    {
        animState = 1;
    }

    public void AnimRun()
    {
        animState = 2;
    }

    public void Animfight()
    {
        animState = 3;
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
