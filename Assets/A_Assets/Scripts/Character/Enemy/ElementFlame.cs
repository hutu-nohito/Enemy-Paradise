using UnityEngine;
using System.Collections;

public class ElementFlame : MonoBehaviour {

    /*


        (Serch) → Player → (Attack) → 距離10m以下 → 固有攻撃 　→  距離　→　yes → Serchへ
                               ↓                                 20m
                           距離10m以上      →       フレイム 　→　以上  →  no  → Attackへ

    */

    //この辺は敵によりけりなのでこっち
    public GameObject[] Bullet;//攻撃
    public Transform[] Muzzle;//攻撃が出てくる場所

    public int Enemy_Level = 0;//この値によって攻撃方法を変える
    /*

        1:  Bomb
        2:  Kanketusen
        3:  Meteor

        */

    private Enemy_ControllerZ ecZ;
    private MoveSmooth MS;//移動は全部これ
    private Camera_ControllerZ CCZ;

    private Animator animator;//アニメーションはふよふよとやられモーションのみ
    private AudioSource[] SE;//音

    //汎用
    private float time = 0;//使ったら戻す
    private Coroutine coroutine;//一度に動かすコルーチンは1つ ここでとっとけば止めるのが楽
    private bool isCoroutine = false;//コルーチンを止めるときにはfalseに戻すこと

    //向き取得用
    private Vector3 oldpos;//1フレーム前
    private Vector3 movedirection;

    private int priority = 0;//状態の優先度
    /*
    Attack    1
    isDamage  2
    isReturn  3
    isFind    4
    Search    5
    */

    //キャラクタ固有の状態
    //アニメーションは基本ここで管理するのでこれを使うときは向こうはIdleにでもしとく
    public enum Element_State
    {
        Attack,//攻撃
        Search,//うろうろ
        Damage,
        Return,
    }
    public Element_State state = Element_State.Search;
    public Element_State GetState() { return state; }
    public void SetState(Element_State state) { this.state = state; }

    private Element_State oldstate = Element_State.Search;

    // Use this for initialization
    void Start () {

        ecZ = GetComponent<Enemy_ControllerZ>();
        MS = GetComponent<MoveSmooth>();
        animator = GetComponentInChildren<Animator>();
        //SE = GetComponent<AudioSource>();

        CCZ = Camera.main.gameObject.GetComponent<Camera_ControllerZ>();

        priority = 5;//最初はサーチに

        oldpos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        //動いた距離を取得
        movedirection = oldpos - transform.position;

        if (state == Element_State.Attack)
        {
            priority = 1;

            //前を向ける
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.Player.transform.position - transform.position), 0.05f);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            MS.Stop();//止める

