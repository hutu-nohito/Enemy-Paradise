using UnityEngine;
using System.Collections;

public class TouchJin : MonoBehaviour {

    public Magic_ControllerVR MC;
    public GameObject HandR;
    public int PointNumber = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        
        if (col.gameObject == HandR)
        {
            Debug.Log(PointNumber);
            MC.DotToDot(PointNumber);
        }
    }
}
