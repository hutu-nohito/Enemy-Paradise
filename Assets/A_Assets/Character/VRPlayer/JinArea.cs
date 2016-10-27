using UnityEngine;
using System.Collections;

public class JinArea : MonoBehaviour {

    public GameObject HandL;
    public Magic_ControllerVR magic;

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
        if(col.gameObject == HandL)
        {
            if (flag_save)
            {
                save.SetRod();
            }
            else
            {
                magic.SetRod();
            }
            
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject == HandL)
        {
            if (flag_save)
            {
                save.SetRod();
            }
            else
            {
                magic.PutRod();
            }
            
        }
    }
}