            coroutine = StartCoroutine(Attack());

        }

        if (ecZ.isDamage)
        {
            if (priority >= 2)
            {
                if(state != Element_State.Damage)
                {
                    state = Element_State.Damage;
                    priority = 2;
                    //前を向ける
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.Player.transform.position - transform.position), 0.05f);
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                    //アニメーションセット
                    animator.SetTrigger("Damage");//ダメージ
                    MS.Stop();//止める
                }

            }
            
        }
        if (state == Element_State.Damage)
        {
            
            if (priority >= 6)
            {

                priority = 2;

                //アニメーションセット
                //animator.SetTrigger("Damage");//ここできょろきょろ                  

                //前を向ける
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.Player.transform.position - transform.position), 0.05f);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                
            }
            else if (priority >= 2)
            {

                priority = 2;
                

                //前を向ける
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.Player.transform.position - transform.position), 0.05f);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

                //ダメージ受けたらだいたい死ぬが、生き残ったらとりあえず攻撃を激しくする
                coroutine = StartCoroutine(Attack());//反撃
                ecZ.Stop();//こけた時まれに下方向に力が働くので一応止めとく

            }
        }
        
        if (ecZ.isReturn)
        {
            if (priority >= 3)
            {
                state = Element_State.Return;
                priority = 3;
            }

        }
        if (state == Element_State.Return)
        {

            if (priority >= 3)
            {
                priority = 3;
                //アニメーションセット
                //animator.SetTrigger("Walk");//歩き

                //とりあえず中心へ(Territoryはワールド座標にしとく)
                ecZ.Move(ecZ.Territory.position, ecZ.speed);

                //前を向ける
                transform.rotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(ecZ.Territory.localPosition - transform.localPosition), 0.05f);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

                //テリトリとの距離で行動変化
                if ((ecZ.Territory.localPosition - transform.localPosition).magnitude < 10)//真ん中ら辺まで戻ったら
                {
                    state = Element_State.Search;
                    priority = 5;
                }

            }

        }
        
        if (ecZ.isFind)
        {
            if (priority >= 4)
            {
                state = Element_State.Attack;
            }
        }

        if (state == Element_State.Search)
        {
            if (priority >= 5)
            {
                priority = 5;
                //アニメーションセット
                //animator.SetTrigger("Walk");//歩き

                //前を向ける
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.move_controller.End - transform.localPosition), 0.05f);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-movedirection), 0.05f);

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
        oldpos = transform.position;

    }

    IEnumerator Attack()
    {

        if (isCoroutine) yield break;
        isCoroutine = true;
        ecZ.Reverse_Magic();

        yield return new WaitForSeconds(1);//ちょっと間をおいてから攻撃

        GameObject bullet;

        //アニメーションセット
        //animator.SetTrigger("Attack");//攻撃

        if ((ecZ.Player.transform.position - transform.position).magnitude > 20)//距離が10以上だったら
        {
            bullet = GameObject.Instantiate(Bullet[0]);//通常弾　flame
            bullet.GetComponent<Attack_Parameter>().Parent = this.gameObject;//誰が撃ったかを渡す
                                                                                                                                                                         //弾を飛ばす処理
            bullet.transform.position = Muzzle[0].position;//Muzzleの位置
            bullet.transform.rotation = Quaternion.LookRotation(ecZ.direction);//回転させて弾頭を進行方向に向ける

            bullet.GetComponent<Rigidbody>().velocity = (ecZ.Player.transform.position - transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed;//ﾌﾟﾚｲﾔに向けて撃つ

            Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

            //効果音と演出
            /*if (!SE[0].isPlaying)
            {

                SE[0].PlayOneShot(SE[0].clip);//SE

            }*/

            yield return new WaitForSeconds(bullet.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        }
        else
        {
            //Bomb
            if (Enemy_Level == 1)
            {

                bullet = GameObject.Instantiate(Bullet[1]);//弾生成
                bullet.GetComponent<Attack_Parameter>().Parent = this.gameObject;//もらった親を渡しておく必要がある

                //弾を飛ばす処理
                bullet.transform.position = Muzzle[1].position;//Muzzleの位置
                bullet.transform.rotation = Quaternion.LookRotation(ecZ.direction);//回転させて弾頭を進行方向に向ける
                
                bullet.GetComponent<Rigidbody>().velocity = ((ecZ.Player.transform.position - transform.position).normalized + transform.TransformDirection(new Vector3(0,1,-0.3f))) * bullet.GetComponent<Attack_Parameter>().speed;//キャラの向いてる方向

                //効果音と演出
                /*if (!SE[1].isPlaying)
                {

                    SE[1].PlayOneShot(SE[1].clip);//SE

                }*/

                Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

                yield return new WaitForSeconds(bullet.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

            }

            //Kanketusen
            if (Enemy_Level == 2)
            {

                Vector3 OldPlayerPos = ecZ.Player.transform.position;

                yield return new WaitForSeconds(Bullet[2].GetComponent<Attack_Parameter>().GetR_Time());//撃つ前の硬直

                bullet = GameObject.Instantiate(Bullet[2]);//弾生成

                bullet.GetComponent<Attack_Parameter>().Parent = this.gameObject;//もらった親を渡しておく必要がある

                //弾を飛ばす処理

                //足元を見る
                RaycastHit hit;
                GameObject hitObject;

                Vector3 LineStart = new Vector3(OldPlayerPos.x, OldPlayerPos.y, OldPlayerPos.z);
                Vector3 LineDirection = Vector3.down;//下でおｋ

                if (Physics.Raycast(LineStart, LineDirection, out hit, 50))
                {
                    hitObject = hit.collider.gameObject;//レイヤーがIgnoreLayerのObjectは弾かれる。

                    //Debug.DrawLine(LineStart, hit.point, Color.blue);
                    //Debug.Log(hitObject);

                    //地面だったら
                    if (hitObject.gameObject.name == "Terrain")
                    {

                        bullet.transform.position = hit.point;

                    }
                }
                else
                {
                    bullet.transform.position = new Vector3(OldPlayerPos.x, OldPlayerPos.y - ecZ.Player.transform.localScale.y / 2, OldPlayerPos.z);//下に地面がなかったら
                }

                bullet.transform.rotation = Quaternion.LookRotation(ecZ.direction);//回転させて弾頭を進行方向に向ける

                //効果音と演出
                /*if (!SE[2].isPlaying)
                {

                    SE[2].PlayOneShot(SE[2].clip);//SE

                }*/

                Destroy(bullet, bullet.GetComponent<Attack_Parameter>().GetA_Time());

                yield return new WaitForSeconds(0.5f);//撃った後の硬直
            }

            //Meteor
            if(Enemy_Level == 3)
            {
                //animator.SetTrigger("Shoot");

                yield return new WaitForSeconds(1);//撃つまでのため

                CCZ.flag_quake = true;//カメラ揺らす

                GameObject[] bulletM = new GameObject[5];

                for (int i = 0; i < 5; i++)
                {

                    bulletM[i] = GameObject.Instantiate(Bullet[3]);//弾生成

                    if (bulletM[i] != null)
                    {

                        bulletM[i].GetComponent<Attack_Parameter>().Parent = this.gameObject;//もらった親を渡しておく必要がある

                        bulletM[i].transform.position = Muzzle[3].position + transform.TransformDirection(new Vector3(Random.Range(-5, 6), Random.Range(0, 5), Random.Range(0, 15)));//ランダム
                        bulletM[i].transform.rotation = Quaternion.LookRotation(transform.TransformDirection(Vector3.forward));//回転させて弾頭を進行方向に向ける
                        bulletM[i].GetComponent<Rigidbody>().velocity = ((transform.TransformDirection(new Vector3(0, -4, 2))) * Bullet[3].GetComponent<Attack_Parameter>().speed);

                        Destroy(bulletM[i], Bullet[3].GetComponent<Attack_Parameter>().GetA_Time());

                    }


                }

                yield return new WaitForSeconds(Bullet[3].GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

                CCZ.flag_quake = false;//揺れを止める
            }
        }

        ecZ.Reverse_Magic();
        if ((ecZ.Player.transform.position - transform.position).magnitude > 30)//距離が20以上だったら
        {
            state = Element_State.Search;//Search状態に戻す
            priority = 5;
        }
            
        isCoroutine = false;

    }
}
