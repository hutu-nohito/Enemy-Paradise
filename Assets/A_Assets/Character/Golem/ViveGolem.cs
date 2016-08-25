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
    *   
    */
    /******************************************************************************/

    //キャラクタの状態
    public enum ActionState
    {
        Stop,//アニメーションが終わるまで待機(状態でなく網目ーションの整合性のために必要)
        SpikeAttack,//とげ攻撃
        HammerAttack,//ハンマー攻撃
        Jump,//跳ぶらしい
        Fight,//臨戦態勢

    }//intにすれば優先度にできる
    public ActionState state = ActionState.Stop;
    public ActionState GetState() { return state; }
    public void SetState(ActionState state) { this.state = state; }

    public GameObject[] AttackCol;//攻撃判定
    public Transform[] JumpPoint;//とりあえず決まったとこにジャンプ

    //演出
    private Camera_ControllerZ CCZ;

    // Use this for initialization
    void Start()
    {
        base.BaseStart();

        CCZ = Camera.main.gameObject.GetComponent<Camera_ControllerZ>();

        //初期状態セット
        coroutine = StartCoroutine(ChangeState(2.0f, ActionState.HammerAttack));

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
            flag_fade = true;
            transform.Rotate(2, 0, 0);//たおしてみる
            Destroy(this.gameObject, 1);//とりあえず消す
            state = ActionState.Stop;
            animState = (int)ActionState.Stop;

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

            if (animState != (int)ActionState.Fight)
            {
                base.animator.SetTrigger("Idle");
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

        base.animator.SetTrigger("SpikeAttack");
        //前を向ける
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(base.Player.transform.position - transform.position), 0.05f);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        yield return new WaitForSeconds(0.5f);//予備動作

        AttackCol[0].SetActive(true);

        yield return new WaitForSeconds(1.5f);//攻撃後のため
        
        AttackCol[0].SetActive(false);
        state = ActionState.SpikeAttack;
        isCoroutine = false;
    }

    //ハンマー攻撃
    IEnumerator HammerAttack()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        base.animator.SetTrigger("HammerAttack");
        //前を向ける
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(base.Player.transform.position - transform.position), 0.05f);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        yield return new WaitForSeconds(0.5f);//予備動作

        AttackCol[1].SetActive(true);
        AttackCol[1].GetComponent<CapsuleCollider>().enabled = false;//振ってるときは接触判定をなくす

        yield return new WaitForSeconds(0.2f);//攻撃後のため

        CCZ.flag_quake = true;

        yield return new WaitForSeconds(0.3f);//攻撃後のため

        CCZ.flag_quake = false;

        yield return new WaitForSeconds(0.5f);//攻撃後のため

        AttackCol[1].SetActive(false);
        AttackCol[1].GetComponent<CapsuleCollider>().enabled = true;//接触判定つける
        

        state = ActionState.HammerAttack;
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

        CCZ.flag_quake = true;

        yield return new WaitForSeconds(1.0f);//予備動作

        CCZ.flag_quake = false;

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

        CCZ.flag_quake = true;

        yield return new WaitForSeconds(1.0f);//予備動作

        CCZ.flag_quake = false;

        state = ActionState.Jump;
        isCoroutine = false;
    }

    //アニメーション///////////////////////////////////////////////////////
    private int animState = 0;//アニメータのパラメタが取得できないのでとりあえずこれで代用

    public void AnimIdle()
    {
        animState = (int)ActionState.Stop;
    }

    public void AnimWalk()
    {
        animState = (int)ActionState.Fight;
    }

    public void AnimSpikeAttack()
    {
        animState = (int)ActionState.SpikeAttack;
    }

    public void AnimHammerAttack()
    {
        animState = (int)ActionState.HammerAttack;
    }

    public void AnimJump()
    {
        animState = (int)ActionState.Jump;
    }

}
