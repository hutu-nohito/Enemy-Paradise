using UnityEngine;
using System.Collections;

public class VRTutorial : MonoBehaviour {

    private Event_Controller EC;

    public GameObject HandR;

	// Use this for initialization
	void Start () {

        //探す必要ないかも
        EC = GameObject.Find("Event_Controller").GetComponent<Event_Controller>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        //ちゅーとりある
        if (col.tag == "Player")
        {
            
        }
        if (col.tag == "Bullet")
        {
            
        }
        if(col.gameObject == HandR)//1
        {
            EC.TutorialClear();
        }
    }
}
