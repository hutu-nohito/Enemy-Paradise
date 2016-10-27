using UnityEngine;
using System.Collections;

public class WorldHUD : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        transform.position = Camera.main.transform.position + Camera.main.transform.TransformDirection(Vector3.forward).normalized;
        //transform.LookAt(Camera.main.transform.position);
        transform.localRotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
        
    }
}
