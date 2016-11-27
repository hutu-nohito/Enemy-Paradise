using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Sound_Manager : MonoBehaviour {

    /******************************************************************************/
    /** @brief BGMの管理
    * @date 2016/11/27
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    */
    /******************************************************************************/
    /* 更新履歴
    *  
    */
    /******************************************************************************/

    /*
        0:タイトル
        1:ホーム
        2:ギルド
        3:チュートリアル
        4:コロシアム
    */

    private AudioSource Audio_Souce;//BGMの音源
    public AudioClip[] BGM;//

    // Use this for initialization
    void Start () {

        Audio_Souce = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Check_BGM()
    {
        if (SceneManager.GetActiveScene().name == "title")
        {
            Audio_Souce.clip = BGM[0];
            Audio_Souce.Play();
        }
        if (SceneManager.GetActiveScene().name == "Home")
        {
            //Audio_Souce.clip = BGM[1];
            //Audio_Souce.Play();
        }
        if (SceneManager.GetActiveScene().name == "Guild")
        {
            Audio_Souce.clip = BGM[2];
            Audio_Souce.Play();
        }
        if (SceneManager.GetActiveScene().name == "BackyardVR")
        {
            Audio_Souce.clip = BGM[3];
            Audio_Souce.Play();
        }
        if (SceneManager.GetActiveScene().name == "lilith2 battle field")
        {
            Audio_Souce.clip = BGM[4];
            Audio_Souce.Play();
        }
    }
}
