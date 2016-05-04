using UnityEngine;
using System.Collections;

public class Character_Parameters : MonoBehaviour {

    //プレイヤーと敵の大元の基底クラス
    //キャラのパラメタ設定

    public int QuestStageID = 0;//ステージごとにそのクエストで出すかどうか決めるID ステージごとなので変わることはない
    public int GetQuestStage() { return QuestStageID; }

    //キャラクタのパラメタ設定(初期値)//////////////////////////////////////////////////////////

    public string CharaName = "Lilith";//キャラの種族名
    public string GetCharaName() { return CharaName; }
    public void SetCharaName(string CharaName) { this.CharaName = CharaName; }
    
    //派生クラスで現在のHPが取れる
    public int H_point = 3;//体力の最大値
    public int GetHP() { return H_point; }
    public void SetHP(int H_point) { this.H_point = H_point; }

    public int M_point = 10;//魔力の最大値
    public int GetMP() { return M_point; }
    public void SetMP(int M_point) { this.M_point = M_point; }

    public int power = 1;//基本攻撃力
    public int GetPower() { return power; }
    public void SetPower(int power) { this.power = power; }

    public int def = 0;//基本防御力
    public int GetDefense() { return def; }
    public void SetDefense(int def) { this.def = def; }

    public float speed = 5; //キャラクタの移動速度
    public float GetSpeed() { return speed; }
    public void SetSpeed(float speed) { this.speed = speed; }

    public float jump = 6;//キャラクタがジャンプする高さ
    public float GetJump() { return jump; }
    public void SetJump(float jump) { this.jump = jump; }

    public string[] weak_element;//弱点属性
    public string[] GetWeak() { return weak_element; }
    public void SetWeak(int num,string element)
    { this.weak_element[num] = element; }//numが置き換える弱点の場所。elementが新しい弱点属性(nullも可)

    public string[] proof_element;//耐性属性
    public string[] GetProof() { return proof_element; }
    public void SetProof(int num, string element)
    { this.proof_element[num] = element; }//numが置き換える耐性の場所。elementが新しい耐性属性(nullも可)

    public string[] invalid_element;//無効化属性
    public string[] GetInvalid() { return invalid_element; }
    public void SetInvalid(int num, string element)
    { this.invalid_element[num] = element; }//numが置き換える耐性の場所。elementが新しい耐性属性(nullも可)

    public Vector3 move_direction = Vector3.zero;//キャラクタの移動方向
    public Vector3 GetMove() { return move_direction; }
    public void SetMove(Vector3 move_direction) { this.move_direction = move_direction; }

    public Vector3 direction = Vector3.zero;//キャラクタが向いている方向
    public Vector3 GetDirection() { return direction; }
    public void SetDirection(Vector3 direction) { this.direction = direction; }

}
