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
    */
    /******************************************************************************/
    
    public GameObject Parent;//このあたり判定を持つキャラ
    public bool weak_point = false;
    private Character_Parameters Cpara;//キャラクタのパラメタ

    private Renderer[] Renderer;//レンダー1
    private Renderer[] SkinRenderer;//レンダー2
    public GameObject Model;//モデル

    //コルーチン
    private Coroutine coroutine;
    private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
    private bool isCoroutine = false;

    //演出
    public GameObject[] Effects;//出すエフェクト
    private List<Vector3> Effect_basePos = new List<Vector3>();//エフェクトの基準位置

    void Start()
    {
        if (Parent.gameObject.tag == "Player")
        {
            Cpara = Parent.GetComponent<Player_ControllerZ>();//プレイヤーだったらこっち
        }
        else
        {
            Cpara = Parent.GetComponent<monster_Cont2>();//敵だったらこっち
        }

        //エフェクトの基準位置を取得
        for (int i = 0 ;i < Effects.Length ; i++)
        {
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
        if (Parent.GetComponent<Character_Parameters>().GetF_Damage())//これはCharacterController用
        {
            if (Parent.GetComponent<Character_Parameters>().GetGround())
            {
                Cpara.SetActive();//動かす
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
                    //Cpara.Damage();//とりあえずダメージを受けたことを知らせる
                    ReverseDamage();//ダメージを連続で受けないようにする
                    //StartCoroutine(Blink());
                    Invoke("ReverseDamage", 0.5f);
                }

                //こっからノックバック////////////////////////////////////////////////////////////////

                if (attack.GetKnockBack().magnitude > 0)
                {
                    //vec2をvec1に向かって角度差分だけ回転させる
                    var vec1 = (col.gameObject.transform.position - Parent.gameObject.transform.position);
                    var vec2 = attack.GetKnockBack();
                    var axis = Vector3.Cross(vec2, vec1);
                    var res = Quaternion.AngleAxis(Vector3.Angle(vec2,vec1), axis) * vec2;

                    //吹っ飛ぶ(後ろか上にしか吹っ飛ばないけど横に飛ぶことはないだろうしいいか)
                    iTween.MoveTo(Parent.gameObject, iTween.Hash(
                            "position", new Vector3(
                            Parent.gameObject.transform.position.x - res.x,
                            Parent.gameObject.transform.position.y + attack.GetKnockBack().y,
                            Parent.gameObject.transform.position.z - res.z)
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

                //こっから演出////////////////////////////////////////////////////////////////

                //(できれば攻撃された部分にエフェクトを出したい)
                //(そのうち地水火風で分ける)
                for (int i = 0; i < Effects.Length; i++)
                {

                    /*Effects[i].transform.localPosition = Effect_basePos[i];//位置合わせ
                    Effects[i].transform.localPosition -= new Vector3(
                        (col.transform.position.x - Parent.transform.position.x) * 1.5f,
                        (col.transform.position.y - Parent.transform.position.y) * -0.1f,
                        (col.transform.position.z - Parent.transform.position.z) * 1.5f);//位置合わせ
                    */

                    //位置合わせ
                    Effects[i].transform.position = col.transform.position;//弾が当たった場所

                    //一瞬だけ消してつけるともう一回再生してくれる
                    Effects[i].SetActive(false);
                    Effects[i].SetActive(true);
                    //StartCoroutine(ErasseEffect(Effects[i]));
                }
                
            }

            //こっから状態異常///////////////////////////////////////////////////////////

            //弾に付加されてるからここでやるのが手っ取り早い
            if (attack.GetAilment() == "Poison")
            {
                if (!Cpara.flag_poison)//毒状態は重ならない
                {
                    Cpara.flag_poison = true;
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

    //エフェクト///////////////////////////////////////////////

    IEnumerator Effect()
    {
        if (isCoroutine) { yield break; }
        isCoroutine = true;

        yield return new WaitForSeconds(5.0f);
    }
    
    //状態異常管理//////////////////////////////////////////////

    IEnumerator Poison()
    {
        
        while (true)
        {
            yield return new WaitForSeconds(1.0f);//継続ダメージの入る間隔

            Cpara.H_point -= 1;//スリップダメージの量
            count++;
            Debug.Log("www");

            //毒を止める
            if (count == 10)//時間で切るべき？
            {

                Cpara.flag_poison = false;//毒の自然治癒
                StopCoroutine(Poison());
            }

        }

    }
}
