using UnityEngine;
using System.Collections;

public class Player_ControllerZ : Character_Parameters{

    /*
    プレイヤーの操作用
    Jump()//ジャンプ
    Move()//移動

    */

    //使うもの
    private CharacterController playerController;//キャラクタコントローラで動かす場合
    private Animator animator;//アニメーション設定用
	private Static save;

    public GameObject MainCamera;//動かす用のカメラ

    //初期パラメタ(邪魔なのでインスペクタに表示しない)
    [System.NonSerialized]
    public int max_HP, max_MP, base_Pow, base_Def;
    //[System.NonSerialized]
    public float base_Sp, base_Ju;

    //坂判定用
    private RaycastHit slideHit;
    public float slideSpeed = 0.1f;//滑るスピード
    private bool isSliding = false;//滑ってるかどうか

    //ダッシュ
    private bool flag_Dash = false;//ダッシュ待機
    private bool isDash = false;//ダッシュする
    public bool Dash = true;//ダッシュ中
    public float inputTime = 0.9f;//入力受付時間
    //private int inputCount = 0;//入力は一回まで
    private string inputKey = "W";//入力(どの方向か)
    private float elapsedTime = 0.0f;

    public float RotSpeed = 0.1f;//曲がる速さ

    //デバッグ用(デバックしたいときはこれにチェック)
    public bool isDemo = false;

    void Start()
    {
        playerController = GetComponent<CharacterController>();//rigidbodyを使う場合は外す
        animator = GetComponentInChildren<Animator>();//アニメータを使うとき

        if (!isDemo)
        {
            save = GameObject.FindGameObjectWithTag("Manager").GetComponent<Static>();
        }
		
        //メインカメラが設定されてなかったらする
        if (MainCamera == null) MainCamera = Camera.main.gameObject;

        //HPとMPの引継ぎ
        if (!isDemo)
        {
            H_point = save.H_Point;
            M_point = save.M_Point;
        }
		
        //初期パラメタを保存
        max_HP = H_point;
        max_MP = M_point;
        base_Pow = power;
        base_Def = def;
        base_Sp = speed;
        base_Ju = jump;

    }

