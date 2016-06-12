using UnityEngine;
using System.Collections;

public class Haniwonder : Enemy_Base
{
 
    /******************************************************************************/
    /** @brief 埴輪AIテスト
    * @date 2016/06/12
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    */
    /******************************************************************************/
    /* 更新履歴
    *   すごいスピードでうろうろする
    */
    /******************************************************************************/

    //キャラクタの状態
    public enum ActionState
    {
        Stop,//アニメーションが終わるまで待機(状態でなくアニメーションの整合性のために必要)
        //Avoid,//避ける
        Attack,//攻撃
        Fight,//臨戦態勢
        Run,//プレイヤを見つけて近づいてる
        Search,//うろうろしてる

    }//intにすれば優先度にできる
    public ActionState state = ActionState.Stop;
    public ActionState GetState() { return state; }
    public void SetState(ActionState state) { this.state = state; }

    public float rotSpeed = 5;

    // Use this for initialization
    void Start()
    {
        base.BaseStart();

        //初期状態セット
        coroutine = StartCoroutine(ChangeState(1.0f, ActionState.Search));

    }

    // Update is called once per frame
    void Update()
    {

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
            //ダウン演出
            //if (animState != (int)ActionState.Stop)
            //{
            //    base.animator.SetTrigger("Die");
            //    Destroy(this.gameObject, 3);//とりあえず消す
            //}
            //state = ActionState.Stop;
            //animState = (int)ActionState.Stop;

        }

        //相手がいなくなった時の処理
        if (base.Player == null)
        {
            //StopAllCoroutines();
            //base.Player = this.gameObject;
            //state = ActionState.Stop;
            //animState = (int)ActionState.Stop;
            ////勝利のポーズ
            //base.animator.SetTrigger("Win");
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
        
        //避ける
        //if (state == ActionState.Avoid)
        //{
        //    if (animState != (int)ActionState.Stop)
        //    {
        //        base.animator.SetTrigger("Idle");
        //    }

        //    //避ける動作
        //    iTween.MoveUpdate(this.gameObject, iTween.Hash(
        //            "position", new Vector3(
        //            transform.position.x + ((transform.position - base.Player.transform.position).z / (transform.position - base.Player.transform.position).magnitude) * 1.5f,
        //            transform.position.y,
        //            transform.position.z + (-(transform.position - base.Player.transform.position).x / (transform.position - base.Player.transform.position).magnitude) * 1.5f
        //            ),
        //            "time", 0.8f,
        //            "easetype", "easeInOutBack"//全然利いてない
        //            )
        //            );
        //    transform.Rotate(0, 12, 0);

        //    if ((transform.position - base.Player.transform.position).magnitude > 10)
        //    {
        //        coroutine = StartCoroutine(ChangeState(0.5f, ActionState.Run));
        //    }
        //    else
        //    {
        //        coroutine = StartCoroutine(ChangeState(0.5f, ActionState.Fight));
        //    }


        //}

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
                base.animator.SetTrigger("Fight");
            }

