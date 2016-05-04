using UnityEngine;
using System.Collections;

public class Magic_Parameter : MonoBehaviour {

    //魔法の管理用パラメタ

    //キャラクタのパラメタ設定//////////////////////////////////////////////////////////

    public int magic_ID = 0;//魔法の管理番号
    public int GetM_ID() { return magic_ID; }

    public string magic_name = "Bullet";//魔法の名称
    public string GetM_name() { return magic_name; }

    public enum InputType{//入力方法
    GetDown,//押す
    Get//押して離す
    }
    public InputType inputtype = InputType.GetDown;
    public InputType GetInputType() { return inputtype; }
    
    public int spend_MP = 5;//消費MP
    public int GetSMP() { return spend_MP; }
    public void SetSMP(int spend_MP) { this.spend_MP = spend_MP; }

    public int existedNum = 1;//同時に出せる数
    public int GetExNum() { return existedNum; }
    public void SetExNum(int existedNum) { this.existedNum = existedNum; }

    public enum MagicElement
    {//魔法の属性
        Earth,//地
        Water,//水
        Fire,//火
        Wind,//風
    }
    public MagicElement magicElement = MagicElement.Earth;
    public MagicElement GetMagicElement() { return magicElement; }

    public GameObject Parent;//使用者
    public GameObject GetParent() { return Parent; }
    public void SetParent(GameObject Parent) { this.Parent = Parent; }

}
