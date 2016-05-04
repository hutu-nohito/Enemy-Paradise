using UnityEngine;
using System.Collections;
using UnityEngine.UI;//これが必要

public class TextQuestPaper : MonoBehaviour {

    //これはテキストにつける

    new private Text guiText;

    public GameObject QuestPaper;//クエストは(今んとこ)6個固定なので
    private Quest_Parameter QP;

    public enum Text_Type
    {
        Name,//クエスト名
        Misson,//カテゴリ
        Target,//対象
        Stage,//ステージ
        Pay,//報酬
        Lank,//ランク
        Term//期間
    }
    public Text_Type type = Text_Type.Name;

    // Use this for initialization
    void Start()
    {

        guiText = this.GetComponent<Text>();//つけてるテキストをゲット
        QP = QuestPaper.GetComponent<Quest_Parameter>();

        switch (type)
        {
            case Text_Type.Name:

                guiText.text = QP.GetName() ;
                /*
                _static = GameObject.FindGameObjectWithTag("Manager").GetComponent<Static>();

                if (_static.day - (int)_static.day == 0.5f)
                {
                    guiText.text = (int)_static.day + "日目　夜";
                }
                else
                {
                    guiText.text = (int)_static.day + "日目　昼";
                }*/
                break;
            case Text_Type.Misson
            :
                switch (QP.GetType())
                {
                    case Quest_Parameter.Quest_Type.Subjugation:
                        guiText.text = "カテゴリ:" + "討伐";
                        break;
                    case Quest_Parameter.Quest_Type.Capture:
                        guiText.text = "カテゴリ:" + "捕獲";
                        break;
                    case Quest_Parameter.Quest_Type.Collect:
                        guiText.text = "カテゴリ:" + "採取";
                        break;
                    case Quest_Parameter.Quest_Type.Defense:
                        guiText.text = "カテゴリ:" + "防衛";
                        break;
                    case Quest_Parameter.Quest_Type.Escort:
                        guiText.text = "カテゴリ:" + "護衛";
                        break;
                    case Quest_Parameter.Quest_Type.reclamation:
                        guiText.text = "カテゴリ:" + "開拓";
                        break;
                    case Quest_Parameter.Quest_Type.exploration:
                        guiText.text = "カテゴリ:" + "調査";
                        break;
                }
                break;
            case Text_Type.Target:

                guiText.text = "ターゲット:" + QP.GetTarget(0);
                for (int i = 1;i < QP.quest_Target.Length; i++)
                {
                    guiText.text = guiText.text + "," + QP.GetTarget(i);
                }
                break;
            case Text_Type.Stage:

                guiText.text = "場所:";
                switch (QP.GetStage())
                {
                    case Quest_Parameter.Stage.Green:
                        guiText.text += "草原";
                        break;
                    case Quest_Parameter.Stage.Mine:
                        guiText.text += "鉱山";
                        break;
                    case Quest_Parameter.Stage.Town:
                        guiText.text += "街";
                        break;
                    case Quest_Parameter.Stage.Swamp:
                        guiText.text += "湿原";
                        break;
                    case Quest_Parameter.Stage.Ruins:
                        guiText.text += "荒野";
                        break;
                    
                }
                break;
            case Text_Type.Pay:

                guiText.text = "報酬:" + QP.GetReward(0);
                for (int i = 1; i < QP.rewards.Length; i++)
                {
                    guiText.text = guiText.text + "," + QP.GetReward(i);
                }
                break;
            case Text_Type.Lank:

                guiText.text = "推奨ランク:" + QP.GetLevel().ToString();
                break;
            case Text_Type.Term:

                guiText.text = "張り出し期間:" + "あと" + QP.GetTerm().ToString() + "日";
                break;
            default:
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
