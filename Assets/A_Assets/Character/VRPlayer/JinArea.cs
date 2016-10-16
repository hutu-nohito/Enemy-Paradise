using UnityEngine;
using System.Collections;

public class JinArea : MonoBehaviour {

    public GameObject HandL;
    public Magic_ControllerVR magic;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject == HandL)
        {
            magic.SetRod();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject == HandL)
        {
            magic.PutRod();
        }
    }
}
