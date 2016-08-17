using UnityEngine;
using System.Collections;

public class VRTutorial : MonoBehaviour {

    public GameObject GM;
    private Event_Manager EM;

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
            if (EM.TutorialStep == 1)//移動
            {
                EM.TutorialClear();
            }
            if (EM.TutorialStep == 3)//視点操作
            {
                EM.TutorialClear();
            }
        }
        if (col.tag == "Bullet")
        {
            if (EM.TutorialStep == 5)//ウェルオーウィスプ
            {
                EM.TutorialClear();
            }
        }
    }
}
