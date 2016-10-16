using UnityEngine;
using System.Collections;

public class HeadCol : MonoBehaviour {

    public ViveHaniwonder script;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        

	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "HitPlayer")
        {
            script.Hit();
        }
    }
}
