using UnityEngine;
using System.Collections;

public class FatBat : MonoBehaviour {

    /*
        (Serch) → Player → (威嚇) → 距離10m以下 → (踊る) → HP減る → (攻撃) 
                               ↓                       ↓                  ↓
                           距離10m以上              距離10m以上
                            　 ↓                       ↓
          ↑                   ←                    がっかり               ←

    */

    //この辺は敵によりけりなのでこっち
    public GameObject Bullet;//攻撃
    public Transform Muzzle;//攻撃が出てくる場所

    private Enemy_ControllerZ ecZ;
    private MoveSmooth MS;//移動は全部これ

    private Animator animator;
    private int animState = 0;//アニメータのパラメタが取得できないのでとりあえずこれで代用
    /*
    アニメーションの番号割り振り

        1   Walk
        2   Ikaku
        3   Dance
        4   Syobon
        5   Attack
        6   Kyorokyoro

    */   

    //汎用
    private float time = 0;//使ったら戻す
    private Coroutine coroutine;//一度に動かすコルーチンは1つ ここでとっとけば止めるのが楽
    private bool isCoroutine = false;//コルーチンを止めるときにはfalseに戻すこと


    private int priority = 0;//状態の優先度
    /*
    Attack    1
    isDamage  2
    isReturn  3
    Ikaku     4
    Dance     4
    Syobon    4
    isFind    5
    Search    6
    */

    //キャラクタ固有の状態
    //アニメーションは基本ここで管理するのでこれを使うときは向こうはIdleにでもしとく
    public enum Fatbat_State
    {
        Ikaku,//プレイヤを見つけて近づいてる(臨戦態勢)
        Dance,//踊る
        Syobon,//がっかり
        Attack,//攻撃
        Search,//うろうろ
        Damage,
        Return,
    }
    public Fatbat_State state = Fatbat_State.Search;
    public Fatbat_State GetState() { return state; }
    public void SetState(Fatbat_State state) { this.state = state; }

    private Fatbat_State oldstate = Fatbat_State.Search;

    // Use this for initialization
    void Start()
    {

        ecZ = GetComponent<Enemy_ControllerZ>();
        MS = GetComponent<MoveSmooth>();
        animator = GetComponentInChildren<Animator>();
        

        priority = 6;//最初はサーチに

        //Player = GameObject.FindWithTag("Player").transform;

    }

