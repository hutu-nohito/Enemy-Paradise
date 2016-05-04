using UnityEngine;
using System.Collections;

public class Enemy_ControllerZ : Enemy_Parameter
{

    /*

    敵の基本操作用

        キャラクタの状態管理は全部ここでやる
        他ではやらない
        ここではキャラクタを動かさない
        ここはトリガを管理

        浮いてる敵の高さはジャンプ力を使う
        浮いてる敵は自分より高い位置にある段差は上らない
    */

    //使うもの
    [System.NonSerialized]
    public Move_Controller move_controller;//周辺探索用
    private MoveSmooth MoveS;//動かすよう空中

    [System.NonSerialized]
    public GameObject Player;//操作キャラ

    public Transform Territory;//縄張り

    public bool frontWall = false;//前に壁がある

    //汎用
    

    //初期パラメタ(邪魔なのでインスペクタに表示しない)
    [System.NonSerialized]
    public int max_HP, max_MP, base_Pow, base_Def;
    [System.NonSerialized]
    public float base_Sp, base_Ju;

    // Use this for initialization
    void Start()
    {
        move_controller = GetComponent<Move_Controller>();
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Territory == null) Territory = this.gameObject.transform;//テリトリーがない場合は自分の位置を入れといて変な挙動をしないようにする

        //移動方法によって動きを変える
        switch (move)
        {
            case Enemy_Move.Ground://×ナビで動く　○自力で
                MoveS = GetComponent<MoveSmooth>();
                break;
            case Enemy_Move.Float://自力で動かす
                MoveS = GetComponent<MoveSmooth>();
                break;
            case Enemy_Move.Stand://何もしない
                break;
            default:
                break;
        }

        //初期パラメタを保存
        max_HP = H_point;
        max_MP = M_point;
        base_Pow = power;
        base_Def = def;
        base_Sp = speed;
        base_Ju = jump;

    }

    // Update is called once per frame
    void Update()
    {
        //HPがなくなった時の処理
        if (H_point <= 0)
        {

            Destroy(this.gameObject);
            GameObject.FindGameObjectWithTag("Manager").GetComponent<QuestManager>().SetCount(CharaName);

        }

        //索敵    撤去
        /*
        if (state == Enemy_State.Search)
        {
            //どっちかある方移動
            if(MoveS != null)
            MoveS.Move(move_controller.End,speed);

            //前を向ける
            transform.rotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(move_controller.End - transform.localPosition), 0.05f);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        }*/

        //逃げ    撤去
        /*
        if (state == Enemy_State.Run)
        {

            Vector3 follow = (Player.transform.localPosition - this.transform.localPosition).normalized;
            follow.y = 0.0f;

            //どっちかある方移動
            if (MoveS != null)
                MoveS.Move(-follow * speed, speed);

            transform.localRotation = Quaternion.LookRotation(-follow);

        }*/

        //Returnは個別のほうがよさげ
        /*if (state == Enemy_State.Return)
        {
            time += Time.deltaTime;

            //とりあえず中心へ(Territoryはワールド座標にしとく)
            if (Nav != null)
                Nav.Move(Territory.position);
            if (MoveS != null)
                MoveS.Move(Territory.position, speed);

            //前を向ける
            transform.rotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(Territory.position - transform.localPosition), 0.05f);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            
            if(time > 10.0f)
            {
                state = old_state;//10秒くらいたったら元の状態に戻す
            }
        }*/

        //レイキャストで何とかして壁に当たらないようにする
        RaycastHit hit;
        Vector3 StartPos = transform.position + new Vector3(0,transform.localScale.y,0);//とりあえず頭から出す

        //これのとき前に壁がある
        if (Physics.Raycast(StartPos, transform.TransformDirection(Vector3.forward), out hit ,20))
        {
            //Debug.DrawLine(StartPos, hit.point, Color.green);
            frontWall = true;

        }
        else
        {
            frontWall = false;
        }

        //レイキャストで浮かせる
        RaycastHit hit_float;
        Vector3 StartPos_float = transform.position;//とりあえず足元から出す

        //
        if (move == Enemy_Move.Float)//浮いてるやつの高さ調整
        {
            if (Physics.Raycast(StartPos_float, transform.TransformDirection(Vector3.down), out hit_float, jump))//jumpが高さ
            {
                //Debug.DrawLine(StartPos_float, hit_float.point, Color.green);
                transform.position = new Vector3(transform.position.x, hit_float.point.y + jump, transform.position.z);//高さを固定する

            }
            else//下の足場に下りようとしてる
            {
                Move(Vector3.down, speed);
            }
        }
            

        direction = transform.TransformDirection(Vector3.forward);//向き

        //移動用
        if (isMove)
        {
            transform.position += deltaPos * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            if (elapsedTime > time)
            {

                elapsedTime = 0;
                isMove = false;

            }
        }

    }

    //ワールド座標で移動//////////////////////////////////////////////////////////////////////////////////

    private Vector3 StartPos;
    private Vector3 EndPos;
    private float time = 5;
    private Vector3 deltaPos;

    private float elapsedTime;

    private bool isMove = false;

    public void Move(Vector3 End, float speed)
    {
        elapsedTime = 0;
        EndPos = End;
        StartPos = transform.position;
        time = (EndPos - StartPos).magnitude / speed;//道のり÷速さ
        deltaPos = (EndPos - StartPos) / time;
        
        isMove = true;
    }

    public void Stop()
    {
        isMove = false;//これで止まる
    }

    //状態管理//////////////////////////////////////////////////////////////////////////////
    //優先順位も個別でやるべき
    //トリガ
    public bool isFind = false;
    public bool isDamage = false;
    public bool isReturn = false;

    public void Find()
    {
        isFind = true;
    }

    public void NotFind()
    {
        isFind = false;
    }

    public void Damage()
    {
        isDamage = true;
    }

    public void NotDamage()
    {
        isDamage = false;
    }

    //縄張りから外れた時に戻ってくるよう
    public void Return()
    {
        isReturn = true;
    }
    public void NotReturn()
    {
        isReturn = false;
    }

}
