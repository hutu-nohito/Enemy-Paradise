using UnityEngine;
using System.Collections;

public class MousePoint : MonoBehaviour {

	public Vector3 worldPoint = Vector3.zero;
	public float z_position = 50.0f;
	
	void Update () {
		
		Vector3 screenPoint = Input.mousePosition;
		screenPoint.z = 50.0f;

        //カメラがなかったら処理しない
        if(Camera.main != null)
		worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
		
	}
}
