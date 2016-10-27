using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TouchJin : MonoBehaviour {

    public Magic_ControllerVR MC;
    public GameObject HandR;
    public int PointNumber = 0;
    public GameObject UI;

    public SaveRune save;
    public bool flag_save = false;

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
            if (flag_save)
            {
                save.DotToDot(PointNumber);
            }
            else
            {
                MC.DotToDot(PointNumber);
            }
            
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
            UI.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        }
    }
}
