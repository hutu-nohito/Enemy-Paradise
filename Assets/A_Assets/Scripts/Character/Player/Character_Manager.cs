using UnityEngine;
using System.Collections;

public class Character_Manager : Character_Parameters {

    //フラグと現在のパラメタを管理
    //このクラスに対してGet()系の関数を使うと現在のパラメタの値が取得できる
    //これでプレイヤの管理をする(ダメージを受けたら、ここのSetHP()でHPを減らす)

    //プレイヤの行動状態//////////////////////////////////////////////////////////
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

    public void SetKeylock() { 
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

    public bool flag_watch = false;//注目しているかどうか
    public bool GetF_Watch() { return flag_watch; }//注目しているか
    public void Set_Watch() { flag_watch = true; }//注目
    public void Release_Watch() { flag_watch = false; }//注目解除

    //プレイヤの状態異常(ailment)/////////////////////////////////////////////////////////////////

    public bool flag_poison = false;//毒状態
    public bool GetPoison() { return flag_poison; }
    public void ReversePoison() { flag_poison = !flag_poison; }

    //プレイヤの状態変化(conversion)/////////////////////////////////////////////////////////////////

    public bool flag_invincible = false;//無敵かどうか
    public bool GetInvincible() { return flag_invincible; }
    public void ReverseInvincible() { flag_invincible = !flag_invincible; }

    //public bool flag_speedUp = false;//加速状態

}
