using UnityEngine;
using System.Collections;
using System.Collections.Generic;//List用

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
    *   普段は体操してて、プレイヤーが近づくと突撃してくる
    *   プレイヤーの動きに反応するので見つけるとかはない
    *   ビーム
    */
    /******************************************************************************/

    private AudioSource SE;//音
    public GameObject AI;

    //残像用
    //public GameObject Model;//アーマチュア

    //弾
    public GameObject[] Bullet;//攻撃
    public Transform[] Muzzle;//攻撃が出てくる場所

    //キャラクタの状態
    public enum ActionState
    {
        Stop,//アニメーションが終わるまで待機(状態でなくアニメーションの整合性のために必要)
        Attack,//攻撃
        Beam,//ビーム
        Counter,//攻撃に対してカウンターしてくる
        Exercise,//体操

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
        coroutine = StartCoroutine(ChangeState(1.0f, ActionState.Exercise));

        //Player = AI;
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
            transform.Rotate(2,0,0);//たおしてみる
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
            StartCoroutine(AfterImage());

            coroutine = StartCoroutine(Counter());
        }

        //アタックするだけ
        if (state == ActionState.Attack)
        {
            //残像
            StartCoroutine(AfterImage());

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

            if((transform.position - Player.transform.position).magnitude < 10)//プレイヤーとの距離が近くなったら
            {
                float randomtime = Random.Range(0.0f, 1.0f);
                coroutine = StartCoroutine(ChangeState(0.0f,ActionState.Attack));//ばらつきを出す
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

    //残像
    //IEnumerator AfterImage (GameObject Image)
    //{
    //    yield return new WaitForSeconds(0.5f);

    //    GameObject AfterImage = Instantiate(Image);
    //    AfterImage.transform.position = Old_position;
    //    AfterImage.transform.rotation = Quaternion.Euler(Image.transform.eulerAngles.x, transform.rotation.y + 90 , Image.transform.eulerAngles.x);//なぜか90度ずれてる

    //    SkinnedMeshRenderer[] AfterImageMesh = AfterImage.GetComponentsInChildren<SkinnedMeshRenderer>();

    //    //消したいメッシュの時だけ消す
    //    for (int i = 0;i < AfterImageMesh.Length; i++)
    //    {
    //        Material[] AfterImageMaterial = AfterImageMesh[i].materials;

    //        //消したいマテリアルの時だけ消す(条件分岐で何とかする)
    //        for(int j = 0;j < AfterImageMaterial.Length; j++)
    //        {
    //            //RenderingModeを切り替えるためにはこの7個の設定を変えなければならない(Standard Shader)
    //            AfterImageMaterial[j].SetFloat("_Mode", 2);//たぶんFade
    //            AfterImageMaterial[j].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
    //            AfterImageMaterial[j].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
    //            AfterImageMaterial[j].SetInt("_ZWrite", 0);
    //            AfterImageMaterial[j].DisableKeyword("_ALPHATEST_ON");
    //            AfterImageMaterial[j].EnableKeyword("_ALPHABLEND_ON");
    //            AfterImageMaterial[j].DisableKeyword("_ALPHAPREMULTIPLY_ON");

    //            AfterImageMaterial[j].color = new Color(AfterImageMaterial[j].color.r, AfterImageMaterial[j].color.g, AfterImageMaterial[j].color.b, 0.2f); ;
    //        }
            
    //    }
                
    //    Destroy(AfterImage, 0.1f);

    //}

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
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (Player.transform.position - transform.position).normalized.x * 10,//定数が突進距離
                "z", transform.position.z + (Player.transform.position - transform.position).normalized.z * 10,//定数が突進距離
                "time", 1.0f,
                "easetype", iTween.EaseType.easeInOutBack)

                );

        yield return new WaitForSeconds(0.1f);

        //前を向ける
        iTween.RotateTo(this.gameObject, iTween.Hash(
                "y", Mathf.Repeat(Quaternion.LookRotation(-GetMove()).eulerAngles.y, 360.0f) ,//(たまにおかしくなるので後で検証)
                "time", 0.25f,
                "easetype", iTween.EaseType.linear)

                );
        
        yield return new WaitForSeconds(1);

        AttackCol.SetActive(false);
        state = ActionState.Exercise;
        isCoroutine = false;
    }

    //ビーム
    IEnumerator Beam()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        yield return new WaitForSeconds(1.0f);//

        //前を向ける
        iTween.RotateTo(this.gameObject, iTween.Hash(
                "y", Mathf.Repeat(Quaternion.LookRotation(Player.transform.position - Muzzle[0].transform.position).eulerAngles.y, 360.0f),//(たまにおかしくなるので後で検証),
                "time", 0.25f,
                "easetype", iTween.EaseType.linear)

                );

        yield return new WaitForSeconds(0.25f);//

        Bullet[1].SetActive(true);

        Bullet[1].transform.Rotate(-Mathf.Atan((Player.transform.position.y - transform.position.y)/ (Player.transform.position - transform.position).magnitude) * (180 / Mathf.PI),0,0);

        //効果音と演出
        if (!SE.isPlaying)
        {

            SE.PlayOneShot(SE.clip);//SE

        }

        //アニメーションセット
        //animator.SetTrigger("Beam");//攻撃

        //GameObject bullet;

        //bullet = GameObject.Instantiate(Bullet[0]);//通常弾
        //bullet.GetComponent<Attack_Parameter>().Parent = this.gameObject;//誰が撃ったかを渡す

        ////弾を飛ばす処理
        //bullet.transform.position = Muzzle[0].position;//Muzzleの位置
        //bullet.transform.rotation = Quaternion.LookRotation((Player.transform.position - Muzzle[0].transform.position).normalized);//回転させて弾頭を進行方向に向ける


        //bullet.GetComponent<Rigidbody>().velocity = ((Player.transform.position + new Vector3(0,1,0)) - Muzzle[0].transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed;//ﾌﾟﾚｲﾔに向けて撃つ
        //Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

        //効果音と演出
        /*if (!SE[0].isPlaying)
        {

            SE[0].PlayOneShot(SE[0].clip);//SE

        }*/

        yield return new WaitForSeconds(0.7f);//撃った後の硬直

        Bullet[1].SetActive(false);
        Bullet[1].transform.rotation = new Quaternion(0, 0, 0, 0);

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

        float randomdist = Random.Range(0.0f,1.0f);
        randomdist = 0.5f;

        //ジグザグ突進
        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (Player.transform.position.x - transform.position.x + 5) * randomdist * 2,
                "z", transform.position.z + (Player.transform.position.z - transform.position.z + 5) * (1 - randomdist) * 2,//定数が突進距離
                "time", randomdist,
                "easetype", iTween.EaseType.linear)

                );


        yield return new WaitForSeconds(0.1f);

        //前を向ける
        iTween.RotateTo(this.gameObject, iTween.Hash(
                "y", Mathf.Repeat(Quaternion.LookRotation(-GetMove()).eulerAngles.y, 360.0f),//(たまにおかしくなるので後で検証)
                "time", 0.25f,
                "easetype", iTween.EaseType.linear)

                );

        yield return new WaitForSeconds(randomdist);

        iTween.MoveTo(this.gameObject, iTween.Hash(
                "x", transform.position.x + (Player.transform.position.x - transform.position.x - 5) * (1 - randomdist) * 2,
                "z", transform.position.z + (Player.transform.position.z - transform.position.z - 5) * randomdist * 2,//定数が突進距離
                "time", (1 - randomdist),
                "easetype", iTween.EaseType.easeOutBack)

                );


        yield return new WaitForSeconds(0.1f);

        //前を向ける
        iTween.RotateTo(this.gameObject, iTween.Hash(
                "y", Mathf.Repeat(Quaternion.LookRotation(-GetMove()).eulerAngles.y, 360.0f),//(たまにおかしくなるので後で検証)
                "time", 0.25f,
                "easetype", iTween.EaseType.linear)

                );

        yield return new WaitForSeconds(1 - randomdist);
        
        AttackCol.SetActive(false);
        state = ActionState.Exercise;
        isCoroutine = false;
    }

    //索敵
    IEnumerator Search()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        while (true)
        {
            transform.eulerAngles = new Vector3(0, 90, 0);
            base.animator.SetTrigger("Run");

            //右移動
            iTween.MoveTo(this.gameObject, iTween.Hash(
                    "position", transform.position + new Vector3(15,0,0),
                    "time", 1.0f,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            //移動時間
            yield return new WaitForSeconds(0.95f);

            base.animator.SetTrigger("Idle");

            //iTween.RotateBy(this.gameObject, iTween.Hash(
            //        "y", 90,
            //        "time", 0.75f,
            //        "easetype", iTween.EaseType.linear)

            //        );
            
            //回ってる時間
            yield return new WaitForSeconds(1.65f);

            transform.eulerAngles = new Vector3(0, 0, 0);
            base.animator.SetTrigger("Run");

            //前移動
            iTween.MoveTo(this.gameObject, iTween.Hash(
                    "position", transform.position + new Vector3(0, 0, 15),
                    "time", 1.0f,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            //移動時間
            yield return new WaitForSeconds(0.95f);

            base.animator.SetTrigger("Idle");

            //iTween.RotateBy(this.gameObject, iTween.Hash(
            //        "y", 90,
            //        "time", 0.75f,
            //        "easetype", iTween.EaseType.linear)

            //        );

            //回ってる時間
            yield return new WaitForSeconds(1.65f);

            transform.eulerAngles = new Vector3(0, 270, 0);
            base.animator.SetTrigger("Run");

            //左移動
            iTween.MoveTo(this.gameObject, iTween.Hash(
                    "position", transform.position + new Vector3(-15, 0, 0),
                    "time", 1.0f,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            //移動時間
            yield return new WaitForSeconds(0.95f);

            base.animator.SetTrigger("Idle");

            //iTween.RotateBy(this.gameObject, iTween.Hash(
            //        "y", 90,
            //        "time", 0.75f,
            //        "easetype", iTween.EaseType.linear)

            //        );

            //回ってる時間
            yield return new WaitForSeconds(1.65f);

            transform.eulerAngles = new Vector3(0, 180, 0);
            base.animator.SetTrigger("Run");

            //後移動
            iTween.MoveTo(this.gameObject, iTween.Hash(
                    "position", transform.position + new Vector3(0, 0, -15),
                    "time", 1.0f,
                    "easetype", iTween.EaseType.easeInOutBack)

                    );

            //移動時間
            yield return new WaitForSeconds(0.95f);

            base.animator.SetTrigger("Idle");

            //iTween.RotateBy(this.gameObject, iTween.Hash(
            //        "y", 90,
            //        "time", 0.75f,
            //        "easetype", iTween.EaseType.linear)

            //        );

            //回ってる時間
            yield return new WaitForSeconds(1.65f);
        }
        
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

