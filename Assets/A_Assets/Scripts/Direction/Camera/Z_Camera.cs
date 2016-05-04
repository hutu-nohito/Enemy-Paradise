using UnityEngine;
using System.Collections;

public class Z_Camera : MonoBehaviour {

    /*
    カメラ側の機能
    */

        /// <summary>
        /// 注目が解けたら一定期間は注目できないようにする
        /// </summary>
    public float RotSpeed = 20;//注目時の回転移動速度の調整用
    public float tiltAngle = 2;//カメラティルト調整用の角度
    public float coolTime = 0.0f;//注目できないようにする時間

    public float length;//注目してる敵との距離
	public GameObject Target;//ここに注目対象を格納すればいい

	private GameObject Player;
	private Player_ControllerZ pcZ;

    public GameObject nearMarker;//今注目できる敵のマーカ
    public GameObject targetMarker;//今注目してる敵のマーカ

    //演出
    //マーカを上下に動かす
    public Vector3 QuakeMagnitude = new Vector3(0, 1, 0);
    private Vector3 Offset = Vector3.zero;
    private float quaketime = 0;
    private float elapsedquakeTime = 1.0f;


    // Use this for initialization
    void Start () {
		
		Player = GameObject.FindGameObjectWithTag("Player");
        pcZ = Player.GetComponent<Player_ControllerZ>();

        nearMarker = GameObject.Find("nearMarker");
        targetMarker = GameObject.Find("targetMarker");

        nearMarker.SetActive(false);
        targetMarker.SetActive(false);

        QuakeMagnitude /= 1000;//これでわかりやすく
		
	}

	//ちらちらしそうだったらLateUpdateに変更
	// Update is called once per frame
	void LateUpdate () {
        
        //とりあえず右クリックで注目
		if(Input.GetMouseButton(1)){

            if (Target != null) { //ターゲットがいたら

                pcZ.Set_Watch();//注目しているよー

                /*Vector3 LocalPos = Vector3.zero;
                Vector3 cal = Vector3.zero;//計算用の箱
                float degreeTheta = 0;//アフィン変換用の角度

                if (pcZ.GetF_Move())
                {
                    //回転角を指定してやる
                    if (Input.GetKey(KeyCode.A))
                    {

                        //degreeTheta = -0.1f;
                        degreeTheta = -pcZ.GetSpeed() * RotSpeed / length * Time.deltaTime;//動かすときはスペックで違いが出ないようにDeltatime

                    }
                    if (Input.GetKey(KeyCode.D))
                    {

                        //degreeTheta = 0.1f;
                        degreeTheta = pcZ.GetSpeed() * RotSpeed / length * Time.deltaTime;//動かすときはスペックで違いが出ないようにDeltatime

                    }
                }

                //アフィン変換　回転
                LocalPos = Player.transform.position - Target.transform.position;//ターゲットに対するローカル座標に変換
			    //回転
			    cal.x = LocalPos.x * Mathf.Cos(degreeTheta * Mathf.Deg2Rad) - LocalPos.z * Mathf.Sin(degreeTheta * Mathf.Deg2Rad);
			    cal.z = LocalPos.x * Mathf.Sin(degreeTheta * Mathf.Deg2Rad) + LocalPos.z * Mathf.Cos(degreeTheta * Mathf.Deg2Rad);
			    LocalPos = cal;//座標に代入
			    LocalPos.y = Player.transform.position.y - Target.transform.position.y;//ジャンプさせるからYの値は変えない
			    Player.transform.position = LocalPos + Target.transform.position;//ワールド座標に直す*/

                //Playerの方向 = (最初の方向,向けたい方向,向けたい速度)
                Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, Quaternion.LookRotation(Target.transform.position - Player.transform.position), 0.1f);//Playerをターゲットのほうにゆっくり向ける
                Player.transform.rotation = Quaternion.Euler(0, Player.transform.eulerAngles.y, 0);//Playerのx,zの回転を直す。回転嫌い。全部Eulerにしてしまえばよい

                //this.transform.LookAt((Player.transform.position + Target.transform.position) / 2);//Playerとターゲットの中心をPlayerの後ろから映して、どっちも画面内に収める

                //ターゲットとの高さによって回転角度を決めてる。tiltAngleで角度の調整
                /*Vector3 LookRot = new Vector3((this.transform.position.y - Target.transform.position.y) * tiltAngle, 0, 0);//カメラの回転角度。X方向だけでいい。(ティルト)
                this.transform.localEulerAngles = LookRot;
                this.transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(LookRot) , 1.0f);//Cameraをターゲットのほうにゆっくり向ける
                */

                //注目時のマーカー
                targetMarker.SetActive(true);
                targetMarker.transform.position = Target.transform.position + new Vector3(0, Target.transform.localScale.y + 1, 0) + Offset;
                nearMarker.SetActive(false);
            }
            else
            {
                targetMarker.SetActive(false);
            }

        }
        else
        {
            targetMarker.SetActive(false);
        }


        if (Input.GetMouseButtonUp(1)){

            pcZ.Release_Watch();//注目解除

		}

        //ターゲットがいなければ黄色いマーカを出さない
        if(Target == null)nearMarker.SetActive(false);

        //coolTimeが0.2以下の時注目できないようにする
        if (!pcZ.GetF_Watch()) {

            if(coolTime < 0.2f)
            coolTime += Time.deltaTime;//注目してなかったら足しとく
        }
        else
        {
            coolTime = 0.0f;
        }

        //揺らすよう
        Offset += QuakeMagnitude;
        quaketime += Time.deltaTime;
        if (quaketime > elapsedquakeTime)
        {
            QuakeMagnitude = -QuakeMagnitude;
            quaketime = 0;
        }



    }

    public void SetTarget(GameObject Target,float near)
    {

        if (Target == null) { 
            
            //this.Target = Target;
            //pcZ.Release_Watch();//注目解除
            coolTime = 0.0f;
        
        }//nullのときは注目を外す
        else if (!pcZ.GetF_Watch())
        {
            if(coolTime > 0.2f)
            {
                this.Target = Target;
                nearMarker.SetActive(true);
                nearMarker.transform.position = Target.transform.position + new Vector3(0, Target.transform.localScale.y + 1, 0) + Offset;
            }
            

        }//注目中はターゲットは変えない

        length = near;
        if (length >= 30)
        {
            this.Target = null;
            pcZ.Release_Watch();//注目解除
        }

    }
}
