using UnityEngine;
using System.Collections;

public class DarkLilith_Manager : MonoBehaviour {

    //ダークリリス専用ダメージ処理//

    public GameObject Parent;//このあたり判定を持つキャラ
    public bool weak_point = false;
    private Enemy_ControllerZ ecZ;

    //コルーチン
    private Coroutine coroutine;
    private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
    private bool isCoroutine = false;

    void Start()
    {

        ecZ = Parent.GetComponent<Enemy_ControllerZ>();

    }

    void Update()
    {

        if (ecZ.flag_poison)
        {

            coroutine = StartCoroutine(Poison());

        }

    }
    IEnumerator Poison()
    {

        if (isCoroutine) { yield break; }
        isCoroutine = true;

        yield return new WaitForSeconds(5.0f);

        ecZ.H_point -= 1;
        count++;
        if (count == 10)
        {

            ecZ.flag_poison = false;//毒の自然治癒

        }
        isCoroutine = false;

    }

    void OnTriggerEnter(Collider col)
    {

        if (col.tag == "Bullet")
        {//タグ"Bullet"が攻撃

            //当たった弾の攻撃力取得
            Attack_Parameter attack = col.gameObject.GetComponent<Attack_Parameter>();

            if (attack.Parent != Parent)
            {

                int damage = attack.power;

                //弱点属性でダメージ2倍
                for (int i = 0; i < ecZ.weak_element.Length; i++)
                {

                    if (attack.GetElement() == ecZ.weak_element[i])
                    {

                        damage *= 2;

                    }

                }

                //耐性属性でダメージ1/2倍
                for (int i = 0; i < ecZ.proof_element.Length; i++)
                {

                    if (attack.GetElement() == ecZ.proof_element[i])
                    {

                        damage /= 2;

                    }

                }

                //弱点部位でダメージ2倍
                if (weak_point)
                {

                    damage *= 2;

                }

                //実際のダメージ処理
                ecZ.H_point -= damage;

                //こっから状態異常///////////////////////////////////////////////////////////

                if (attack.GetAilment() == "Poison")
                {

                    ecZ.flag_poison = true;

                }

            }
        }
    }
}
