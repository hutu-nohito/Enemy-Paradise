using UnityEngine;
using System.Collections;

public class Move_Controller : MonoBehaviour {

	//変数//////////////////////////////
	
	private int count = 0;//動作の番号
	public float loop_time = 5.0f;//１ループの時間

	public Vector3[] Pos;//設定する止まる場所(自身からの相対位置)
	public Vector3 End;//送る値

	//GameObject////////////////////////
	
	//flag///////////////////////////////
	
	
	void Start () {
		
		Stop();
		
	}
	
	void Stop (){
		
		End = this.transform.localPosition + Pos[count];
        
		if(count == Pos.Length - 1){
			
			count = -1;
			
		}

		count++;
		Invoke("Stop",loop_time);		
	}

}