    void Update()
    {
        //HPがなくなった時の処理
        if (H_point <= 0)
        {
            if (isDemo)
            {

                //デバッグ用なので満タンに戻す
                H_point = max_HP;

            }
            else
            {

                save.H_Point = max_HP;//HPは満タンに
                GameObject.FindGameObjectWithTag("Manager").GetComponent<QuestManager>().Questfailure();

            }
        }

        //接地用
        if (playerController.isGrounded)
        {
            flag_ground = true;
        }
        else
        {
            flag_ground = false;
        }


        //アニメーションリセット///////////////////////////////////////////////////////////
        move_direction = new Vector3(0.0f, move_direction.y, 0.0f);
        //キーボードからの入力読み込み/////////////////////////////////////////////////////

        float InputX = 0.0f;
        float InputY = 0.0f;

        Vector3 inputDirection = Vector3.zero;//入力された方向

        if (flag_move)
        {
                InputX = Input.GetAxis("Horizontal");
                InputY = Input.GetAxis("Vertical");
                inputDirection = new Vector3(InputX, 0, InputY);//入力された方向                

        }

        //ジャンプ
        if (playerController.isGrounded)
        {
            if (flag_move)
            {
                if(!isSliding) flag_jump = true;//動けるときだけ地上にいたら。
            }

            move_direction = Vector3.zero;
            animator.SetBool("Jump", false);
            if (Input.GetButtonDown("Jump"))
            {
                if (flag_jump) { Jump(); }
            }

        }

        //キャラクタの方向回転
        if (!GetF_Watch())//注目してたら回さない
        {
            if (flag_move)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    //Playerの方向 = (最初の方向,向けたい方向,向けたい速度)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), RotSpeed);//Playerをターゲットのほうにゆっくり向ける
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);//Playerのx,zの回転を直す。回転嫌い。全部Eulerにしてしまえばよい

                }
                if (Input.GetKey(KeyCode.S))
                {
                    //Playerの方向 = (最初の方向,向けたい方向,向けたい速度)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MainCamera.transform.TransformDirection(Vector3.back)), RotSpeed);//Playerをターゲットのほうにゆっくり向ける
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);//Playerのx,zの回転を直す。回転嫌い。全部Eulerにしてしまえばよい

                }
                if (Input.GetKey(KeyCode.A))
                {
                    //Playerの方向 = (最初の方向,向けたい方向,向けたい速度)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MainCamera.transform.TransformDirection(Vector3.left)), RotSpeed);//Playerをターゲットのほうにゆっくり向ける
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);//Playerのx,zの回転を直す。回転嫌い。全部Eulerにしてしまえばよい

                }
                if (Input.GetKey(KeyCode.D))
                {
                    //Playerの方向 = (最初の方向,向けたい方向,向けたい速度)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MainCamera.transform.TransformDirection(Vector3.right)), RotSpeed);//Playerをターゲットのほうにゆっくり向ける
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);//Playerのx,zの回転を直す。回転嫌い。全部Eulerにしてしまえばよい

                }
            }
            
            
        }

        //カメラの方向を取得　それに合わせて動かす
        //direction = transform.TransformDirection(Vector3.forward);
        direction = MainCamera.transform.TransformDirection(Vector3.forward);

        //キャラクタ移動処理
        if (inputDirection.magnitude > 0.1)
        {

            _Move();
            animator.SetFloat("Speed", move_direction.magnitude);

        }
        else
        {

            animator.SetFloat("Speed", 0);

        }

        //ダッシュ////////////////////////////////
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    if (!flag_Dash)
        //    {
        //        inputKey = "W";
        //    }                
        //    flag_Dash = true;
        //}
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    if (!flag_Dash)
        //        inputKey = "A";
        //    flag_Dash = true;
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    if (!flag_Dash)
        //        inputKey = "S";
        //    flag_Dash = true;
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    if (!flag_Dash)
        //        inputKey = "D";
        //    flag_Dash = true;
        //}

        //if (Dash)
        //{
        //    if (flag_Dash)
        //    {
        //        elapsedTime += Time.deltaTime;
        //        if (elapsedTime > inputTime)
        //        {
        //            flag_Dash = false;
        //            elapsedTime = 0;
        //        }

        //        switch (inputKey)
        //        {
        //            case "W":
        //                if (Input.GetKeyUp(KeyCode.W))//押しっぱなしで発動しないよう
        //                {
        //                    isDash = true;
        //                }
        //                if (isDash)
        //                {
        //                    if (Input.GetKeyDown(KeyCode.W))//
        //                    {
        //                        speed = 2 * base_Sp;
        //                        elapsedTime = 0;
        //                        isDash = false;
        //                        flag_Dash = false;
        //                    }
        //                }
        //                break;
        //            case "A":
        //                if (Input.GetKeyUp(KeyCode.A))
        //                {
        //                    isDash = true;
        //                }
        //                if (isDash)
        //                {
        //                    if (Input.GetKeyDown(KeyCode.A))//
        //                    {
        //                        speed = 2 * base_Sp;
        //                        elapsedTime = 0;
        //                        isDash = false;
        //                        flag_Dash = false;
        //                    }
        //                }
        //                break;
        //            case "S":
        //                if (Input.GetKeyUp(KeyCode.S))
        //                {
        //                    isDash = true;
        //                }
        //                if (isDash)
        //                {
        //                    if (Input.GetKeyDown(KeyCode.S))//
        //                    {
        //                        speed = 2 * base_Sp;
        //                        elapsedTime = 0;
        //                        isDash = false;
        //                        flag_Dash = false;
        //                    }
        //                }
        //                break;
        //            case "D":
        //                if (Input.GetKeyUp(KeyCode.D))
        //                {
        //                    isDash = true;
        //                }
        //                if (isDash)
        //                {
        //                    if (Input.GetKeyDown(KeyCode.D))//
        //                    {
        //                        speed = 2 * base_Sp;
        //                        elapsedTime = 0;
        //                        isDash = false;
        //                        flag_Dash = false;
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //}
        

        if (inputDirection.magnitude == 0)//ダッシュ解除
        {
            speed = base_Sp;
            flag_Dash = false;
        }

        //坂に立ってたら滑らす
        if (isSliding)
        {//滑るフラグが立ってたら
            Vector3 hitNormal = slideHit.normal;
            move_direction.x = hitNormal.x * slideSpeed;
            move_direction.z = hitNormal.z * slideSpeed;
            isSliding = false;//ここでリセットしとく
        }

        //キャラにかかる重力決定（少しふわふわさせてる）
        if (move_direction.y > -2)
        {
                        
            move_direction.y += Physics.gravity.y * Time.deltaTime;     

        }

        //キャラの移動実行（動かす方向＊動かすスピード＊補正）
        playerController.Move(move_direction * speed * Time.deltaTime);

    }

    //ジャンプ
    void Jump()
    {
        flag_jump = false;

        move_direction.y = jump;
        animator.SetBool("Jump", true);

    }

    //実際に動かす方向決定
    void _Move()
    {

        if (Input.GetKey(KeyCode.W))
        {
            move_direction.x += direction.x;
            move_direction.z += direction.z;

        }
        if (Input.GetKey(KeyCode.S))
        {
            move_direction.x -= direction.x;
            move_direction.z -= direction.z;
        }
        if (Input.GetKey(KeyCode.A))
        {
            move_direction.x -= direction.z;
            move_direction.z += direction.x;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move_direction.x += direction.z;
            move_direction.z -= direction.x;
        }
    }

    //地面に立ってないとき下が坂じゃないか確認
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!playerController.isGrounded)
        {
            //キャラクターの位置から下方向にRayを飛ばす（指定レイヤー限定※この場合は地面コリジョンのレイヤー）
            //RayLengthは、Rayを飛ばす距離。私の場合は地面の位置すれすれまで飛ばしてます（地面の高さは固定な前提）
            //レイヤーマスクは⇒で指定 int layerMask =1 << LayerMask.NameToLayer("レイヤー名");
            if (Physics.Raycast(transform.position, Vector3.down, out slideHit, 10))
            {
                //衝突した際の面の角度とが滑らせたい角度以上かどうかを調べます。
                if (Vector3.Angle(slideHit.normal, Vector3.up) > playerController.slopeLimit)
                {
                    isSliding = true;
                }
            }
            
        }
    }
}
