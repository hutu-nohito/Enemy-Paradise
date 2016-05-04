using UnityEngine;
using System.Collections;

public class Quest_Parameter : MonoBehaviour {

    //クエストのパラメタ設定

    public int quest_ID = 0;//クエスト識別番号
    public int GetID() { return quest_ID; }
    public void SetID(int ID) { this.quest_ID = ID; }

	public string QuestName = "Lilith";//クエストの名前
    public string GetName() {return QuestName; }
    public void SetName(string Name) { this.QuestName = Name; }

    public enum Quest_Type//クエストの種類。終了条件に使う
    {
        Subjugation,//討伐
        Capture,//捕獲
        Collect,//採取
        Defense,//防衛
        Escort,//護衛
        reclamation,//開拓
        exploration//調査
    }
    public Quest_Type type = Quest_Type.Subjugation;
    public Quest_Type GetType() { return type; }
    public void SetType(Quest_Type type) { this.type = type; }

    public enum Stage//クエストを行うステージ
    {
        Gaidou,
        Forest,
        Pond,
        Kougen,
        Green,//草原、森
        Mine,//鉱山
        Town,//街
        Swamp,//湿地
        Ruins,//遺跡
    }
    public Stage stage = Stage.Green;
    public Stage GetStage() { return stage; }
    public void SetStage(Stage stage) { this.stage = stage; }

    public int queststageID = 0;//ステージの構成を選択　変わる
    public int GetQestStageID() { return queststageID; }
    public void SetQuestStageID(int ID) { this.queststageID = ID; }

    public string[] quest_Target;//そのクエストの終了条件を満たす対象
    public string GetTarget(int num) { return quest_Target[num]; }
    public void SetTarget(string[] Target) { for (int i = 0; i < Target.Length; i++) { this.quest_Target[i] = Target[i]; } }

    public int clear_num = 5;//クリア討伐数

    public string[] rewards;//クエ報酬（複数可）
    public string GetReward(int num) { return rewards[num]; }//番号を送ってくれればその報酬を返す
    public void SetReward(string[] Reward) { for (int i = 0; i < Reward.Length; i++) { this.rewards[i] = Reward[i]; } }

    public string quest_level = "E";//クエスト難易度
    public string GetLevel() { return quest_level; }
    public void SetLevel(string level) { this.quest_level = level; }


    public int quest_term = 7;//クエストが張り出される期間
    public int GetTerm() { return quest_term; }
    public void SetTerm(int term) { this.quest_term = term; }

    public float quest_time = 0.5f;//そのクエストで何日進むか
    public float GetTime() { return quest_time; }
    public void SetTime(float time) { this.quest_time = time; }

}
