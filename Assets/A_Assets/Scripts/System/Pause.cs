using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

    //Zボタンでポーズ
    public GameObject PauseBoad;
    private bool isPause = false;

    private QuestManager QM;
    private Player_ControllerZ PCZ;

	// Use this for initialization
	void Start () {

        QM = GameObject.FindGameObjectWithTag("Manager").GetComponent<QuestManager>();
        PCZ = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerZ>();

	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (isPause)
            {
                PCZ.SetActive();
                PauseBoad.SetActive(false);
                Time.timeScale = 1;
                isPause = false;
            }
            else
            {
                PCZ.SetKeylock();
                PauseBoad.SetActive(true);
                Time.timeScale = 0;
                isPause = true;
            }
        }
	}

    public void Continue()
    {
        PCZ.SetActive();
        PauseBoad.SetActive(false);
        Time.timeScale = 1;
        isPause = false;
    }

    public void Quit()
    {
        PCZ.SetActive();
        Time.timeScale = 1;
        QM.Questfailure();
    }


}
