using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {
	//特別なイベントがある場合にこれが呼び出される(Manager)
	
	public TextAsset[] EventText;//ここにイベント用のテキストを格納
	
	public GameObject Canvas;//UI
	
	private Static save;//日数、起動回数、HP、MP、名声、ボーナスポイント
	
	// Use this for initialization
	void Start () {
		
		save = GetComponent<Static>();
		Canvas = GameObject.Find ("Canvas");
		//if (save.GetDay () == 1) {
			
			Canvas.GetComponent<uGUI_Msg>().dispMessage(EventText[0]);
			
		//}
		
	}
	
	// Update is called once per frame
	void Update () {

	}
}
