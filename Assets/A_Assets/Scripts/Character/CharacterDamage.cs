using UnityEngine;
using System.Collections;

public class CharacterDamage : MonoBehaviour {

    /******************************************************************************/
    /** @brief キャラクターのダメージ処理全般
    * @date 2016/05/29
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
*/
    /******************************************************************************/
    /* 更新履歴
    *  プレイヤも敵も一緒くたにできるように
    *  とりあえず敵だけ？
    */
    /******************************************************************************/
    
    public GameObject Parent;//このあたり判定を持つキャラ
    public bool weak_point = false;
    private Character_Parameters Cpara;

    private Renderer[] Renderer;//レンダー1
    private Renderer[] SkinRenderer;//レンダー2
    public GameObject Model;//モデル

    //コルーチン
    private Coroutine coroutine;
    private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
    private bool isCoroutine = false;

    //演出
    public GameObject[] Effects;//出すエフェクト

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
        
        //今はいらない
        /*
        Renderer = Model.GetComponentsInChildren<MeshRenderer>();
        SkinRenderer = Model.GetComponentsInChildren<SkinnedMeshRenderer>();
        */

    }

    void Update()
    {

        //とりあえず止めとく
        /*
        if (Cpara.flag_poison)
        {

            coroutine = StartCoroutine(Poison());

        }
        */

        /*
        if (Parent.GetComponent<Character_Parameters>().GetF_Damage())
        {
            if (Parent.GetComponent<Character_Parameters>().GetGround())
            {
                //Cpara.SetActive();//動かす
            }
        }
        */

    }

    IEnumerator Poison()
    {

        if (isCoroutine) { yield break; }
        isCoroutine = true;

        yield return new WaitForSeconds(5.0f);

        Cpara.H_point -= 1;
        count++;
        if (count == 10)
        {

            Cpara.flag_poison = false;//毒の自然治癒

        }
        isCoroutine = false;

    }

    void OnTriggerEnter(Collider col)
    {

        if (col.tag == "Bullet")
        {//タグ"Bullet"が攻撃

            //当たった弾の攻撃力取得
            Attack_Parameter attack = col.gameObject.GetComponent<Attack_Parameter>();

            if (attack.Parent.name != Parent.name)
            {

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

                //こっから状態異常///////////////////////////////////////////////////////////

                if (attack.GetAilment() == "Poison")
                {

                    Cpara.flag_poison = true;

                }

                //こっからノックバック
                if (Parent.GetComponent<Rigidbody>() != null)
                {
                    if (attack.GetKnockBack().magnitude > 0)
                    {
                        //吹っ飛ぶ
                        iTween.MoveTo(Parent.gameObject, iTween.Hash(
                                "position", Parent.gameObject.transform.position + Parent.transform.TransformDirection(attack.GetKnockBack()),
                                "time", 0.8f,
                                "easetype", iTween.EaseType.easeOutBack)
                                );
                        Cpara.SetKeylock();//行動不能だったと思う
                    }
                }

                //こっから演出
                //とりあえず止めとく
                /*
                for (int i = 0; i < Effects.Length; i++)
                {
                    //Effects[i].transform.parent = null;//子供にしとくとたいてい消える
                    Effects[i].SetActive(true);
                    StartCoroutine(ErasseEffect(Effects[i]));
                }
                */
            }


        }
    }

    void ReverseDamage()
    {
        Cpara.Reverse_Damage();
    }
    
    IEnumerator ErasseEffect(GameObject Effect)
    {
        yield return new WaitForSeconds(0.5f);//エフェクトが出てる時間

        Effect.SetActive(false);
    }
}
