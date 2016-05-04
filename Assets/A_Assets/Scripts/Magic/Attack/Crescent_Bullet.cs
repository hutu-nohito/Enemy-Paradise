using UnityEngine;
using System.Collections;

public class Crescent_Bullet : Attack_Parameter {

    public float returnTime = 2.5f;//折り返す時間
    private bool flag_Return = false;
    //これはPlayer用のホーミング。敵はSimpleMoveのfollowでいいと思う。
    private GameObject Target;
    private bool flag_Stop = false;//高さだけにする

    /// 旋回速度
    public float _rotSpeed = 1.0f;

    // Use this for initialization
    void Start()
    {
        Target = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Return());

    }
    
    IEnumerator Return()
    {
        yield return new WaitForSeconds(returnTime);

        this.GetComponent<Rigidbody>().velocity = Vector3.zero;

        yield return new WaitForSeconds(0.4f);

        flag_Return = true;

        /*
        Vector3 ReturnPos = Parent.transform.position + new Vector3(0,1.0f,0);
        this.GetComponent<Rigidbody>().velocity = ((ReturnPos - this.transform.position).normalized * speed);
        */
    }

    /// 現在の角度がこれで求まるらしい
    float Direction
    {
        get { return Mathf.Atan2(this.GetComponent<Rigidbody>().velocity.z, this.GetComponent<Rigidbody>().velocity.x) * Mathf.Rad2Deg; }
    }

    // Update is called once per frame
    void Update()
    {
        if (flag_Return)
        {
            Vector3 next = Target.transform.position;
            Vector3 now = transform.position;
            // 目的となる角度を取得する
            var d = next - now;
            var targetAngle = Mathf.Atan2(d.z, d.x) * Mathf.Rad2Deg;
            // 角度差を求める
            var deltaAngle = Mathf.DeltaAngle(Direction, targetAngle);
            var newAngle = Direction;
            if (Mathf.Abs(deltaAngle) < _rotSpeed)
            {
                // 旋回速度を下回る角度差なので何もしない

            }
            else if (deltaAngle > 0)
            {
                // 左回り
                newAngle += _rotSpeed;

            }
            else
            {
                // 右回り
                newAngle -= _rotSpeed;

            }

            // 新しい速度を設定する
            SetVelocity(newAngle, speed);

        }
        

    }

    void SetVelocity(float direction, float speed)
    {
        var vx = Mathf.Cos(Mathf.Deg2Rad * direction) * speed;
        var vz = Mathf.Sin(Mathf.Deg2Rad * direction) * speed;

        if (Mathf.Abs(Target.transform.position.x - transform.position.x) < 0.5f && Mathf.Abs(Target.transform.position.z - transform.position.z) < 0.5f)
        {

            flag_Stop = true;

        }

        //高さは別に設定
        var vy = ((Target.transform.position.y + (Target.transform.localScale.y)) - transform.position.y) * speed / (Target.transform.position - transform.position).magnitude;//高さを後に合わせたほうが見栄えがいい

        if (flag_Stop) { vx = 0; vz = 0; flag_Stop = false; }

        this.GetComponent<Rigidbody>().velocity = new Vector3(vx, vy, vz);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            //これで100を超えないはず
            if(Target.GetComponent<Player_ControllerZ>().GetMP() + 10 < 100)
            {
                Target.GetComponent<Player_ControllerZ>().SetMP(Target.GetComponent<Player_ControllerZ>().GetMP() + 10);
            }
            else
            {
                Target.GetComponent<Player_ControllerZ>().SetMP(100);
            }
            
        }
    }
}
