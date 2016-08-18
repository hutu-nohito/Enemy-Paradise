using UnityEngine;
using System.Collections;

public class DeathHand : Enemy_Base {


    /******************************************************************************/
    /** @brief 黒ちゃんの闇AIテスト
    * @date 2016/06/12
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    */
    /******************************************************************************/
    /* 更新履歴
    *   ランダムにふよふよする
    *   消えながら弾を撃つ
    */
    /******************************************************************************/


    private AudioSource SE;//音

    public GameObject AI;

    //キャラクタの状態
    public enum ActionState
    {
        Stop,//アニメーションが終わるまで待機(状態でなくアニメーションの整合性のために必要)
        Attack,//攻撃
        Warp,//ワープ
        Search,//うろうろしてる

    }//intにすれば優先度にできる
    public ActionState state = ActionState.Stop;
    public ActionState GetState() { return state; }
    public void SetState(ActionState state) { this.state = state; }

    //弾
    public GameObject[] Bullet;//攻撃
    public Transform[] Muzzle;//攻撃が出てくる場所

    // Use this for initialization
    void Start()
    {
        base.BaseStart();

        //初期状態セット
        coroutine = StartCoroutine(ChangeState(1.0f, ActionState.Search));
        //coroutine = StartCoroutine(ChangeState(1.0f, ActionState.Warp));

        //CPU戦用
        //Player = AI;

        SE = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        base.BaseUpdate();

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
            StopAllCoroutines();
            state = ActionState.Stop;
            animState = (int)ActionState.Stop;
            flag_fade = true;
        }

        //相手がいなくなった時の処理
        if (base.Player == null)
        {
            StopAllCoroutines();
            base.Player = this.gameObject;
            state = ActionState.Stop;
            animState = (int)ActionState.Stop;
            ////勝利のポーズ
            //base.animator.SetTrigger("Win");
            //効果音と演出
            if (!SE.isPlaying)
            {

                SE.loop = true;
                SE.Play();//SE

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

        //常にプレイヤーのほうを向いている
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Player.transform.position - transform.position), 0.05f);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        //状態遷移//////////////////////////////////////////////////////////////////////////////////

        //何もしない
        if (state == ActionState.Stop)
        {
            //アニメーションの最中など動かしたくないときに用いる
        }

        //アタックするだけ
        if (state == ActionState.Attack)
        {
            coroutine = StartCoroutine(Attack());
        }

        if (state == ActionState.Warp)
        {
            coroutine = StartCoroutine(Warp());
        }

        //うろうろしてる
        if (state == ActionState.Search)
        {

            if (animState != (int)ActionState.Search)
            {
                //base.animator.SetTrigger("Run");
            }

            //残像
            StartCoroutine(AfterImage());

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
        if (isCoroutine) yield break;
        isCoroutine = true;

        yield return new WaitForSeconds(1);//ちょっと間をおいてから攻撃

        GameObject bullet;

        //アニメーションセット
        //animator.SetTrigger("Attack");//攻撃

        bullet = GameObject.Instantiate(Bullet[0]);//通常弾
        bullet.GetComponent<Attack_Parameter>().Parent = this.gameObject;//誰が撃ったかを渡す

        bullet.GetComponent<HomingVR>().TargetSet(Player);//ホーミングのターゲットをセット
                                                          //弾を飛ばす処理
        bullet.transform.position = Muzzle[0].position;//Muzzleの位置
        bullet.transform.rotation = Quaternion.LookRotation(direction);//回転させて弾頭を進行方向に向ける

        bullet.GetComponent<Rigidbody>().velocity = (Player.transform.position + new Vector3(0,Player.transform.localScale.y / 2 ,0) - transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed;//ﾌﾟﾚｲﾔに向けて撃つ

        Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

        //効果音と演出
        /*if (!SE[0].isPlaying)
        {

            SE[0].PlayOneShot(SE[0].clip);//SE

        }*/

        yield return new WaitForSeconds(bullet.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        state = ActionState.Search;
        isCoroutine = false;
    }

    IEnumerator Warp()
    {

        if (isCoroutine) yield break;
        isCoroutine = true;

        flag_fade = true;

        yield return new WaitForSeconds(2);//消えるまで

        float randomWarp = Random.Range(-0.5f,0.5f);

        transform.position = new Vector3(
            base.Player.transform.position.x + (base.Player.transform.TransformDirection(Vector3.back).x /*+ randomWarp*/) * 10,
            home_position.y + (randomWarp / 2),
            base.Player.transform.position.z + (base.Player.transform.TransformDirection(Vector3.back).z /*+ randomWarp*/) * 10
            );//ﾌﾟﾚｲﾔの背後にワープ

        yield return new WaitForSeconds(1);//移動

        flag_fade = false;
        
        //効果音と演出
        if (!SE.isPlaying)
        {

            SE.PlayOneShot(SE.clip);//SE

        }

        yield return new WaitForSeconds(2);//現れるまで

        float randomvalue = Random.Range(0.0f, 1.0f);
        if (randomvalue < 0.8)
        {
            state = ActionState.Attack;

        }
        else if (randomvalue < 0.9)
        {
            state = ActionState.Search;

        }
        else
        {
            state = ActionState.Warp;
        }

        isCoroutine = false;

    }

    //索敵
    IEnumerator Search()
    {
        
        if (isCoroutine) yield break;
        isCoroutine = true;
        
        //ランダムにふよふよ
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", home_position.x + Random.Range(-5.0f, 5.0f),
                "y", home_position.y + Random.Range(-1.0f, 1.0f),
                "z", home_position.z + Random.Range(-5.0f, 5.0f),
                "time", 2.5f,
                "easetype", iTween.EaseType.easeInOutCubic)
                );

        yield return new WaitForSeconds(2.5f);

        //サーチ中にたまにプレイヤの後ろにワープしてくる

        float randAt1 = Random.value;
        float randAt2 = Random.value;

        if (randAt1 > 0.2)
        {
            if (randAt2 < 0.5)
            {
                StopAllCoroutines();
                state = ActionState.Warp;
                isCoroutine = false;
            }
            else
            {
                StopAllCoroutines();
                state = ActionState.Attack;
                isCoroutine = false;
            }
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

}
