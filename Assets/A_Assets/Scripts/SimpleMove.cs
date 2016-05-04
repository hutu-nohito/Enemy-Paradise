using UnityEngine;
using System.Collections;

public class SimpleMove : MonoBehaviour {

	public enum Move_Type{
		Line,//直進
		Rotate,//回転
		Sin,//Sin
		Scale,//拡縮
		Follow,//プレーヤー追尾
		Destroy,//消す
		Look//Playerのほうを向く
	}; 
	public Move_Type element = Move_Type.Line;

	public Vector3 Control = Vector3.zero;

	public GameObject Player;

	private float time = 0;

    //Sin用
    private MoveSmooth MS;
    private Move_Controller MC;
    public float moveSpeed = 0;
	
	// Use this for initialization
	void Start () {

		Player = GameObject.FindGameObjectWithTag("Player");

        //Sinで動かすときはこの二つが必要
        if(element == Move_Type.Sin)
        {
            MS = GetComponent<MoveSmooth>();
            MC = GetComponent<Move_Controller>();
        }

	}
	
	// Update is called once per frame
	void Update () {

		switch(element){
		case Move_Type.Line:
			Line();
			break;
		case Move_Type.Rotate:
			Rotate();
			break;
		case Move_Type.Sin:
			Sin();
			break;
		case Move_Type.Scale:
			Scale();
			break;
		case Move_Type.Follow:
			Follow();
			break;
		case Move_Type.Destroy:
			Destroy ();
			break;
		case Move_Type.Look:
			Look();
			break;
		default:
			break;
		}
		
	}

	void Line (){

		transform.Translate(Control * Time.deltaTime);

	}

	void Rotate (){

		transform.Rotate(Control * Time.deltaTime);
		//transform.localEulerAngles += Control * Time.deltaTime;

	}

	void Sin (){

        MS.Move(MC.End,moveSpeed);

	}

	void Scale (){

		transform.localScale += (Control * Time.deltaTime);

	}

	void Follow (){

		Vector3 follow = (Player.transform.position - this.transform.position).normalized;
		follow.y = 0.0f;
		transform.position += (follow * 0.01f * Control.x);
		transform.rotation = Quaternion.LookRotation(-follow);
		
	}

	void Destroy(){

		Destroy (this.gameObject);

	}

	//挙動がおかしい
	void Look (){

		/*Vector3 look = this.transform.position - Player.transform.position;
		transform.rotation = Quaternion.LookRotation(Control.x * look);*/
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Player.transform.position - transform.position), 0.1f);//これをPlayerのほうにゆっくり向ける
		
	}

}
