using UnityEngine;
using System.Collections;

public class DethHand : Enemy_Base
{
   
    /******************************************************************************/
    /** @brief 黒ちゃんの闇AIテスト
    * @date 2016/06/12
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    */
    /******************************************************************************/
    /* 更新履歴
    *   消えながら弾を撃つ
    */
    /******************************************************************************/

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

    //ワープ時のフェード用
    public GameObject Model;//モデル
    private bool isFade = true;//魔方陣フェード用
    private float color = 0.25f;//透明度

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

        //常にプレイヤーのほうを向いている
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Player.transform.position - transform.position), 0.05f);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

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


        //ワープ時にスーって消える
        if (isFade)
        {
            if (color < 0.9) color += Time.deltaTime * 1;
            Model.GetComponent<SkinnedMeshRenderer>().materials[0].color = new Color(Model.GetComponent<SkinnedMeshRenderer>().materials[0].color.r, Model.GetComponent<SkinnedMeshRenderer>().materials[0].color.g, Model.GetComponent<SkinnedMeshRenderer>().materials[0].color.b, color);
            //Model.GetComponent<Renderer>().material.color = new Color(Model.GetComponent<Renderer>().material.color.r, Model.GetComponent<Renderer>().material.color.g, Model.GetComponent<Renderer>().material.color.b, color);

        }
        else
        {
            if (color > 0) color -= Time.deltaTime * 1;
            Model.GetComponent<SkinnedMeshRenderer>().materials[0].color = new Color(Model.GetComponent<SkinnedMeshRenderer>().materials[0].color.r, Model.GetComponent<SkinnedMeshRenderer>().materials[0].color.g, Model.GetComponent<SkinnedMeshRenderer>().materials[0].color.b, color);
            //Model.GetComponent<Renderer>().material.color = new Color(Model.GetComponent<Renderer>().material.color.r, Model.GetComponent<Renderer>().material.color.g, Model.GetComponent<Renderer>().material.color.b, color);
        }

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

            coroutine = StartCoroutine(Search());

            //サーチ中にたまにプレイヤの後ろにワープしてくる
            if ((int)Time.time % 5 == 0)//5秒ごと
            {
                float randAt1 = Random.value;
                float randAt2 = Random.value;

                if (randAt1 > 0.9)
                {
                    if (randAt2 < 0.1)
                    {
                        StopAllCoroutines();
                        state = ActionState.Warp;
                        isCoroutine = false;
                    }
                }
            }

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
                                                                         //弾を飛ばす処理
        bullet.transform.position = Muzzle[0].position;//Muzzleの位置
        bullet.transform.rotation = Quaternion.LookRotation(direction);//回転させて弾頭を進行方向に向ける

        bullet.GetComponent<Rigidbody>().velocity = (Player.transform.position - transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed;//ﾌﾟﾚｲﾔに向けて撃つ

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

        //isFade = false;

        yield return new WaitForSeconds(1);//消えるまで

        transform.position = base.Player.transform.position;// + new Vector3(0, base.jump, 0) + base.Player.transform.TransformDirection(Vector3.back) * 5;//ﾌﾟﾚｲﾔの背後にワープ

        yield return new WaitForSeconds(1);//移動

        //isFade = true;

        yield return new WaitForSeconds(1);//現れるまで

        state = ActionState.Attack;
        isCoroutine = false;

    }

    //索敵
    IEnumerator Search()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        while (true)
        {
            //右移動
            iTween.MoveTo(this.gameObject, iTween.Hash(
                    "position", transform.position + new Vector3(10, 0, 0),
                    "time", 2.5f,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            //移動時間
            yield return new WaitForSeconds(2.5f);
            
            //待ってる時間
            yield return new WaitForSeconds(0.5f);
            
            //前移動
            iTween.MoveTo(this.gameObject, iTween.Hash(
                    "position", transform.position + new Vector3(0, 0, 10),
                    "time", 2.5f,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            //移動時間
            yield return new WaitForSeconds(2.5f);

            //待ってる時間
            yield return new WaitForSeconds(0.5f);
            
            //左移動
            iTween.MoveTo(this.gameObject, iTween.Hash(
                    "position", transform.position + new Vector3(-10, 0, 0),
                    "time", 2.5f,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            //移動時間
            yield return new WaitForSeconds(2.5f);

            //待ってる時間
            yield return new WaitForSeconds(0.5f);
            
            //後移動
            iTween.MoveTo(this.gameObject, iTween.Hash(
                    "position", transform.position + new Vector3(0, 0, -10),
                    "time", 2.5f,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            //移動時間
            yield return new WaitForSeconds(2.5f);

            //待ってる時間
            yield return new WaitForSeconds(0.5f);

        }
        
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
