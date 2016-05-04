using UnityEngine;
using System.Collections;

public class ColEffect : MonoBehaviour {

    //ぶつかったときに出るエフェクト　ダメージエフェクトとかに

    public GameObject[] Effects;//出すエフェクト
    public bool setaudio = false;//おとならすならこれ
    private AudioSource audiosource;
    public float EffTime = 2;//エフェクトが消えるまでの時間

	// Use this for initialization
	void Start () {

        //SEを鳴らす場合
        if (setaudio) audiosource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        
        //エフェクト系
        //当たっても消したくないもの
        if (col.gameObject.name == "Vision" || col.gameObject.name == "Search" || col.gameObject.name == "Territory" || col.gameObject.name == "Player_H")
        {
            return;
        }
        //当たっても消えないもの
        if (col.tag == "Reflect" || col.tag == "Player")
        {
            return;
        }
        if (col.tag != "Bullet")//弾と接触するときはエフェクトは出さない
        for(int i = 0;i < Effects.Length; i++)
        {
            Effects[i].transform.parent = null;//子供にしとくとたいてい消える
            Effects[i].SetActive(true);
            Destroy(Effects[i], EffTime);//2秒ぐらいで消しとく
        }

        //SE系
        if(audiosource != null)
        {
            if (!audiosource.isPlaying)
            {

                audiosource.PlayOneShot(audiosource.clip);//SE

            }
        }
    }
}
