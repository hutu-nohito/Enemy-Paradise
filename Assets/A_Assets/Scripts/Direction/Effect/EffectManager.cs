using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour {

	//攻撃とかの視覚、聴覚効果を制御する

	public GameObject[] Effect;//実際に出てくるエフェクト
	public float[] WaitTime;//待ち時間
	private bool isEffect = false;

	private Coroutine coroutine;//コルーチン用の箱

	// Use this for initialization
	void Start () {
	
		coroutine = StartCoroutine(EffectUpdate());

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*IEnumerator EffectStart (){

	}*/

	IEnumerator EffectUpdate (){

		if(isEffect){yield break;}

		Effect [0].SetActive (true);

		//待機
		yield return new WaitForSeconds(WaitTime[0]);

		Effect [0].SetActive (false);
		Effect [1].SetActive (true);

	}
}
