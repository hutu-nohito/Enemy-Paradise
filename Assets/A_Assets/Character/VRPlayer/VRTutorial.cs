using UnityEngine;
using System.Collections;

public class VRTutorial : MonoBehaviour {

    public GameObject GM;
    private Event_Manager EM;

    public GameObject HandR;

	// Use this for initialization
	void Start () {

        EM = GM.GetComponent<Event_Manager>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            
        }
        if (col.tag == "Bullet")
        {
            
        }
        if(col.gameObject == HandR)//1
        {
            EM.TutorialClear();
        }
    }
}
