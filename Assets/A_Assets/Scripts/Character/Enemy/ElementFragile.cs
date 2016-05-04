using UnityEngine;
using System.Collections;

public class ElementFragile : MonoBehaviour {

    /*


        (Serch) → Player → (Attack) → 距離10m以下 → 固有攻撃 　→  距離　→　yes → Serchへ
                               ↓                                20m以上
                           距離10m以上      →   ワープ回り込み →  Attackへ

    */

    //この辺は敵によりけりなのでこっち
    public GameObject[] Bullet;//攻撃
    public Transform[] Muzzle;//攻撃が出てくる場所

    public int Enemy_Level = 0;//この値によって攻撃方法を変える
    /*

        1:  Bubble
        2:  Crescent
        3:  Icicle

        */

    private Enemy_ControllerZ ecZ;
    private MoveSmooth MS;//移動は全部これ
    private Camera_ControllerZ CCZ;

    private Animator animator;//アニメーションはふよふよと攻撃モーションのみ
    private AudioSource[] SE = new AudioSource[2];//音
    private GameObject[] breakEff = new GameObject[3];//壊れるエフェクト

    //汎用
    private float time = 0;//使ったら戻す
    private Coroutine coroutine;//一度に動かすコルーチンは1つ ここでとっとけば止めるのが楽
    private bool isCoroutine = false;//コルーチンを止めるときにはfalseに戻すこと

    //向き取得用
    private Vector3 oldpos;//1フレーム前
    private Vector3 movedirection;


