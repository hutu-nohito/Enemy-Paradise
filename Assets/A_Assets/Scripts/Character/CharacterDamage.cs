using UnityEngine;
using System.Collections;
using System.Collections.Generic;//リスト

public class CharacterDamage : MonoBehaviour {

    /******************************************************************************/
    /** @brief キャラクターのダメージ処理全般(状態異常も)
    * @date 2016/05/29
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
*/
    /******************************************************************************/
    /* 更新履歴
    *  プレイヤも敵も一緒くたにできるように
    *  とりあえず敵だけ？
    *  一応プレイヤにも適用してる
    *  ヒットエフェクト追加(属性ごと)
    *  毒追加
    */
    /******************************************************************************/
    
    private GameObject Parent;//このあたり判定を持つキャラ
    public bool weak_point = false;
    private Character_Parameters Cpara;//キャラクタのパラメタ

    private Renderer[] Renderer;//レンダー1
    private Renderer[] SkinRenderer;//レンダー2
    public GameObject Model;//モデル
    

    //演出(最初に取得)
    private List<GameObject> Effects = new List<GameObject>();//出すエフェクト
    private List<Vector3> Effect_basePos = new List<Vector3>();//エフェクトの基準位置

    //使用するエフェクト
    /*
            0   Hit
            1   Fire
            2   Water
            3   Poison
    */
    private enum EffectKind
    {
        Hit,
        Fire,
        Water,
        Poison
    }
    private EffectKind effectKind = EffectKind.Hit;

    void Start()
    {
        //親を取得
        Parent = gameObject.transform.parent.gameObject;
        

        if (Parent.gameObject.tag == "Player")
        {
            Cpara = Parent.GetComponent<Player_ControllerZ>();//プレイヤーだったらこっち
        }
        else
        {
            while (Parent.GetComponent<Enemy_Base>() == null)//エネミーベースが見つかるまで親をたどる
            {
                Parent = Parent.transform.parent.gameObject;
            }
                Cpara = Parent.GetComponent<Enemy_Base>();//敵だったらこっち
        }

        //エフェクトを取得(共通エフェクトなので決め打ち)
        Effects.Add(Parent.transform.FindChild("Effects").gameObject.transform.FindChild("Eff_Hit").gameObject);//ヒットエフェクト
        Effects.Add(Parent.transform.FindChild("Effects").gameObject.transform.FindChild("Eff_Fire").gameObject);//火エフェクト
        Effects.Add(Parent.transform.FindChild("Effects").gameObject.transform.FindChild("Eff_Water").gameObject);//水エフェクト
        Effects.Add(Parent.transform.FindChild("Effects").gameObject.transform.FindChild("Eff_Poison").gameObject);//毒エフェクト
        
        //エフェクト消す
        //エフェクトの基準位置を取得
        for (int i = 0; i < Effects.Count; i++)
        {
            Effects[i].SetActive(false);
            Effect_basePos.Add(Effects[i].transform.localPosition);
        }
        
        //今はいらない
        /*
        Renderer = Model.GetComponentsInChildren<MeshRenderer>();
        SkinRenderer = Model.GetComponentsInChildren<SkinnedMeshRenderer>();
        */

    }

    void Update()
    {

        //着地判定(敵側で必要)
        if (Parent.GetComponent<Character_Parameters>().GetKnock())//これを入れておかないとすぐに着地したことになる
        {
            if (Parent.GetComponent<Character_Parameters>().GetGround())
            {
                //ノックバックして着地した時に動かせるように
                Cpara.SetActive();//動かす
                Cpara.ReverseKnock();
            }
        }
        
    }

