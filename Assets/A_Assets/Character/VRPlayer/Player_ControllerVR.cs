using UnityEngine;
using System.Collections;

public class Player_ControllerVR : Character_Parameters {
    /******************************************************************************/
    /** @brief VRプレイヤの操作管理
    * @date 2016/07/19
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    * 
*/
    /******************************************************************************/
    /* 更新履歴
    *　プレイヤーコントローラからいらない機能を削除(ジャンプ含む)
    */
    /******************************************************************************/
    
    //使うもの
    private CharacterController playerController;//キャラクタコントローラで動かす場合
    private Animator animator;//アニメーション設定用

    public GameObject MainCamera;//動かす用のカメラ

    //初期パラメタ(邪魔なのでインスペクタに表示しない)
    [System.NonSerialized]
    public int max_HP, max_MP, base_Pow, base_Def;
    //[System.NonSerialized]
    public float base_Sp, base_Ju;
    
    public float RotSpeed = 0.1f;//曲がる速さ
    
    void Start()
    {
        playerController = GetComponent<CharacterController>();//rigidbodyを使う場合は外す
        animator = GetComponentInChildren<Animator>();//アニメータを使うとき
        
        
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
            
        }

        //接地用(Character_Parametersに合わせて無駄に入れてる)
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

        //これをVive用に変更
        if (flag_move)
        {
            InputX = Input.GetAxis("Horizontal");
            InputY = Input.GetAxis("Vertical");
            inputDirection = new Vector3(InputX, 0, InputY);//入力された方向                

        }

        //キャラクタの方向回転(これもいらないかも)
        if (!GetF_Watch())//注目してたら回さない
        {
            if (flag_move)
            {
                //if (Input.GetKey(KeyCode.W))
                //{
                //    //Playerの方向 = (最初の方向,向けたい方向,向けたい速度)
                //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), RotSpeed);//Playerをターゲットのほうにゆっくり向ける
                //    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);//Playerのx,zの回転を直す。回転嫌い。全部Eulerにしてしまえばよい

                //}
                //if (Input.GetKey(KeyCode.S))
                //{
                //    //Playerの方向 = (最初の方向,向けたい方向,向けたい速度)
                //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MainCamera.transform.TransformDirection(Vector3.back)), RotSpeed);//Playerをターゲットのほうにゆっくり向ける
                //    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);//Playerのx,zの回転を直す。回転嫌い。全部Eulerにしてしまえばよい

                //}
                //if (Input.GetKey(KeyCode.A))
                //{
                //    //Playerの方向 = (最初の方向,向けたい方向,向けたい速度)
                //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MainCamera.transform.TransformDirection(Vector3.left)), RotSpeed);//Playerをターゲットのほうにゆっくり向ける
                //    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);//Playerのx,zの回転を直す。回転嫌い。全部Eulerにしてしまえばよい

                //}
                //if (Input.GetKey(KeyCode.D))
                //{
                //    //Playerの方向 = (最初の方向,向けたい方向,向けたい速度)
                //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MainCamera.transform.TransformDirection(Vector3.right)), RotSpeed);//Playerをターゲットのほうにゆっくり向ける
                //    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);//Playerのx,zの回転を直す。回転嫌い。全部Eulerにしてしまえばよい

                //}

            }


        }

        //カメラの方向を取得　それに合わせて動かす
        //direction = transform.TransformDirection(Vector3.forward);
        direction = MainCamera.transform.TransformDirection(Vector3.forward);
        
        //transform.Rotate(0, Quaternion.LookRotation(direction).eulerAngles.y - Quaternion.LookRotation(transform.TransformDirection(Vector3.forward)).eulerAngles.y, 0);
        transform.eulerAngles = new Vector3(0, Quaternion.LookRotation(direction).eulerAngles.y, 0);
        //transform.eulerAngles = new Vector3(0 , Quaternion.LookRotation(direction).eulerAngles.y, 0);
        //Debug.Log((Quaternion.LookRotation(direction).eulerAngles.y));
        //Debug.Log((transform.rotation.eulerAngles.y));
        //Debug.Log(Quaternion.AngleAxis(Quaternion.LookRotation(direction).y, Vector3.up));

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
        
        //キャラにかかる重力決定（少しふわふわさせてる）
        if (move_direction.y > -2)
        {

            move_direction.y += Physics.gravity.y * Time.deltaTime;

        }

        //キャラの移動実行（動かす方向＊動かすスピード＊補正）
        playerController.Move(move_direction * speed * Time.deltaTime);

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
}