    private int priority = 0;//状態の優先度
    /*
    isReturn  1 こうしないとどっかにワープしてっちゃう

    Attack    2
    isDamage  3
    
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
    void Start()
    {

        ecZ = GetComponent<Enemy_ControllerZ>();
        MS = GetComponent<MoveSmooth>();
        animator = GetComponentInChildren<Animator>();
        SE[0] = GetComponent<AudioSource>();

        CCZ = Camera.main.gameObject.GetComponent<Camera_ControllerZ>();

        priority = 5;//最初はサーチに
    }

    // Update is called once per frame
    void Update()
    {

        //動いた距離を取得
        movedirection = oldpos - transform.position;

        if (ecZ.isReturn)
        {
            if (priority >= 1)
            {
                state = Element_State.Return;
                priority = 1;
            }

        }
        if (state == Element_State.Return)
        {

            if (priority >= 1)
            {
                priority = 1;
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

        if (state == Element_State.Attack)
        {
            priority = 2;

            //前を向ける
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.Player.transform.position - transform.position), 0.05f);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            ecZ.Stop();//こけた時まれに下方向に力が働くので一応止めとく
            coroutine = StartCoroutine(Attack());

        }

        if (ecZ.isDamage)
        {
            if (priority >= 3)
            {
                if (state != Element_State.Damage)
                {
                    state = Element_State.Damage;
                    priority = 3;
                    //前を向ける
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.Player.transform.position - transform.position), 0.05f);
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                    //アニメーションセット
                    animator.SetTrigger("Damage");//ダメージ
                }
            }

        }
        if (state == Element_State.Damage)
        {

            if (priority >= 6)
            {

                priority = 3;

                //アニメーションセット
                //animator.SetTrigger("Kyorokyoro");//ここできょろきょろ                  

                //前を向ける
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.Player.transform.position - transform.position), 0.05f);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            }
            else if (priority >= 3)
            {

                priority = 3;
                //アニメーションセット
                //animator.SetTrigger("Walk");//歩き

                //前を向ける
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ecZ.Player.transform.position - transform.position), 0.05f);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

                //カウンター
                coroutine = StartCoroutine(Attack());
                ecZ.Stop();//こけた時まれに下方向に力が働くので一応止めとく
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
                transform.rotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(-movedirection), 0.05f);
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
        oldstate = state;
        oldpos = transform.position;

    }
    
    IEnumerator Attack()
    {

        if (isCoroutine) yield break;
        isCoroutine = true;
        ecZ.Reverse_Magic();

        yield return new WaitForSeconds(1);//ちょっと間をおいてから攻撃

        GameObject[] bullet = new GameObject[Muzzle.Length];
        SE[0].enabled = true;//SE戻す

        yield return new WaitForSeconds(0.5f);//出すまで

        //効果音と演出
        if (!SE[0].isPlaying)
        {
            //ゴゴゴゴは始まるまでにラグがある
            SE[0].PlayOneShot(SE[0].clip);//SE

        }

        CCZ.flag_quake = true;//カメラ揺らす

        for (int i = 0; i < Muzzle.Length; i++)
        {
            bullet[i] = GameObject.Instantiate(Bullet[0]);//弾生成
            bullet[i].GetComponent<Attack_Parameter>().Parent = this.gameObject;//もらった親を渡しておく必要がある
        }
        
        
        //弾を飛ばす処理
        //足元を見る
        RaycastHit hit;
        GameObject hitObject;

        //0で判断する
        Vector3 LineStart = new Vector3(bullet[0].transform.position.x, bullet[0].transform.position.y + 4 /*- zcamera.Target.transform.localScale.y / 2*/, bullet[0].transform.position.z);
        Vector3 LineDirection = Vector3.down;//下でおｋ

        if (Physics.Raycast(LineStart, LineDirection, out hit, 200))
        {
            hitObject = hit.collider.gameObject;//レイヤーがIgnoreLayerのObjectは弾かれる。

            //Debug.DrawLine(LineStart, hit.point, Color.blue);
            //Debug.Log(hitObject);

            //地面だったら
            if (hitObject.gameObject.name == "Terrain")
            {
                for (int i = 0; i < Muzzle.Length; i++)
                {
                    bullet[i].transform.position = Muzzle[i].transform.position - new Vector3(0, Muzzle[i].transform.position.y - hit.point.y, 0) - new Vector3(0, 4, 0);

                }

            }
            else
            {
                for (int i = 0; i < Muzzle.Length; i++)
                {
                    bullet[i].transform.position = Muzzle[i].transform.position - new Vector3(0, Muzzle[i].transform.position.y - hit.point.y - hitObject.transform.localScale.y, 0) - new Vector3(0, 4, 0);
                }

                    
            }
        }
        else
        {
            for (int i = 0; i < Muzzle.Length; i++)
            {
                bullet[i].transform.position = Muzzle[i].transform.position - new Vector3(0, 4, 0);
            }
                
        }

        //必要なものを拾っとく
        GameObject[] effect = new GameObject[3];

        for (int i = 0; i < Muzzle.Length; i++)
        {
            bullet[i].transform.rotation = Quaternion.LookRotation(transform.TransformDirection(Vector3.forward).normalized);//回転させて弾頭を進行方向に向ける

            effect[i] = bullet[i].transform.FindChild("Spurt").gameObject;
            breakEff[i] = bullet[i].transform.FindChild("Break").gameObject;
            breakEff[i].SetActive(false);//いったん消しとく

        }

        SE[1] = bullet[0].GetComponent<AudioSource>();

        for (int i = 0; i < Muzzle.Length; i++)
        {
            bullet[i].GetComponent<Rigidbody>().velocity = (Vector3.up * bullet[i].GetComponent<Attack_Parameter>().speed);

        }

        yield return new WaitForSeconds(0.5f);//止める

        for (int i = 0; i < Muzzle.Length; i++)
        {
            bullet[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            bullet[i].GetComponent<Rigidbody>().isKinematic = true;//固定
            Destroy(effect[i], 1.3f);
            bullet[i].GetComponent<Collider>().enabled = false;
            bullet[i].transform.FindChild("Wall_Model").gameObject.GetComponent<Collider>().enabled = true;
            Invoke("SEStop", 1.3f);
            //bullet.GetComponent<MeshCollider>().isTrigger = false;

            Invoke("Break", bullet[i].GetComponent<Attack_Parameter>().GetA_Time() - 0.6f);//消えるちょい前に演出
            Destroy(bullet[i], bullet[i].GetComponent<Attack_Parameter>().GetA_Time());
        }
            

        yield return new WaitForSeconds(bullet[0].GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        ecZ.Reverse_Magic();

        state = Element_State.Search;//Search状態に戻す
        priority = 5;


        isCoroutine = false;

    }

    void SEStop()
    {
        SE[0].enabled = false;//SE止める
        CCZ.flag_quake = false;//揺れを止める
    }

    //壊れる演出
    void Break()
    {
        for (int i = 0; i < Muzzle.Length; i++)
        {
            breakEff[i].SetActive(true);//演出ON
        }
            
        //breakSE.enabled = true;//SEON
        SE[1].PlayOneShot(SE[1].clip);//SE

    }
}