    void OnTriggerEnter(Collider col)
    {

        if (col.tag == "Bullet")
        {//タグ"Bullet"が攻撃

            //当たった弾の攻撃力取得
            Attack_Parameter attack = col.gameObject.GetComponent<Attack_Parameter>();

            if (attack.Parent.name != Parent.name)
            {
                //ダメージ処理///////////////////////////////////////////////////////////////

                int damage = attack.power;

                //弱点属性でダメージ2倍
                for (int i = 0; i < Cpara.weak_element.Length; i++)
                {

                    if (attack.GetElement() == Cpara.weak_element[i])
                    {

                        damage *= 2;

                    }

                }

                //耐性属性でダメージ1/2倍
                for (int i = 0; i < Cpara.proof_element.Length; i++)
                {

                    if (attack.GetElement() == Cpara.proof_element[i])
                    {

                        damage /= 2;

                    }

                }

                //無効化属性でダメージ0
                for (int i = 0; i < Cpara.invalid_element.Length; i++)
                {

                    if (attack.GetElement() == Cpara.invalid_element[i])
                    {

                        damage = 0;

                    }

                }

                //弱点部位でダメージ2倍
                if (weak_point)
                {

                    damage *= 2;

                }
                
                //実際のダメージ処理
                if (Cpara.GetF_Damage())
                {
                    Cpara.H_point -= damage;
                    Cpara.SendMessage("Damage",SendMessageOptions.DontRequireReceiver);//とりあえずダメージを受けたことを知らせる
                    ReverseDamage();//ダメージを連続で受けないようにする
                    //StartCoroutine(Blink());
                    Invoke("ReverseDamage", 0.5f);
                }

                //こっから演出////////////////////////////////////////////////////////////////

                //属性エフェクト
                switch (col.GetComponent<Attack_Parameter>().GetElement())
                {
                    case Attack_Parameter.MagicElement.Earth:
                        //effectKind = EffectKind.Earth;
                        break;
                    case Attack_Parameter.MagicElement.Water:
                        effectKind = EffectKind.Water;
                        break;
                    case Attack_Parameter.MagicElement.Fire:
                        effectKind = EffectKind.Fire;
                        break;
                    case Attack_Parameter.MagicElement.Wind:
                        //effectKind = EffectKind.Wind;
                        break;

                }
                Effects[(int)effectKind].transform.position = col.transform.position;//弾が当たった場所
                //一瞬だけ消してつけるともう一回再生してくれる
                Effects[(int)effectKind].SetActive(false);
                Effects[(int)effectKind].SetActive(true);

                Effects[(int)EffectKind.Hit].transform.position = col.transform.position;//弾が当たった場所

                //一瞬だけ消してつけるともう一回再生してくれる
                Effects[(int)EffectKind.Hit].SetActive(false);
                Effects[(int)EffectKind.Hit].SetActive(true);
                
                
            }

            //こっから状態異常///////////////////////////////////////////////////////////

            //ノックバック
            if (attack.GetKnockBack().magnitude > 0)
            {
                Cpara.ReverseKnock();//ノックバック状態ですよ

                //vec2をvec1に向かって角度差分だけ回転させる
                var vec1 = (Parent.gameObject.transform.position - col.gameObject.transform.position);
                var vec2 = attack.GetKnockBack();
                //Y軸は別計算で
                vec1 = new Vector3(vec1.x, 0, vec1.z).normalized;
                vec2 = new Vector3(vec2.x, 0, vec2.z).normalized;
                
                var axis = Vector3.Cross(vec2, vec1);//２つのベクトルに直行するベクトルを求める(外積)
                var ang = Quaternion.LookRotation(vec2).eulerAngles.y - Quaternion.LookRotation(vec1).eulerAngles.y;//二つのベクトルの角度差
                var res = Vector3.zero;//吹っ飛ぶ方向

                if (ang > -180)
                {
                    res = Quaternion.AngleAxis(-ang, axis) * attack.GetKnockBack();//axisを軸にして第1引数の角度だけ回転させる
                }
                else
                {
                    res = Quaternion.AngleAxis(ang, axis) * attack.GetKnockBack();//axisを軸にして第1引数の角度だけ回転させる
                }

                //吹っ飛ぶ
                iTween.MoveTo(Parent.gameObject, iTween.Hash(
                        "position", new Vector3(
                        Parent.gameObject.transform.position.x + res.x,
                        Parent.gameObject.transform.position.y + attack.GetKnockBack().y,
                        Parent.gameObject.transform.position.z + res.z)
                        /*Parent.gameObject.transform.position.x - (col.gameObject.transform.position.x - Parent.gameObject.transform.position.x) * attack.GetKnockBack().x,
                        Parent.gameObject.transform.position.y + attack.GetKnockBack().y,
                        Parent.gameObject.transform.position.z - (col.gameObject.transform.position.z - Parent.gameObject.transform.position.z) * attack.GetKnockBack().z)*/
                        //- (col.gameObject.transform.position - Parent.gameObject.transform.position)
                        /*Parent.transform.TransformDirection(attack.GetKnockBack())*/,
                        "time", 0.3f/*,
                            "easetype", iTween.EaseType.easeOutBack*/)
                        );
                Cpara.SetKeylock();//行動不能だったと思う
            }

            //毒
            if (attack.GetAilment() == Attack_Parameter.Ailment.Poison)
            {
                if (!Cpara.flag_poison)//毒状態は重ならない
                {
                    Cpara.flag_poison = true;
                    Effects[(int)EffectKind.Poison].SetActive(true);
                    StartCoroutine(Poison());//一回動かせばいい
                }
            }
        }
    }

    //反転を時間を遅らせて行うため
    void ReverseDamage()
    {
        Cpara.Reverse_Damage();
    }
    
    //状態異常管理//////////////////////////////////////////////

    IEnumerator Poison()
    {
        //自然治癒する毒
        for (int i = 0; i < 5; i++) {//ここで指定した回数分ダメージ

            yield return new WaitForSeconds(1.0f);//継続ダメージの入る間隔

            Cpara.H_point -= 1;//スリップダメージの量

            //毒を止める
            if (i == 4)//時間で切るべき？
            {

                Cpara.flag_poison = false;//毒の自然治癒
                Effects[(int)EffectKind.Poison].SetActive(false);
                
            }

        }

        //自然治癒しない毒
        /*
        while(true)
        {
            yield return new WaitForSeconds(1.0f);//継続ダメージの入る間隔

            Cpara.H_point -= 1;//スリップダメージの量

            //毒を回復する条件ここ々に入れる
        }
        */

    }
}
