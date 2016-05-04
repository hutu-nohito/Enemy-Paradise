using UnityEngine;
using System.Collections;

public class Saisyu : MonoBehaviour {

    //採取用

    private QuestManager QM;
    public GameObject SE;
    private AudioSource audiosource;

	// Use this for initialization
	void Start () {

        QM = GameObject.FindGameObjectWithTag("Manager").GetComponent<QuestManager>();
        audiosource = SE.GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {

        if (col.gameObject.tag == "Player")
        {
            audiosource.PlayOneShot(audiosource.clip);
            SE.transform.parent = null;
            QM.SaisyuCount();
            Destroy(this.gameObject, 0.1f);
        }
    }
}