    // Update is called once per frame
    void Update()
    {
        //アニメーションを取得してみる
        AnimatorStateInfo anim = animator.GetCurrentAnimatorStateInfo(0);

        if (state == Fatbat_State.Attack)
        {
            priority = 1;

            //前を向ける
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.Player.transform.position - transform.position), 0.05f);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            
            coroutine = StartCoroutine(Attack());

        }

        if (ecZ.isDamage)
        {
            if (priority >= 2)
            {
                state = Fatbat_State.Damage;
                priority = 2;
            }
                
        }
        if (state == Fatbat_State.Damage)
        {
            
            if (priority >= 6)
            {

                priority = 2;

                //アニメーションセット
                if(animState != 6)
                {
                    animator.SetTrigger("Kyorokyoro");//ここできょろきょろ
                    animState = 6;
                }
                   

                //前を向ける
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.Player.transform.position - transform.position), 0.05f);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                

            }
            else if(priority >= 2)
            {

                priority = 2;
                //アニメーションセット
                if (animState != 1)
                {
                    animator.SetTrigger("Walk");//歩き
                    animState = 1;
                }

                //前を向ける
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.Player.transform.position - transform.position), 0.05f);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                

                ecZ.Move(ecZ.Player.transform.position, ecZ.speed);//Playerに近づく

                //プレイヤとの距離で行動変化
                if ((ecZ.Player.transform.position - transform.position).magnitude < 5)//距離が5以下だったら
                {
                    state = Fatbat_State.Attack;
                    priority = 1;
                }
                if ((ecZ.Player.transform.position - transform.position).magnitude > 20)//距離が10以上だったら
                {
                    state = Fatbat_State.Syobon;
                    priority = 4;
                }
            }

            //ダンスしてるとき=プレーヤに十分近づいてるので攻撃
            if(state == Fatbat_State.Dance)
            {
                state = Fatbat_State.Attack;
                priority = 1;
            }
        }

        if (ecZ.isReturn)
        {
            if (priority >= 3)
            {
                state = Fatbat_State.Return;
                priority = 3;
            }
                
        }
        if (state == Fatbat_State.Return)
        {

            if (priority >= 3)
            {
                priority = 3;
                //アニメーションセット
                if (animState != 1)
                {
                    animator.SetTrigger("Walk");//歩き
                    animState = 1;
                }

                //とりあえず中心へ(Territoryはワールド座標にしとく)
                ecZ.Move(ecZ.Territory.position, ecZ.speed);

                //前を向ける
                transform.rotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(ecZ.Territory.localPosition - transform.localPosition), 0.05f);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

                //テリトリとの距離で行動変化
                if ((ecZ.Territory.localPosition - transform.localPosition).magnitude < 5)//真ん中ら辺まで戻ったら
                {
                    state = Fatbat_State.Search;
                    priority = 6;
                }

            }
                        
        }

        if (state == Fatbat_State.Ikaku)
        {
            if (priority >= 4)
            {
                //ちょっと間をおいてから行動
                time += Time.deltaTime;

                if (time < 3)
                {

                    priority = 4;
                    //アニメーションセット
                    if (animState != 2)
                    {
                        animator.SetTrigger("Ikaku");//威嚇
                        animState = 2;
                    }

                    //前を向ける
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.Player.transform.position - transform.position), 0.05f);
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

                }
                else
                { 


                    //プレイヤとの距離で行動変化
                    if ((ecZ.Player.transform.position - transform.position).magnitude < 5)//距離が5以下だったら
                    {
                        state = Fatbat_State.Dance;
                        priority = 4;

                    }
                    else if ((ecZ.Player.transform.position - transform.position).magnitude < 20)//距離が20以下だったら
                    {
                        ecZ.Move(ecZ.Player.transform.position, ecZ.speed);//Playerに近づく
                        if (animState != 1)
                        {
                            animator.SetTrigger("Walk");//威嚇
                            animState = 1;
                        }
                        
                    }
                    else
                    {
                        state = Fatbat_State.Syobon;
                        priority = 4;
                    }
                }
                

            }      
        }

        if (state == Fatbat_State.Dance)
        {
            if (priority >= 4)
            {
                priority = 4;
                //アニメーションセット
                if (animState != 3)
                {
                    animator.SetTrigger("Dance");//ダンス
                    animState = 3;
                }

                //前を向ける
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.Player.transform.position - transform.position), 0.05f);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

                //MPを奪う
                coroutine = StartCoroutine(Dance());
                

                if ((ecZ.Player.transform.position - transform.position).magnitude > 20)
                {
                    state = Fatbat_State.Syobon;
                    priority = 4;
                    StopCoroutine(Dance());//一応止めとく
                    isCoroutine = false;
                }
            }
        }

        if (state == Fatbat_State.Syobon)
        {
            if (priority >= 4)
            {
                priority = 4;
                //アニメーションセット
                if (animState != 6)
                {
                    animator.SetTrigger("Kyorokyoro");//きょろきょろ
                    animState = 6;
                }

                //とりあえず中心へ(Territoryはワールド座標にしとく)
                ecZ.Move(ecZ.Territory.position, ecZ.speed);

                //前を向ける
                transform.rotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(ecZ.Territory.localPosition - transform.localPosition), 0.05f);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

                time += Time.deltaTime;

                //テリトリとの距離で行動変化
                if ((ecZ.Territory.localPosition - transform.localPosition).magnitude < 5)//真ん中ら辺まで戻ったら
                {
                    ecZ.Stop();//止める
                }

                //5秒くらい落ち込んでてもらう
                if (time > 5)
                {
                    state = Fatbat_State.Search;
                    priority = 6;
                    time = 0;
                }

                
            }

        }

        if (ecZ.isFind)
        {
            if (priority >= 5)
            {
                priority = 5;

                state = Fatbat_State.Ikaku;
                priority = 4;

            }         
        }

        if (state == Fatbat_State.Search)
        {
            if (priority >= 6)
            {
                priority = 6;
                //アニメーションセット
                if (animState != 1)
                {
                    animator.SetTrigger("Walk");//歩き
                    animState = 1;
                }

                //前を向ける
                transform.rotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(ecZ.move_controller.End - transform.localPosition), 0.05f);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

                MS.Move(ecZ.move_controller.End, ecZ.speed);              

            }
                
        }

        //状態が変化したら前の状態のいどうは中断
        if (oldstate != state)
        {
            time = 0;
            MS.Stop();
            ecZ.Stop();
        }
        oldstate = state;

    }

    /////////////////////////////////////
    IEnumerator Attack()
    {

        if (isCoroutine) yield break;
        isCoroutine = true;
        ecZ.Reverse_Magic();

        yield return new WaitForSeconds(0.1f);//ちょっと間をおいてから攻撃

        GameObject bullet;

        //アニメーションセット
        if (animState != 5)
        {
            animator.SetTrigger("Attack");//攻撃
            animState = 5;
        }

        ecZ.Move(ecZ.Player.transform.position - (ecZ.Player.transform.position - transform.position).normalized * 2, ecZ.speed * 5);//Playerの手前で止まる

        yield return new WaitForSeconds(0.2f);//近づききってから攻撃

        bullet = GameObject.Instantiate(Bullet);
        bullet.transform.FindChild("Armature").gameObject.transform.FindChild("Bone").gameObject.GetComponentInChildren<Attack_Parameter>().Parent = this.gameObject;//誰が撃ったかを渡す

        //弾を飛ばす処理
        bullet.transform.position = Muzzle.position + (ecZ.direction);
        bullet.transform.rotation = Quaternion.LookRotation(ecZ.direction);//回転させて弾頭を進行方向に向ける
        //bullet.GetComponent<Rigidbody>().velocity = ((ecZ.Player.transform.position - this.transform.position).normalized * at_para.speed);

        /*if(!audioSource.isPlaying){
			
			audioSource.Play();
			
		}*/

        Destroy(bullet, bullet.transform.FindChild("Armature").gameObject.transform.FindChild("Bone").gameObject.GetComponentInChildren<Attack_Parameter>().GetA_Time());

        yield return new WaitForSeconds(1);

        //アニメーションセット
        if (animState != 4)
        {
            animator.SetTrigger("Syobon");//ここで落ち込ませる
            animState = 4;
        }

        yield return new WaitForSeconds(4);//落ち込み待ち
        ecZ.Reverse_Magic();
        state = Fatbat_State.Syobon;
        priority = 4;
        isCoroutine = false;

    }

    IEnumerator Dance()
    {//MPを削る

        if (isCoroutine) { yield break; }
        isCoroutine = true;

        yield return new WaitForSeconds(1);//奪うスピード

        if (ecZ.Player.GetComponent<Player_ControllerZ>().GetMP() > 0)//マイナスにならないようにしとく
        ecZ.Player.GetComponent<Player_ControllerZ>().SetMP(ecZ.Player.GetComponent<Player_ControllerZ>().GetMP() - 3);

        isCoroutine = false;

    }
}
