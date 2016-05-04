using UnityEngine;
using System.Collections;

public class BlockDelete : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {

        if (col.name == "DarkLilith_Block(Clone)")
        {
            Destroy(col.gameObject);
        }
    }
}
