using UnityEngine;
using System.Collections;

public class Camera_ControllerZ : MonoBehaviour {

    //カメラの操作全般　ボタン操作含む

        /// <summary>
        /// 障害物を避けるとカメラがあらぶる(テラインに限定)
        /// </summary>
    public float distance = 5.0f;//変えるとプレイヤとの距離が変わる
    public float horizontalAngle = 0.0f;
    public float rotAngle = 180.0f;//画像の横幅分カーソルを移動させたとき何度回転するか

    public float verticalAngle = 10.0f;
    public GameObject lookTarget;
    public Vector3 Offset = Vector3.zero;//これで位置調整

    private Player_ControllerZ pcZ;
    private Z_Camera Zcamara;

    private bool flag_TopView = false;
    private float T_distance = 0;//Tを押したときのdistance

    private bool is_Q_Move = false;//回転用

    //演出
    //揺らすよう
    public bool flag_quake;
    public Vector3 QuakeMagnitude = new Vector3(0,0.2f,0);
    private float quaketime = 0;
    private float elapsedquakeTime = 0.05f;

    void Start()
    {
        //Playerをセットし忘れてたら探す
        if (lookTarget == null) lookTarget = GameObject.FindGameObjectWithTag("Player");

        pcZ = lookTarget.GetComponent<Player_ControllerZ>();
        Zcamara = GetComponent<Z_Camera>();

        //現在の向きから割り出さないとだめ
        //horizontalAngle = lookTarget.transform.eulerAngles.y;
        verticalAngle = 0.0f;
        elapsedTime = 0;
        EndPos = lookTarget.transform.eulerAngles.y;
        StartPos = horizontalAngle;
        time = 0.5f;
        deltaPos = (EndPos - StartPos) / time;
        deltaPos = Mathf.Repeat(deltaPos, 360.0f / time);//360進数に直す

        is_Q_Move = true;
    }

    void LateUpdate()
    {
        //ドラック入力でカメラのアングルを更新する
        if (pcZ.GetF_Move())//動けるときだけカメラを操作できるようにしとく
        {
            float anglePerPixel = rotAngle / (float)Screen.width;
            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            horizontalAngle += delta.x * anglePerPixel;
            horizontalAngle = Mathf.Repeat(horizontalAngle, 360.0f);
            verticalAngle -= delta.y * anglePerPixel;
            verticalAngle = Mathf.Clamp(verticalAngle, -60.0f, 60.0f);

        }

        //カメラの位置と回転を更新する
        if (lookTarget != null)
        {
            Vector3 lookPosition = lookTarget.transform.position + Offset;

            //注視対象からの相対位置を求める
            Vector3 relativePos = Quaternion.Euler(verticalAngle, horizontalAngle, 0) * new Vector3(0, 0, -distance);

            if (flag_quake)
            {
                //注視対象の位置にオフセット加算した位置に移動させる
                transform.position = lookPosition + relativePos + QuakeMagnitude;
            }
            else
            {
                //注視対象の位置にオフセット加算した位置に移動させる
                transform.position = lookPosition + relativePos;
            }
            

            //注視対象を注視させる
            if(!pcZ.GetF_Watch())//注目してなければ
            transform.LookAt(lookPosition);

            //注目時にカメラをプレイヤの後ろに
            if (pcZ.GetF_Watch())
            {
                if(lookTarget != null)
                {
                    Vector2 length = new Vector2(Zcamara.Target.transform.position.x - lookPosition.x, Zcamara.Target.transform.position.z - lookPosition.z);



                    if (length.magnitude > 5)
                    {
                        /*if (Mathf.DeltaAngle(horizontalAngle, lookTarget.transform.eulerAngles.y) < -0.1f)
                        {
                            horizontalAngle += (lookTarget.transform.eulerAngles.y - horizontalAngle) * Time.deltaTime;
                        }*/


                        elapsedTime = 0;
                        EndPos = lookTarget.transform.eulerAngles.y;
                        StartPos = horizontalAngle;
                        time = 0.3f;
                        deltaPos = (EndPos - StartPos) / time;
                        deltaPos = Mathf.Repeat(deltaPos, 360.0f / time);//360進数に直す

                        is_Q_Move = true;

                        /*if (horizontalAngle != lookTarget.transform.eulerAngles.y)
                        horizontalAngle += (horizontalAngle - lookTarget.transform.eulerAngles.y) * Time.deltaTime;*/

                        //transform.LookAt(lookPosition);
                        transform.LookAt(new Vector3((lookPosition.x + Zcamara.Target.transform.position.x) / 2,
                                                     (lookPosition.y + Zcamara.Target.transform.position.y) / 2,
                                                     (lookPosition.z + Zcamara.Target.transform.position.z) / 2));//敵とLookPositionの中間点を見せてみる

                        //距離が離れたらカメラを引くべき
                        
                    }
                    else//距離が近かったらカメラを回さない
                    {
                        transform.LookAt(lookPosition);
                    }
                }

            }

            //揺らすよう
            if (flag_quake)
            {
                quaketime += Time.deltaTime;
                if (quaketime > elapsedquakeTime)
                {
                    QuakeMagnitude = -QuakeMagnitude;
                    quaketime = 0;
                }
            }

            //障害物を避ける
            RaycastHit hitInfo;
            if(Physics.Linecast(lookPosition,transform.position,out hitInfo))
            {
                //if(hitInfo.transform.gameObject.tag != "Enemy" && hitInfo.transform.gameObject.tag != "Bullet")
                if(hitInfo.transform.gameObject.name == "Terrain")
                {

                    transform.position = hitInfo.point;
                }


            }
        }

        //ボタン操作系///////////////////////////////////////////////////////////////////
        //カメラを後ろからに戻す 回転リセットだけで勝手にやってくれそう
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //現在の向きから割り出さないとだめ
            //horizontalAngle = lookTarget.transform.eulerAngles.y;
            verticalAngle = 0.0f;
            elapsedTime = 0;
            EndPos = lookTarget.transform.eulerAngles.y;
            StartPos = horizontalAngle;
            time = 0.5f;
            deltaPos = (EndPos - StartPos) / time;
            deltaPos = Mathf.Repeat(deltaPos, 360.0f / time);//360進数に直す

            is_Q_Move = true;

        }
        if (!pcZ.GetF_Watch())//注目してないときに右クリックでカメラを正面に向ける
        {
            if (Input.GetMouseButtonDown(1))
            {
                //現在の向きから割り出さないとだめ
                //horizontalAngle = lookTarget.transform.eulerAngles.y;
                verticalAngle = 0.0f;
                elapsedTime = 0;
                EndPos = lookTarget.transform.eulerAngles.y;
                StartPos = horizontalAngle;
                time = 0.5f;
                deltaPos = (EndPos - StartPos) / time;
                deltaPos = Mathf.Repeat(deltaPos, 360.0f / time);//360進数に直す

                is_Q_Move = true;
            }
        }
            
