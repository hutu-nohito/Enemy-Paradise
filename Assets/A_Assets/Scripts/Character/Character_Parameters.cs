using UnityEngine;
using System.Collections;

public class Character_Parameters : MonoBehaviour {

    /******************************************************************************/
    /** @brief キャラクタのパラメタをすべてここで管理
    * @date 2016/05/29
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    *
*/
    /******************************************************************************/
    /* 更新履歴
    *  フラグ管理と統合する
    *  行動状態追加
    *  状態異常追加
    *  状態変化追加
    */
    /******************************************************************************/


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

    public Attack_Parameter.MagicElement[] weak_element;//弱点属性
    public Attack_Parameter.MagicElement[] GetWeak() { return weak_element; }
    public void SetWeak(int num, Attack_Parameter.MagicElement element)
    { this.weak_element[num] = element; }//numが置き換える弱点の場所。elementが新しい弱点属性(nullも可)

    public Attack_Parameter.MagicElement[] proof_element;//耐性属性
    public Attack_Parameter.MagicElement[] GetProof() { return proof_element; }
    public void SetProof(int num, Attack_Parameter.MagicElement element)
    { this.proof_element[num] = element; }//numが置き換える耐性の場所。elementが新しい耐性属性(nullも可)

    public Attack_Parameter.MagicElement[] invalid_element;//無効化属性
    public Attack_Parameter.MagicElement[] GetInvalid() { return invalid_element; }
    public void SetInvalid(int num, Attack_Parameter.MagicElement element)
    { this.invalid_element[num] = element; }//numが置き換える耐性の場所。elementが新しい耐性属性(nullも可)

    public Vector3 home_position = Vector3.zero;//キャラクタの初期位置
    public Vector3 GetHome() { return home_position; }

    public Vector3 move_direction = Vector3.zero;//キャラクタの移動方向
    public Vector3 GetMove() { return move_direction; }
    public void SetMove(Vector3 move_direction) { this.move_direction = move_direction; }

    public Vector3 direction = Vector3.zero;//キャラクタが向いている方向(ほぼPlayerが向いてる方向として使ってる)
    public Vector3 GetDirection() { return direction; }
    public void SetDirection(Vector3 direction) { this.direction = direction; }

    //行動状態//////////////////////////////////////////////////////////
    public bool flag_move = true;//移動できるかどうか(WASDが有効かどうか)
    public bool flag_jump = true;//ジャンプできるかどうか(Spaceが有効かどうか)
    public bool flag_damage = true;//ダメージを受けるかどうか
    public bool flag_magic = true;//魔法が使えるかどうか(マウスが有効かどうか)

    public bool GetF_Move() { return flag_move; }//移動できるか
    public bool GetF_Jump() { return flag_jump; }//ジャンプできるか
    public bool GetF_Damage() { return flag_damage; }//ダメージをうけるかどうか
    public bool GetF_Magic() { return flag_magic; }//魔法が使えるか

    public void Reverse_Move() { flag_move = !flag_move; }//移動反転
    public void Reverse_Jump() { flag_jump = !flag_jump; }//ジャンプ反転
    public void Reverse_Damage() { flag_damage = !flag_damage; }//ダメージ反転
    public void Reverse_Magic() { flag_magic = !flag_magic; }//魔法反転

    public void SetKeylock()
    {
        flag_move = false;
        flag_jump = false;
        flag_magic = false;
    }//操作禁止

    public void SetActive()
    {
        flag_move = true;
        flag_jump = true;
        flag_magic = true;
    }//キーロック解除

    public void SetMovelock()
    {
        flag_move = false;
        flag_jump = false;
    }//移動禁止

    //状態////////////////////////////////////////////////////////////////////////////////

    public bool flag_ground = true;//接地してるかどうか
    public bool GetGround() { return flag_ground; }
    public void ReverseGround() { flag_ground = !flag_ground; }

    public bool flag_fade = false;//消えてるかどうか
    public bool GetFade() { return flag_fade; }
    public void ReverseFade() { flag_fade = !flag_fade; }

    //状態異常(ailment)/////////////////////////////////////////////////////////////////

    public bool flag_poison = false;//毒状態
    public bool GetPoison() { return flag_poison; }
    public void ReversePoison() { flag_poison = !flag_poison; }

    //ノックバックは状態異常
    public bool flag_knock = false;//ノックバック状態
    public bool GetKnock() { return flag_knock; }
    public void ReverseKnock() { flag_knock = !flag_knock; }

    //状態変化(conversion)/////////////////////////////////////////////////////////////////

    public bool flag_invincible = false;//無敵かどうか
    public bool GetInvincible() { return flag_invincible; }
    public void ReverseInvincible() { flag_invincible = !flag_invincible; }

    //注目(今はプレイヤのみ)
    public bool flag_watch = false;//注目しているかどうか
    public bool GetF_Watch() { return flag_watch; }//注目しているか
    public void Set_Watch() { flag_watch = true; }//注目
    public void Release_Watch() { flag_watch = false; }//注目解除
}
