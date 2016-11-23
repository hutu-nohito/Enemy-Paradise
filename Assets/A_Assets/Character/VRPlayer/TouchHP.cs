using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TouchHP : MonoBehaviour {

    public Magic_ControllerVR MC;
    public GameObject HandR;
    public GameObject UI;
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {

        if (col.gameObject == HandR)
        {
            MC.PutHP();

        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject == HandR)
        {
            UI.GetComponent<Text>().color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        }
    }

    void OnTriggerExit(Collider col)
    {

        if (col.gameObject == HandR)
        {
            UI.GetComponent<Text>().color = new Color(0.2f, 0.2f, 0.2f, 0.75f);
        }
    }
}
