using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TouchJin : MonoBehaviour {

    public Magic_ControllerVR MC;
    public GameObject HandR;
    public int PointNumber = 0;
    public GameObject UI;

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

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject == HandR)
        {
            UI.GetComponent<Image>().color = new Color(1.0f,1.0f,1.0f,1.0f);
        }
    }

    void OnTriggerExit(Collider col)
    {

        if (col.gameObject == HandR)
        {
            UI.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
        }
    }
}
