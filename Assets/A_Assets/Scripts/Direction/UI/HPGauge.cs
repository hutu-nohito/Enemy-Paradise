using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HPGauge : MonoBehaviour
{

    private Image circleGauge; // CircleGauge

    private Player_ControllerVR pcZ;
    private float maxHP = 100;
    private float oldHP = 100;
    private float maxMP = 100;
    private float oldMP = 100;

    private bool isGauge = false;
    private float time = 0;
    private float deltaGauge = 0;//ゲージの変化量
    public float deltaTime = 1;//ゲージを変化させる時間

    public enum GaugeType
    {
        HP,
        MP
    }
    public GaugeType type = GaugeType.HP;

    void Start()
    {
        circleGauge = GetComponent<Image>();
        pcZ = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ControllerVR>();

        switch (type)
        {
            case GaugeType.HP:
                oldHP = pcZ.GetHP();
                maxHP = oldHP;
                break;
            case GaugeType.MP:
                oldMP = pcZ.GetMP();
                maxMP = oldMP;
                break;
            default:
                break;
        }
        
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
            if(circleGauge.fillAmount >= 0 || circleGauge.fillAmount <= 1)//範囲指定
            circleGauge.fillAmount += deltaGauge / deltaTime * Time.deltaTime;
            time += Time.deltaTime;

            if(time > deltaTime)
            {
                switch (type)
                {
                    case GaugeType.HP:
                        circleGauge.fillAmount = pcZ.GetHP() / maxHP;
                        break;
                    case GaugeType.MP:
                        circleGauge.fillAmount = pcZ.GetMP() / maxMP;
                        break;
                    default:
                        break;
                }
                time = 0;
                isGauge = false;
            }


        }
        switch (type)
        {
            case GaugeType.HP:
                if (pcZ.GetHP() != oldHP)
                {
                    deltaGauge = (pcZ.GetHP() - oldHP) / maxHP;

                    if (isGauge)
                    {
                        time = 0;//いったん切る
                        deltaGauge = pcZ.GetHP() / maxHP - circleGauge.fillAmount;

                    }
                    else
                    {
                        isGauge = true;
                    }

                }
                oldHP = pcZ.GetHP();
                break;
            case GaugeType.MP:
                if (pcZ.GetMP() != oldMP)
                {
                    deltaGauge = (pcZ.GetMP() - oldMP) / maxMP; ;

                    if (isGauge)
                    {
                        time = 0;//いったん切る
                        deltaGauge = pcZ.GetMP() / maxMP - circleGauge.fillAmount;

                    }
                    else
                    {
                        isGauge = true;
                    }
                    
                }
                oldMP = pcZ.GetMP();
                break;
            default:
                break;
        }       
    }
}
