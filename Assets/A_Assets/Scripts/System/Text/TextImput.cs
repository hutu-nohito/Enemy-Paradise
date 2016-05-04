using UnityEngine;
using System.Collections;
using UnityEngine.UI;//これが必要

public class TextImput : MonoBehaviour {

	new private Text guiText;

	private Static _static;
	private Player_ControllerZ pcZ;
    private Magic_Controller mc;
    private Enemy_ControllerZ ecZ;

	//public bool isCp = false;
	public enum Text_Type { 
        Day,//日付
        LP,//名声
        BP,//ボーナスポイント
        HP,//player HP
		MP,//player MP
        Magic,//選択中の魔法
        Enemy}
	public Text_Type type = Text_Type.Day;

	// Use this for initialization
	void Start () {

		guiText = this.GetComponent<Text>();

		switch(type){
		    case Text_Type.Day:
                _static = GameObject.FindGameObjectWithTag("Manager").GetComponent<Static>();

                if (_static.day - (int)_static.day == 0.5f) { 
                    guiText.text = (int)_static.day + "日目　夜";
                }
                else
                {
                    guiText.text = (int)_static.day + "日目　昼";
                }
			    break;
            case Text_Type.LP:
                _static = GameObject.FindGameObjectWithTag("Manager").GetComponent<Static>();
                guiText.text = "名声　" + _static.lank_P;
                break;
            case Text_Type.BP:
                _static = GameObject.FindGameObjectWithTag("Manager").GetComponent<Static>();
                guiText.text = "BP　" + _static.bonus_P;
                break;
		    case Text_Type.HP:

			    pcZ = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player_ControllerZ>();
			    break;
			case Text_Type.MP:
			
				pcZ = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player_ControllerZ>();
				break;
            case Text_Type.Magic:

                mc = GameObject.FindGameObjectWithTag("Player").GetComponent<Magic_Controller>();
                break;
            case Text_Type.Enemy:

			    ecZ = GameObject.Find("Boss_DarkLilith").GetComponent<Enemy_ControllerZ>();
			    break;
		    default:
			    break;
		}

	}
	
	// Update is called once per frame
	void Update () {

		switch (type) {
            case Text_Type.Day:
                _static = GameObject.FindGameObjectWithTag("Manager").GetComponent<Static>();

                if (_static.day - (int)_static.day == 0.5f)
                {
                    guiText.text = (int)_static.day + "日目　夜";
                }
                else
                {
                    guiText.text = (int)_static.day + "日目　昼";
                }
                break;
            case Text_Type.HP:

			guiText.text = "CP " + pcZ.H_point;
			break;
		case Text_Type.MP:
			
			guiText.text = "MP " + pcZ.M_point;
			break;
        case Text_Type.Magic:

            guiText.text = "S " + mc.SelectMagic[mc.magic_num].name;
            break;
        case Text_Type.Enemy:
			guiText.text = "Boss HP " + ecZ.H_point;
			break;
		default:
			break;
		}

	}

}
