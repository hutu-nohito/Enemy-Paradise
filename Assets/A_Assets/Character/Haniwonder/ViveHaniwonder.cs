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
    */
    /******************************************************************************/

    private AudioSource SE;//音
    
    //弾
    public GameObject[] Bullet;//攻撃
    public Transform[] Muzzle;//攻撃が出てくる場所
    public GameObject Avatar;//分身

    //キャラクタの状態
    public enum ActionState
    {
        Stop,//アニメーションが終わるまで待機(状態でなくアニメーションの整合性のために必要)
        Attack,//攻撃
        Beam,//ビーム
        Counter,//攻撃に対してカウンターしてくる
        Exercise,//体操
        AfterImage,//影分身して突進
        BeamSprinkler,//ビーム乱れうち

    }//intにすれば優先度にできる
    public ActionState state = ActionState.Stop;
    public ActionState GetState() { return state; }
    public void SetState(ActionState state) { this.state = state; }

    public GameObject AttackCol;//攻撃判定

    // Use this for initialization
    void Start()
    {
        base.BaseStart();

        //初期状態セット
        coroutine = StartCoroutine(ChangeState(5.0f, ActionState.AfterImage));

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
            transform.Rotate(0, 10, 0);//とりあえず回転させとく
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

        //カウンター
        if (state == ActionState.Counter)
        {
            //残像
            //StartCoroutine(AfterImage());

            coroutine = StartCoroutine(Counter());
        }

        //アタックするだけ
        if (state == ActionState.Attack)
        {

            //残像
            //StartCoroutine(AfterImage());
            coroutine = StartCoroutine(Attack());
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
            //残像
            //StartCoroutine(AfterImage());

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

    //攻撃
    IEnumerator Attack()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        base.animator.SetTrigger("Run");
        AttackCol.SetActive(true);

        //プレイヤに突進
        ReverseAfterImage();//残像
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (Player.transform.position - transform.position).normalized.x * 10,//定数が突進距離
                "z", transform.position.z + (Player.transform.position - transform.position).normalized.z * 10,//定数が突進距離
                "time", 1.0f,
                "easetype", iTween.EaseType.easeInOutBack)

                );

        yield return new WaitForSeconds(0.1f);

        //前を向ける
        iTween.RotateTo(this.gameObject, iTween.Hash(
                "y", Mathf.Repeat(Quaternion.LookRotation(-GetMove()).eulerAngles.y, 360.0f),//(たまにおかしくなるので後で検証)
                "time", 0.25f,
                "easetype", iTween.EaseType.linear)

                );

        yield return new WaitForSeconds(1);
        ReverseAfterImage();//残像

        AttackCol.SetActive(false);
        state = ActionState.Exercise;
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

        Bullet[0].SetActive(true);

        Bullet[0].transform.Rotate(-Mathf.Atan((Player.transform.position.y - transform.position.y) / (Player.transform.position - transform.position).magnitude) * (180 / Mathf.PI), 0, 0);

        //効果音と演出
        if (!SE.isPlaying)
        {

            SE.PlayOneShot(SE.clip);//SE

        }
        
        yield return new WaitForSeconds(0.7f);//撃った後の硬直

        Bullet[0].SetActive(false);
        Bullet[0].transform.rotation = new Quaternion(0, 0, 0, 0);

        state = ActionState.Exercise;
        isCoroutine = false;
    }

    //カウンター
    IEnumerator Counter()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        base.animator.SetTrigger("Run");
        AttackCol.SetActive(true);

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

        AttackCol.SetActive(false);
        state = ActionState.Beam;
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
            //Avatars[i].GetComponentInChildren<Haniwonder>().AttackCol.SetActive(true);
            Avatars[i].transform.position = new Vector3(
                Player.transform.position.x + GetDistansP() * Mathf.Cos(Angle + ((i - 2) * 30 * Mathf.PI / 180)),
                transform.position.y,
                Player.transform.position.z + GetDistansP() * (Mathf.Sin(Angle + ((i - 2) * 30 * Mathf.PI / 180)))
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
            //Avatars[i].GetComponentInChildren<Haniwonder>().AttackCol.SetActive(true);
            Avatars[i].transform.position = new Vector3(
                Player.transform.position.x + GetDistansP() * Mathf.Cos(Angle + ((i - 1) * 30 * Mathf.PI / 180)),
                transform.position.y,
                Player.transform.position.z + GetDistansP() * (Mathf.Sin(Angle + ((i - 1) * 30 * Mathf.PI / 180)))
                );
            //Avatars[i].transform.position = AffineRot(Avatars[i].transform.position);//playerとの位置関係で変換
            Avatars[i].transform.LookAt(Player.transform.position);
            Avatars[i].GetComponentInChildren<Attack_Parameter>().SetParent(this.gameObject);//親を設定
        }

        Avatars[4] = this.gameObject;//５番目が自分自身

        transform.LookAt(Player.transform.position);//方向のみを合わせたいならXとYを0に
        base.animator.SetTrigger("Run");
        AttackCol.SetActive(true);

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

        AttackCol.SetActive(false);

        yield return new WaitForSeconds(3);

        state = ActionState.AfterImage;

        isCoroutine = false;
    }

    public void Damage()
    {
        //イベント側で優先度を確認すればよい
        if (state >= ActionState.Exercise)//体操より優先順位が下
        {
            state = ActionState.Counter;
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
        animState = (int)ActionState.Stop;
    }

    public void AnimAttack()
    {
        animState = (int)ActionState.Attack;
    }

    public void AnimExercise()
    {
        animState = (int)ActionState.Exercise;
    }

    public void AnimRun()
    {
        animState = (int)ActionState.Attack;
    }

}
