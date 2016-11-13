using UnityEngine;
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
     * level1,2,3 はにわんだー
     * level? はにまんさー
     * level4 熱血はにわんだー
     */

    private AudioSource SE;//音
    public AudioClip[] cv;//黒ちゃんの声
    
    //弾
    public GameObject[] Bullet;//攻撃
    public Transform[] Muzzle;//攻撃が出てくる場所
    public GameObject Avatar;//分身
    public GameObject TacleCol;//体当たり攻撃判定
    public GameObject HeadCol;//頭突き攻撃判定
    GameObject[] Avatars = new GameObject[3];

    //Timer
    float timer = 0;//使ったら0に

    //キャラクタの状態
    public enum ActionState
    {
        Stop,//アニメーションが終わるまで待機(状態でなくアニメーションの整合性のために必要)
        Guard,//回避行動全般
        Exercise,//体操 一巡100秒
        Idle,//腕組み
        Run,//純粋に走ってる状態？
        Tackle,//体当たり
        Headbutt,//頭突き
        Beam,//ビーム        
        Counter,//攻撃に対してカウンターしてくる
        AfterImage,//影分身して突進
        Knockback//

    }//intにすれば優先度にできる
    public ActionState state = ActionState.Stop;
    public ActionState GetState() { return state; }
    public void SetState(ActionState state) { this.state = state; }

    // Use this for initialization
    void Start()
    {
        base.BaseStart();

        //初期状態セット
        if (level <= 3)
        {
            coroutine = StartCoroutine(ChangeState(7.5f, ActionState.Exercise));
        }
        if (level == 4)
        {
            coroutine = StartCoroutine(ChangeState(7.5f, ActionState.Idle));
        }

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
                Manager.GetComponent<QuestManager>().MonsterCount();
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
                timer += Time.deltaTime;

                if (timer > 1.5f)//ちょっと待ってから
                {
                    StopAllCoroutines();//コルーチンはどうやって止めたらいいんだろう
                    flag_win = true;
                    timer = 0;
                    state = ActionState.Stop;
                    animState = (int)ActionState.Run;
                    ReverseAfterImage();//残像
                    if (level <= 3)//はにわんだー
                    {
                        base.animator.SetTrigger("Dash");
                    }
                    if (level == 4)//熱血
                    {
                        base.animator.SetTrigger("Dance");
                    }
                }

            }
            else
            {
                if (level <= 3)//はにわんだー
                {
                    //勝利のポーズ(相手の周りを走り回る)
                    transform.position = base.AffineRot(transform.position, 1500);
                    //前を向ける(進行方向)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(GetMove()), 0.5f);
                }
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

        //レベル100で動かない
        if(level != 100)
        {

            //体操
            if (state == ActionState.Exercise)
            {
                if (animState != (int)ActionState.Exercise)
                {
                    transform.LookAt(Player.transform.position);
                    base.animator.SetTrigger("Exercise");
                }

                timer += Time.deltaTime;
                if (level == 2)
                {
                    if (timer >= 6)
                    {
                        state = ActionState.Headbutt;
                        timer = 0;
                    }
                }
                if (level == 3)
                {
                    if (timer >= 3.2f)
                    {
                        state = ActionState.Beam;
                        timer = 0;
                    }
                }
            }

            //腕組み
            if (state == ActionState.Idle)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(base.Player.transform.position - transform.position), 0.5f);
                if (animState != (int)ActionState.Idle)
                {
                    base.animator.SetTrigger("Idle");
                }

                if ((int)Time.time % 10 == 0)//10秒ごと
                {
                    float randAt1 = Random.value;


                    if (randAt1 > 0.7)
                    {
                        state = ActionState.AfterImage;
                    }
                    else
                    {
                        state = ActionState.Headbutt;
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

            //頭突き
            if (state == ActionState.Headbutt)
            {
                coroutine = StartCoroutine(Headbutt());
            }

            //ビーム
            if (state == ActionState.Beam)
            {
                coroutine = StartCoroutine(Beam());
            }



            //パターン//////////////////////////////////////////////

            if (state == ActionState.AfterImage)
            {

                coroutine = StartCoroutine(AvatarAttack());
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

    //体当たり
    IEnumerator Tackle()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        if (level == 1)
        {
            base.animator.SetTrigger("Knockback");
        }
        if (level == 4)
        {
            base.animator.SetTrigger("Yell");
        }
        
        yield return new WaitForSeconds(2.0f);

        if (level == 1)
        {
            base.animator.SetTrigger("Dash");
        }
        if (level == 4)
        {
            base.animator.SetTrigger("Run");
        }

        TacleCol.SetActive(true);

        //プレイヤに突進
        if(level == 4)
        {
            ReverseAfterImage();//残像
        }
        
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (Player.transform.position - transform.position).normalized.x * (15 + GetDistanceP()),//定数が突進距離
                "z", transform.position.z + (Player.transform.position - transform.position).normalized.z * (15 + GetDistanceP()),//定数が突進距離
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

        yield return new WaitForSeconds(3);

        if (level == 4)
        {
            ReverseAfterImage();//残像
        }

        TacleCol.SetActive(false);

        if (level == 1)
        {
            state = ActionState.Exercise;
        }
        if (level == 4)
        {
            state = ActionState.Idle;
        }

        isCoroutine = false;
    }

    //頭突き
    IEnumerator Headbutt()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        if (level == 2)
        {
            base.animator.SetTrigger("Dash");
        }
        if (level == 4)
        {
            base.animator.SetTrigger("Yell");
        }

        yield return new WaitForSeconds(2.0f);

        base.animator.SetTrigger("Headbutt");
        HeadCol.SetActive(true);

        //とりあえずノックバックしないようにしとく
        state = ActionState.Guard;

        //プレイヤに突進
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (Player.transform.position - transform.position).normalized.x * (15 + GetDistanceP()),//定数が突進距離
                "z", transform.position.z + (Player.transform.position - transform.position).normalized.z * (15 + GetDistanceP()),//定数が突進距離
                "time", 3.0f,
                "easetype", iTween.EaseType.linear)

                );

        yield return new WaitForSeconds(0.1f);

        //前を向ける
        iTween.RotateTo(this.gameObject, iTween.Hash(
                "y", Mathf.Repeat(Quaternion.LookRotation(GetMove()).eulerAngles.y, 360.0f),//(たまにおかしくなるので後で検証)
                "time", 0.25f,
                "easetype", iTween.EaseType.linear)

                );

        yield return new WaitForSeconds(3);
        //ここまでに当たらなかったら失敗
        base.animator.SetTrigger("falseHeadbutt");
        HeadCol.SetActive(false);

        yield return new WaitForSeconds(3);

        if (level == 2)
        {
            state = ActionState.Exercise;
        }
        if (level == 4)
        {
            state = ActionState.Idle;
        }

        isCoroutine = false;
    }

    //頭突きが当たった
    IEnumerator HitHead()
    {
        HeadCol.SetActive(false);
        base.animator.SetTrigger("trueHeadbutt");

        //反動
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x - (Player.transform.position - transform.position).normalized.x * 10,//定数が突進距離
                "z", transform.position.z - (Player.transform.position - transform.position).normalized.z * 10,//定数が突進距離
                "time", 1.0f,
                "easetype", iTween.EaseType.linear)

                );

        yield return new WaitForSeconds(2.0f);
        
        if (level == 2)
        {
            state = ActionState.Exercise;
        }
        if (level == 4)
        {
            state = ActionState.Idle;
        }


        isCoroutine = false;
    }

    //ビーム
    IEnumerator Beam()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        base.animator.SetTrigger("Beam");

        //前を向ける
        //iTween.RotateTo(this.gameObject, iTween.Hash(
        //        "y", Mathf.Repeat(Quaternion.LookRotation(Player.transform.position - Muzzle[0].transform.position).eulerAngles.y, 360.0f),//(たまにおかしくなるので後で検証),
        //        "time", 0.2f,
        //        "easetype", iTween.EaseType.linear)

        //        );
        transform.LookAt(Player.transform.position);

        yield return new WaitForSeconds(0.25f);//

        SE.PlayOneShot(cv[1]);//SE

        Bullet[0].SetActive(true);

        Bullet[0].transform.Rotate(-Mathf.Atan((Player.transform.position.y - transform.position.y) / (Player.transform.position - transform.position).magnitude) * (180 / Mathf.PI), 0, 0);

        //効果音と演出
        SE.PlayOneShot(cv[0]);//SE

        //yield return new WaitForSeconds(0.7f);//撃った後の硬直
        yield return new WaitForSeconds(1.0f);//撃った後の硬直

        Bullet[0].SetActive(false);
        Bullet[0].transform.rotation = new Quaternion(0, 0, 0, 0);

        if (level == 3)
        {
            state = ActionState.Exercise;
        }
        if (level == 4)
        {
            state = ActionState.Idle;
        }

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

        float Angle = GetAngleP();
        if(GetPositionP().z < 0)
        {
            Angle = -Angle;
        }
        //分身を2体出す
        for(int i = 0; i < Avatars.Length / 2; i++)//半分より前
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
        for (int i = Avatars.Length / 2; i < Avatars.Length - 1; i++)//半分より後
        {
            //分身ははじめから走ってる
            Avatars[i] = GameObject.Instantiate(Avatar);
            //Avatars[i].GetComponentInChildren<Haniwonder>().animator.SetTrigger("Run");
            //Avatars[i].GetComponentInChildren<Haniwonder>().TacleCol.SetActive(true);
            Avatars[i].transform.position = new Vector3(
                Player.transform.position.x + GetDistanceP() * Mathf.Cos(Angle + ((i) * 30 * Mathf.PI / 180)),
                transform.position.y,
                Player.transform.position.z + GetDistanceP() * (Mathf.Sin(Angle + ((i) * 30 * Mathf.PI / 180)))
                );
            //Avatars[i].transform.position = AffineRot(Avatars[i].transform.position);//playerとの位置関係で変換
            Avatars[i].transform.LookAt(Player.transform.position);
            Avatars[i].GetComponentInChildren<Attack_Parameter>().SetParent(this.gameObject);//親を設定
        }

        Avatars[2] = this.gameObject;//５番目が自分自身

        transform.LookAt(Player.transform.position);//方向のみを合わせたいならXとYを0に
        base.animator.SetTrigger("Run");
        TacleCol.SetActive(true);

        int random;
        int[] number = new int[3];//入れ替え用
        //まず0～4の配列を作る
        for (int i = 0; i < number.Length; i++)
        {
            number[i] = i;
        }
        //シャッフル
        for (int i = 0; i < number.Length; i++)
        {
            random = Random.Range(0, 3);
            int temp = number[i];
            number[i] = number[random];
            number[random] = temp;
        }

        yield return new WaitForSeconds(3);

        for (int i = 0; i < 3; i++)
        {         
            
            //プレイヤに突進
            iTween.MoveTo(Avatars[number[i]], iTween.Hash(
                    "x", Avatars[number[i]].transform.position.x + (Player.transform.position - Avatars[number[i]].transform.position).normalized.x * (15 + GetDistanceP()),//定数が突進距離
                    "z", Avatars[number[i]].transform.position.z + (Player.transform.position - Avatars[number[i]].transform.position).normalized.z * (15 + GetDistanceP()),//定数が突進距離
                    "time", 4,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            yield return new WaitForSeconds(0.1f);

            //前を向ける
            if (number[i] != 2)
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
            if (number[i] != 2)
            {
                Destroy(Avatars[number[i]], 6);
            }

            yield return new WaitForSeconds(0.3f);

        }

        TacleCol.SetActive(false);

        yield return new WaitForSeconds(3);

        state = ActionState.Idle;

        isCoroutine = false;
    }

    IEnumerator Avoid()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        if (animState != (int)ActionState.Guard)
        {
            base.animator.SetTrigger("Avoid_L");
        }
            
        
        //避ける動作
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "position", new Vector3(
                transform.position.x + ((transform.position - base.Player.transform.position).z / (transform.position - base.Player.transform.position).magnitude) * 3.5f,
                transform.position.y,
                transform.position.z + (-(transform.position - base.Player.transform.position).x / (transform.position - base.Player.transform.position).magnitude) * 3.5f
                ),
                "time", 0.8f,
                "easetype", iTween.EaseType.linear
                )
                );

        //前を向ける
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(base.Player.transform.position - transform.position), 0.05f);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        yield return new WaitForSeconds(0.8f);//
        
        state = ActionState.Beam;
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
        //        "x", transform.position.x - (Player.transform.position - transform.position).normalized.x * 2,//定数が突進距離
        //        "z", transform.position.z - (Player.transform.position - transform.position).normalized.z * 2,//定数が突進距離
        //        "time", 1.0f,
        //        "easetype", iTween.EaseType.easeOutBack)

        //        );

        yield return new WaitForSeconds(1.0f);//

        base.animator.SetTrigger("Yell");

        yield return new WaitForSeconds(1.5f);//攻撃後のため

        if(level <= 3)
        {
            state = ActionState.Exercise;
        }
        if (level == 4)
        {
            state = ActionState.Idle;
        }

        isCoroutine = false;
    }

    //回避用
    public void Guard()
    {
        if(state == ActionState.Idle)
        {
            state = ActionState.Guard;
            StopAllCoroutines();
            StartCoroutine(Avoid());
        }
        
    }

    public void Hit()
    {

        StopAllCoroutines();
        StartCoroutine(HitHead());
        
    }

    public void Damage()
    {
        //イベント側で優先度を確認すればよい
        if (GetHP() >= 0)
        {
            if (state > ActionState.Exercise)//体操より優先順位が下
            {
                state = ActionState.Knockback;

                //いろいろ消さないと・・・(後処理を別でできるといいかも)
                StopAllCoroutines();//コルーチンはどうやって止めたらいいんだろう
                TacleCol.SetActive(false);
                HeadCol.SetActive(false);
                isCoroutine = false;
                //beam
                Bullet[0].SetActive(false);
                Bullet[0].transform.rotation = new Quaternion(0, 0, 0, 0);
                timer = 0;
                for (int i = 0;i < 2; i++)
                {
                    Destroy(Avatars[i]);
                }

                StartCoroutine(Knockback());
            }
            if (level == 1)
            {
                if (state == ActionState.Exercise)//体操
                {
                    state = ActionState.Tackle;
                }
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

    //キャラクターの状態をリセットするために必要なもの
    void Reset()
    {
        timer = 0;
        isCoroutine = false;
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

    public void AnimDash()
    {
        animState = (int)ActionState.Run;
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
