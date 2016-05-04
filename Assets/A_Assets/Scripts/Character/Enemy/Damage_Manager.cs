using UnityEngine;
using System.Collections;

public class Damage_Manager : MonoBehaviour {

	//敵の実際のダメージ処理を行う//

	public GameObject Parent;//このあたり判定を持つキャラ
	public bool weak_point = false;
	private Enemy_ControllerZ ecZ;

    private Renderer[] Renderer;//レンダー1
    private Renderer[] SkinRenderer;//レンダー2
    public GameObject Model;//モデル

    //コルーチン
    private Coroutine coroutine;
    private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
    private bool isCoroutine = false;

    //演出
    public GameObject[] Effects;//出すエフェクト

    //ノックバック用
    private bool flag_knockback = false;
    public Vector3 EndPos;
    private Vector3 deltaPos;
    private float time = 2.0f;//ここを吹っ飛ぶ距離に応じて変えたい
    private float elapsedTime;
    private bool flag_stop = false;

    void Start () {

		ecZ = Parent.GetComponent<Enemy_ControllerZ>();

        Renderer = Model.GetComponentsInChildren<MeshRenderer>();
        SkinRenderer = Model.GetComponentsInChildren<SkinnedMeshRenderer>();

    }

	void Update () {

        if (ecZ.flag_poison)
        {

            coroutine = StartCoroutine(Poison());

        }

        //ちょっとだけふわふわさせる(y軸だけでいい)
        if (flag_knockback)
        {
            Parent.GetComponent<Rigidbody>().AddForce(new Vector3(0,2,0), ForceMode.Force);//上向きの力
            //Parent.transform.position += deltaPos * Time.deltaTime;
            elapsedTime += Time.deltaTime;

            if (elapsedTime > time)
            {

                elapsedTime = 0;
                flag_knockback = false;
                flag_stop = true;

            }
        }

        if (flag_stop)
        {
            elapsedTime += Time.deltaTime;
            Parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Parent.GetComponent<Rigidbody>().useGravity = false;

            if (elapsedTime > time)
            {

                elapsedTime = 0;
                flag_stop = false;
                Parent.GetComponent<SimpleMove>().Control.x = 0;//回さない
                Parent.GetComponent<Rigidbody>().useGravity = true;

            }
            
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

	void OnTriggerEnter(Collider col){

		if (col.tag == "Bullet") {//タグ"Bullet"が攻撃
			
			//当たった弾の攻撃力取得
			Attack_Parameter attack = col.gameObject.GetComponent<Attack_Parameter> ();

			if (attack.Parent.name == "Player") {

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

                //無効化属性でダメージ0
                for (int i = 0; i < ecZ.invalid_element.Length; i++)
                {

                    if (attack.GetElement() == ecZ.invalid_element[i])
                    {

                        damage = 0;

                    }

                }

                //弱点部位でダメージ2倍
                if (weak_point) {
				
					damage *= 2;
				
				}


                //実際のダメージ処理
                if (ecZ.GetF_Damage())
                {
                    ecZ.H_point -= damage;
                    ecZ.Damage();//とりあえずダメージを受けたことを知らせる
                    ecZ.Reverse_Damage();//ダメージを連続で受けないようにする
                    //StartCoroutine(Blink());
                    Invoke("Reverse_Damage", 0.5f);
                }

                //こっから状態異常///////////////////////////////////////////////////////////

                if (attack.GetAilment() == "Poison")
                {

                    ecZ.flag_poison = true;

                }

                //こっからノックバック
                if (Parent.GetComponent<Rigidbody>() != null)
                {
                    if (attack.GetKnockBack().magnitude > 0)
                    {
                        Parent.GetComponent<SimpleMove>().Control.x = 90;//回す
                        Parent.GetComponent<Rigidbody>().AddForce(Parent.transform.TransformDirection(attack.GetKnockBack()), ForceMode.Impulse);//Yが上下逆
                        flag_knockback = true;
                        ecZ.SetKeylock();//行動不能だったと思う
                        Invoke("SetActive", 1);
                    }
                }

                //こっからノックバック
                /*if (attack.GetKnockBack().magnitude > 0)
                {

                    //EndPos = Parent.transform.position + col.transform.TransformDirection(attack.GetKnockBack());
                    EndPos = Parent.transform.position + Parent.transform.TransformDirection(attack.GetKnockBack());
                    deltaPos = (EndPos - Parent.transform.position) / time;
                    flag_knockback = true;
                    ecZ.SetKeylock();//行動不能だったと思う
                    Invoke("SetActive", time);

                }*/

                //こっから演出
                for (int i = 0; i < Effects.Length; i++)
                {
                    //Effects[i].transform.parent = null;//子供にしとくとたいてい消える
                    Effects[i].SetActive(true);
                    StartCoroutine(ErasseEffect(Effects[i]));
                }

            }

                        
		}
	}

    IEnumerator Blink()
    {

        while (!ecZ.GetF_Damage())
        {
            for (int i = 0; i < Renderer.Length; i++)
            {
                Renderer[i].enabled = !Renderer[i].enabled;
            }
            for (int i = 0; i < SkinRenderer.Length; i++)
            {
                SkinRenderer[i].enabled = !SkinRenderer[i].enabled;

            }

            yield return new WaitForSeconds(0.1f);
        }

        //なんか消えてることがあったのでとりあえず強制的につける
        for (int i = 0; i < Renderer.Length; i++)
        {
            Renderer[i].enabled = true;
        }
        for (int i = 0; i < SkinRenderer.Length; i++)
        {
            SkinRenderer[i].enabled = true;

        }

    }

    void Reverse_Damage()
    {
        ecZ.Reverse_Damage();//無敵時間解除
        ecZ.NotDamage();//ダメージ受けおわり
    }

    void SetActive()
    {
        ecZ.SetActive();
    }

    IEnumerator ErasseEffect(GameObject Effect)
    {
        yield return new WaitForSeconds(0.5f);//エフェクトが出てる時間

        Effect.SetActive(false);
    }
}
