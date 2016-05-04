using UnityEngine;
using System.Collections;

public class DarkLilith : MonoBehaviour {

    /*

        ここにダークリリスの行動を記述
        歩く→ﾌﾟﾚｲﾔとの位置によって方向を変える？
        　　→ﾌﾟﾚｲﾔとの位置によってWaveとWallを使い分ける

		道もここに記述
    */

    
    private GameObject Player;//プレイヤーは常に持たせとく
	private Animator animator;
    private Enemy_ControllerZ ecZ;
    private Block_Controller Block_C;

	public GameObject Block;//道用ブロック
    public GameObject BlockBullet;//攻撃用ブロック
    public Transform[] BlockSpawner;//0が左、1が右
    public Transform[] Places;//移動する場所　ここに移動
    private Transform CurrentPlace;//今いる場所
    private Transform NextPlace;//次の場所

    //ボスは独自の状態を持つ
    public enum Enemy_State
    {
        Idle,//通常
        Walk,//歩き
        Wave,//Wave使用中
        Wall,//Wall使用中
        Shot//ブロックを打ち出す
    }
    public Enemy_State state = Enemy_State.Idle;

    public float ThinkTime = 5;//思考速度

    private bool isCoroutine;//コルーチン稼働中　使ったら戻す
    private Coroutine coroutine;

    // Use this for initialization
    void Start () {

        Player = GameObject.FindGameObjectWithTag("Player");
		animator = GetComponentInChildren<Animator>();
        ecZ = GetComponent<Enemy_ControllerZ>();
        Block_C = GameObject.FindObjectOfType<Block_Controller>();
        CurrentPlace = this.transform;

		coroutine = StartCoroutine ("Road");

    }
	
	// Update is called once per frame
	void Update () {

        switch (state)
        {
            case Enemy_State.Idle:
                StartCoroutine(Idle());
                break;
            case Enemy_State.Walk:
                StartCoroutine(Walk());
                break;
            case Enemy_State.Wall:
                StartCoroutine(Wall());
                break;
            case Enemy_State.Wave:
				StartCoroutine(Wave());
                break;
            case Enemy_State.Shot:
                StartCoroutine(Shot());
                break;

        }

		//プレイヤーのほうを向ける(Lerp系はUpdate内で使わないとだめっぽい)
		if (state == Enemy_State.Walk) {
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (NextPlace.position - transform.position), 0.1f);
			transform.rotation = Quaternion.Euler (0, transform.eulerAngles.y, 0);
		} else {
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (Player.transform.position - transform.position), 0.01f);
			transform.rotation = Quaternion.Euler (0, transform.eulerAngles.y, 0);
		}

        //HPが10以下になったら思考速度を早くしてみる。
        if (ecZ.GetHP() < 10)
        {
        }

        if (ecZ.GetHP() < 1)
        {
            Destroy(this.gameObject);
        }

    }

    //プレイヤーとの距離を返す。
    float CalLength()
    {
        float length = 0;

        length = (Player.transform.position - transform.position).magnitude;

        return length;
    }

    IEnumerator Idle()
    {
        //コルーチンは一つずつ動かす
        if (isCoroutine) yield break;
        isCoroutine = true;

        //次の場所を決める
		NextPlace = Places [Random.Range (0, 3)];

        yield return new WaitForSeconds(ThinkTime);//思考間隔かもしれない

        //次の行動を決める
        int action = Random.Range(0, 2);
        //int action = 2;
        Debug.Log(action);
        switch (action)
        {
            case 0:
                state = Enemy_State.Walk;
                break;
            case 1:
                state = Enemy_State.Wall;
                break;
            case 2:
                state = Enemy_State.Wave;
                break;


        }

        //おんなじ場所が選ばれたら攻撃
        if (NextPlace == CurrentPlace)
        {
            //距離によって攻撃方法が変わる
            if (state == Enemy_State.Walk) { state = Enemy_State.Shot; }

        }
        /*if (NextPlace == CurrentPlace) {
			//距離によって攻撃方法が変わる
			if (CalLength() < 20) { state = Enemy_State.Wave; }
			else { state = Enemy_State.Wall; }
		}
		else//動けるなら動く
		{
			state = Enemy_State.Walk;
		}*/

        isCoroutine = false;
		
	}
	
	IEnumerator Walk()
	{
		if (isCoroutine) yield break;
        isCoroutine = true;

        GetComponent<MoveSmooth>().Move(NextPlace.transform.position, ecZ.GetSpeed());
		animator.SetBool ("Walk",true);

        yield return new WaitForSeconds(ThinkTime - 1);//思考間隔かもしれない

        state = Enemy_State.Idle;//ブロックの生成を止める

        yield return new WaitForSeconds(1);//止めた後

        CurrentPlace = NextPlace;
		animator.SetBool ("Walk",false);
        isCoroutine = false;


    }

    IEnumerator Shot()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;
        animator.SetInteger("Attack", 1);

        GameObject bullet;

        bullet = GameObject.Instantiate(BlockBullet);
        bullet.GetComponent<Attack_Parameter>().Parent = this.gameObject;//もらった親を渡しておく必要がある

        //弾を飛ばす処理
        bullet.transform.position = transform.position + transform.TransformDirection(Vector3.forward) * 5;
        bullet.GetComponent<Rigidbody>().velocity = ((Player.transform.position - transform.position).normalized * bullet.GetComponent<Attack_Parameter>().speed);
        /*if(!audioSource.isPlaying){
			
            audioSource.Play();
			
        }*/
        yield return new WaitForSeconds(ThinkTime);//思考間隔かもしれない

        Destroy(bullet);
        state = Enemy_State.Idle;
        animator.SetInteger("Attack", 0);
        isCoroutine = false;

    }

    IEnumerator Wall()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        Block_C.StartWall();
		animator.SetInteger ("Attack",1);

        yield return new WaitForSeconds(ThinkTime - 2);//思考間隔かもしれない

        state = Enemy_State.Shot;

        yield return new WaitForSeconds(2);

        animator.SetInteger ("Attack",0);
        isCoroutine = false;
    }

    IEnumerator Wave()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;

        Block_C.StartWave();
		animator.SetInteger ("Attack",1);

        yield return new WaitForSeconds(ThinkTime);//思考間隔かもしれない

        state = Enemy_State.Idle;
		animator.SetInteger ("Attack",0);
        isCoroutine = false;
    }

	IEnumerator Road (){

		//Startで開始してずっとやってる
		//右と左で生成タイミングをずらす
		if (state == Enemy_State.Walk) {

			Instantiate (Block, BlockSpawner [0].position, Quaternion.identity);

			yield return new WaitForSeconds (0.3f);

			Instantiate (Block, BlockSpawner [1].position, Quaternion.identity);

        }

        yield return StartCoroutine("RoadManage");

    }

    IEnumerator RoadManage()
    {

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine("Road");

    }

}
