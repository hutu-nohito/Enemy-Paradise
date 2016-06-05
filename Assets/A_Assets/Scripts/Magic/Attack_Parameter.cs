using UnityEngine;
using System.Collections;

public class Attack_Parameter : MonoBehaviour {

    /******************************************************************************/
    /** @brief 攻撃・ギミックに必要な変数
    * @date 2016/06/05
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    * 
*/
    /******************************************************************************/
    /* 更新履歴
    *  ギミックに対応
    */
    /******************************************************************************/


    public int power = 3;//攻撃力
    public int GetPower() { return power; }

    public float speed = 5.0f; //弾速
    public float GetSpeed() { return speed; }

    public float attack_time = 5.0f; //攻撃判定の持続時間
    public float GetA_Time() { return attack_time; }

    public float rigid_time = 5.0f; //硬直時間
    public float GetR_Time() { return rigid_time; }

    public enum MagicElement
    {//魔法の属性
        Earth,//地
        Water,//水
        Fire,//火
        Wind,//風
    }
    public MagicElement magicElement = MagicElement.Earth;
    public MagicElement GetElement() { return magicElement; }

    public enum Property
    {//攻撃の特性
        NaN,//無
        Blow,//打撃
        Assailt,//突撃
        Slash,//斬撃
    }
    public Property property = Property.NaN;
    public Property GetProperty() { return property; }

    public enum Ailment
    {//状態異常
        NaN,//なし
        Poison,//毒
        Paralysis,//麻痺
        confusion,//混乱
    }
    public Ailment ailment = Ailment.NaN;
    public Ailment GetAilment() { return ailment; }

    public Vector3 KnockBack = Vector3.zero;//ノックバックで吹っ飛ばす方向　ノックバックさせない場合は0
    public Vector3 GetKnockBack() { return KnockBack; }
    public void SetKnockBack(Vector3 KnockBack) { this.KnockBack = KnockBack; }

    public GameObject Parent;
    public GameObject GetParent() { return Parent; }
    public void SetParent(GameObject Parent) { this.Parent = Parent; }

}