            //前を向ける
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(base.Player.transform.position - transform.position), 0.05f);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            //アフィン変換　回転
            Vector3 LocalPos = transform.position - base.Player.transform.position;//ターゲットに対するローカル座標に変換
            Vector3 cal = new Vector3(0, transform.position.y, 0);
            float degreeTheta = -0.1f;
            degreeTheta = -speed * rotSpeed / LocalPos.magnitude * Time.deltaTime;//動かすときはスペックで違いが出ないようにDeltatime
            cal.x = LocalPos.x * Mathf.Cos(degreeTheta * Mathf.Deg2Rad) - LocalPos.z * Mathf.Sin(degreeTheta * Mathf.Deg2Rad);
            cal.z = LocalPos.x * Mathf.Sin(degreeTheta * Mathf.Deg2Rad) + LocalPos.z * Mathf.Cos(degreeTheta * Mathf.Deg2Rad);
            LocalPos = cal;//座標に代入
            LocalPos.y = transform.position.y - base.Player.transform.position.y;//ジャンプさせるからYの値は変えない
            transform.position = LocalPos + base.Player.transform.position;//ワールド座標に直す*/

            //離れたら追いかける
            if ((transform.position - base.Player.transform.position).magnitude > 10)
            {
                base.animator.SetTrigger("H_Weapon");
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

            //たまーに避けつつ攻撃
            //if ((int)Time.time % 5 == 0)//5秒ごと
            //{
            //    float randAt1 = Random.value;
            //    float randAt2 = Random.value;

            //    if (randAt1 > 0.9)
            //    {
            //        if (randAt2 < 0.4)
            //        {
            //            state = ActionState.Avoid;
            //        }
            //        else
            //        {
            //            state = ActionState.Attack;
            //        }
            //    }
            //}

        }

        //見つけて近づいてる
        if (state == ActionState.Run)
        {

            if (animState != (int)ActionState.Run)
            {
                base.animator.SetTrigger("Run");
            }

            //前を向ける
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(base.Player.transform.position - transform.position), 0.05f);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            iTween.MoveUpdate(this.gameObject, iTween.Hash(
                "position", new Vector3(base.Player.transform.position.x, transform.position.y, base.Player.transform.position.z),
                "time", 20 / speed)
                );

            //近づいたら戦闘状態に移行
            if ((transform.position - base.Player.transform.position).magnitude < 5)
            {
                base.animator.SetTrigger("G_Weapon");
                state = ActionState.Stop;
                coroutine = StartCoroutine(ChangeState(1.6f, ActionState.Fight));
            }

        }

        //うろうろしてる
        if (state == ActionState.Search)
        {

            if (animState != (int)ActionState.Search)
            {
                //base.animator.SetTrigger("Run");
            }

            coroutine = StartCoroutine(Search());

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
    
    //攻撃
    IEnumerator Attack()
    {
        yield return new WaitForSeconds(1);//どないすべ
    }

    //索敵
    IEnumerator Search()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        while (true)
        {
            transform.eulerAngles = new Vector3(45, 90, 0);

            //右移動
            iTween.MoveTo(this.gameObject, iTween.Hash(
                    "position", transform.position + new Vector3(15,0,0),
                    "time", 0.5f,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            //移動時間
            yield return new WaitForSeconds(0.45f);

            iTween.RotateBy(this.gameObject, iTween.Hash(
                    "y", 90,
                    "time", 0.75f,
                    "easetype", iTween.EaseType.linear)

                    );

            //回ってる時間
            yield return new WaitForSeconds(0.75f);

            transform.eulerAngles = new Vector3(45, 0, 0);

            //前移動
            iTween.MoveTo(this.gameObject, iTween.Hash(
                    "position", transform.position + new Vector3(0, 0, 15),
                    "time", 0.5f,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            //移動時間
            yield return new WaitForSeconds(0.45f);

            iTween.RotateBy(this.gameObject, iTween.Hash(
                    "y", 90,
                    "time", 0.75f,
                    "easetype", iTween.EaseType.linear)

                    );

            //回ってる時間
            yield return new WaitForSeconds(0.75f);

            transform.eulerAngles = new Vector3(45, 270, 0);

            //左移動
            iTween.MoveTo(this.gameObject, iTween.Hash(
                    "position", transform.position + new Vector3(-15, 0, 0),
                    "time", 0.5f,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            //移動時間
            yield return new WaitForSeconds(0.45f);

            iTween.RotateBy(this.gameObject, iTween.Hash(
                    "y", 90,
                    "time", 0.75f,
                    "easetype", iTween.EaseType.linear)

                    );

            //回ってる時間
            yield return new WaitForSeconds(0.75f);

            transform.eulerAngles = new Vector3(45, 180, 0);

            //後移動
            iTween.MoveTo(this.gameObject, iTween.Hash(
                    "position", transform.position + new Vector3(0, 0, -15),
                    "time", 0.5f,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            //移動時間
            yield return new WaitForSeconds(0.45f);

            iTween.RotateBy(this.gameObject, iTween.Hash(
                    "y", 90,
                    "time", 0.75f,
                    "easetype", iTween.EaseType.linear)

                    );

            //回ってる時間
            yield return new WaitForSeconds(0.75f);
        }
        
        isCoroutine = false;
    }

    public void Damage()
    {
        //イベント側で優先度を確認すればよい
        //if (state > ActionState.Fight)//これで臨戦態勢より高ければやってくれる
        //{
        //    switch (state)//今の状態によって行動を変化
        //    {
        //        case ActionState.Attack:
        //            break;
        //        default:
        //            break;
        //    }
        //}
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
        animState = (int)ActionState.Stop;

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
    
}

