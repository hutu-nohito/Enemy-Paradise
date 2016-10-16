using UnityEngine;
using System.Collections;

public class HaniVision : MonoBehaviour {

    public ViveHaniwonder script;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Bullet")
        {
            if (col.gameObject.GetComponent<Attack_Parameter>().Parent.name == "VRPlayer")
            {
                script.Guard();
            }
        }
    }
}
