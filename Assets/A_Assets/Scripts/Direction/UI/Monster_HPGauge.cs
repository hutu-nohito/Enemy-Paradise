using UnityEngine;
using System.Collections;
using UnityEngine.UI;//これが必要

public class Monster_HPGauge : MonoBehaviour {

    public GameObject Monster;

    private Image Gauge; //

    private Enemy_Base EB;
    private float maxHP = 100;
    private float oldHP = 100;
    private float maxMP = 100;
    private float oldMP = 100;

    private bool isGauge = false;
    private float time = 0;
    private float deltaGauge = 0;//ゲージの変化量
    public float deltaTime = 1;//ゲージを変化させる時間
    
    void Start()
    {
        Gauge = GetComponent<Image>();
        EB = Monster.GetComponent<Enemy_Base>();

        oldHP = EB.GetHP();
        maxHP = oldHP;
    }

    void Update()
    {
        changePicturesUpdate();
    }

    void changePicturesUpdate()
    {
        //ゲージをゆっくり変化させる
        if (isGauge)
        {
            if (Gauge.fillAmount >= 0 || Gauge.fillAmount <= 1)//範囲指定
                Gauge.fillAmount += deltaGauge / deltaTime * Time.deltaTime;
            time += Time.deltaTime;

            if (time > deltaTime)
            {
                Gauge.fillAmount = EB.GetHP() / maxHP;
                time = 0;
                isGauge = false;
            }


        }
        if (EB.GetHP() != oldHP)
        {
            deltaGauge = (EB.GetHP() - oldHP) / maxHP;

            if (isGauge)
            {
                time = 0;//いったん切る
                deltaGauge = EB.GetHP() / maxHP - Gauge.fillAmount;

            }
            else
            {
                isGauge = true;
            }

        }
        oldHP = EB.GetHP();
    }
}
