using UnityEngine;
using System.Collections;

public class Enemy_Parameter : Character_Parameters {

    //敵キャラのパラメータ

    //敵キャラの状態//////////////////////////////////////////////////////////
    public bool flag_move = true;//移動できるかどうか(WASDが有効かどうか)
    public bool flag_damage = true;//ダメージを受けるかどうか
    public bool flag_magic = true;//魔法が使えるかどうか(マウスが有効かどうか)

    public bool GetF_Move() { return flag_move; }//移動できるか
    public bool GetF_Damage() { return flag_damage; }//ダメージをうけるかどうか
    public bool GetF_Magic() { return flag_magic; }//魔法が使えるか

    public void Reverse_Move() { flag_move = !flag_move; }//移動反転
    public void Reverse_Damage() { flag_damage = !flag_damage; }//ダメージ反転
    public void Reverse_Magic() { flag_magic = !flag_magic; }//魔法反転

    public void SetKeylock()
    {
        flag_move = false;
        flag_magic = false;
    }//操作禁止

    public void SetActive()
    {
        flag_move = true;
        flag_magic = true;
    }//キーロック解除

    public void SetMovelock()
    {
        flag_move = false;
    }//移動禁止

    //キャラクタの行動状態(State)//////////////////////////////////////////////////////////
    //状態じゃなくてトリガにする
    //いらない
    /*public enum Enemy_State
    {
        Idle,//通常       //意味なし
        Search,//索敵     //プレイヤ発見
        Attack,//臨戦態勢 //意味なし
        Run,//逃げ        //意味なし
        Return,//戻り     //テリトリの外に出た
        Damage,//ダメージ //ダメージを食らった
    }
    public Enemy_State state = Enemy_State.Idle;
    public Enemy_State GetState() { return state; }
    public void SetState(Enemy_State state) { this.state = state; }*/

    public bool flag_stop = false;//止まるかどうか
    public bool GetStop() { return flag_stop; }
    public void SetStop() { flag_stop = true; }
    public void ReleaseStop() { flag_stop = false; }

    //キャラクタの体調(Condition)/////////////////////////////////////////////////////////////////

    public bool flag_poison = false;//毒状態
    public bool GetPoison() { return flag_poison; }
    public void ReversePoison() { flag_poison = !flag_poison; }

    public bool flag_invincible = false;//無敵かどうか
    public bool GetInvincible() { return flag_invincible; }
    public void ReverseInvincible() { flag_invincible = !flag_invincible; }

    //public bool flag_speedUp = false;//加速状態

    //キャラクタの習性(Habit)////////////////////////////////////////////////////////////

    //いらない
    /*public enum Enemy_Habit
    {
        Positive = 0,
        Normal = 1,
        Negative = 2
    }*/
    /*public Enemy_Habit habit = Enemy_Habit.Normal;
    public Enemy_Habit GetHabit() { return habit; }
    public void SetHabit(Enemy_Habit habit) { this.habit = habit; }*/

    /*敵の移動方法
     * 
     * 
     * 敵発見時
     * Ground   : 地上をうろうろ
     * Float    : 空中をうろうろ
     * Stand    : 動かない
    */
    public enum Enemy_Move
    {
        Ground = 0,
        Float = 1,
        Stand = 2
    }

    public Enemy_Move move = Enemy_Move.Ground;
    public Enemy_Move GetMoveState() { return move; }

    /*索敵方法
     * 
     * Sight 視覚
     * Audition 聴覚
     * Flair 嗅覚
     * Oscillo 振動
     * Magic 魔法
     * Thermo 温度
     * 
     */

    public enum Enemy_Search
    {
        Sight = 0,
        Audition = 1,
        Flair = 2,
        Oscillo = 3,
        Magic = 4,
        Thermo = 5,
    }
    public Enemy_Search search = Enemy_Search.Audition;
    public Enemy_Search GetSearch() { return search; }
    public void SetSearch(Enemy_Search search) { this.search = search; }

}