        if (is_Q_Move)
        {
            if(deltaPos < 180 / time)//近いほうに回す
            {
                horizontalAngle += deltaPos * Time.deltaTime;
            }
            else
            {
                horizontalAngle -= ((360 / time - deltaPos)) * Time.deltaTime;
            }
            //horizontalAngle += deltaPos * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            if (elapsedTime > time)
            {

                horizontalAngle = EndPos;

                elapsedTime = 0;
                is_Q_Move = false;

            }
        }
        //////////////////////////////////////////////////////////////////////

        //一人称始点
        /*
        if (Input.GetKeyDown(KeyCode.T))
        {

            if (!flag_TopView)
            {

                flag_TopView = true;
                T_distance = distance;
                SmoothMove(-1, 1);

            }
            else
            {

                flag_TopView = false;
                SmoothMove(T_distance, 1);

            }

        }*/

        //視点の距離を変更
        /*
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(distance == 5)
            {
                SmoothMove(10, 1);
            }
            else if (distance == 10)
            {
                SmoothMove(3, 1);
            }
            else if (distance == 3)
            {
                SmoothMove(5, 1);
            }
            else
            {
                SmoothMove(5, 1);
            }
            flag_TopView = false;
        }
        */

            //スムーズに動かしたいならこれ
        if (isMove)
        {
            distance += deltaPos * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            if (elapsedTime > time)
            {

                distance = EndPos;

                elapsedTime = 0;
                isMove = false;

            }
        }

    }

    //スムーズにdistanceを動かす////////////////////////////////////////////////
    public float StartPos;
    public float EndPos;
    public float time = 5;
    private float deltaPos;

    private float elapsedTime;

    private bool isMove = false;

    void SmoothMove(float End, float speed)
    {
        elapsedTime = 0;
        EndPos = End;
        StartPos = distance;
        time = 1 / speed;
        deltaPos = (EndPos - StartPos) / time;
        
        isMove = true;

    }
}
