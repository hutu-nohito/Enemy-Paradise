using UnityEngine;
using System.Collections;

public class Player_Manager : MonoBehaviour {

	//プレイヤーのダメージ処理。状態異常処理//
	
	public GameObject Parent;//このあたり判定を持つキャラ
	private Player_ControllerZ pcZ;

    private Renderer[] PlayerRenderer;//プレイヤのレンダー1
    private Renderer[] PlayerSkinRenderer;//プレイヤのレンダー2
    public GameObject PlayerModel;//プレイヤのモデル

    //コルーチン
    private Coroutine coroutine;
	private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
	private bool isCoroutine = false;

    //ノックバック用
    private bool flag_knockback = false;
    public Vector3 EndPos;
    private Vector3 deltaPos;
    private float time = 0.5f;
    private float elapsedTime;


    void Start () {
		
		pcZ = Parent.GetComponent<Player_ControllerZ>();

        PlayerRenderer = PlayerModel.GetComponentsInChildren<MeshRenderer>();
        PlayerSkinRenderer = PlayerModel.GetComponentsInChildren<SkinnedMeshRenderer>();

    }
	
	void Update () {

		if(pcZ.flag_poison){

			coroutine = StartCoroutine(Poison());

		}

        if (flag_knockback)
        {
            Parent.transform.position += deltaPos * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            if (elapsedTime > time)
            {

                //Parent.transform.position = EndPos;//正確に動かす必要はない
                elapsedTime = 0;
                flag_knockback = false;
                
            }
        }

    }

	IEnumerator Poison (){

		if(isCoroutine){yield break;}
		isCoroutine = true;

		yield return new WaitForSeconds(5.0f);

		pcZ.H_point -= 1;
		count++;
		if(count == 10){

			pcZ.flag_poison = false;//毒の自然治癒

		}
		isCoroutine = false;

	}
	
	void OnTriggerEnter(Collider col){

		if (col.tag == "Bullet") {//タグ"Bullet"が攻撃
			
			//当たった弾の攻撃力取得
            Attack_Parameter attack = col.gameObject.GetComponent<Attack_Parameter>();

			if(attack.Parent != Parent){

				int damage = attack.power;

                //弱点属性でダメージ2倍
                for (int i = 0; i < pcZ.weak_element.Length; i++) {
					
					if (attack.GetElement() == pcZ.weak_element [i]) {
						
						damage *= 2;
						
					}
					
				}
				
				//耐性属性でダメージ1/2倍
				for (int i = 0; i < pcZ.proof_element.Length; i++) {
					
					if (attack.GetElement() == pcZ.proof_element [i]) {
						
						damage /= 2;
						
					}
					
				}

                //実際のダメージ処理
                if (pcZ.GetF_Damage())
                {
                    pcZ.H_point -= damage;
                    //pcZ.Reverse_Damage();//ダメージを連続で受けないようにする
                    StartCoroutine(Blink());//点滅
                    //Invoke("Reverse_Damage", 3);
                }
                

                //こっから状態異常///////////////////////////////////////////////////////////

                if (attack.GetAilment() == "Poison")
                {
					
					pcZ.flag_poison = true;
					
				}

                //こっからノックバック
                if (attack.GetKnockBack().magnitude > 0)
                {
                    //time = attack.GetKnockBack().magnitude;
                    EndPos = Parent.transform.position + col.transform.TransformDirection(attack.GetKnockBack());
                    deltaPos = (EndPos - Parent.transform.position) / time;
                    flag_knockback = true;
                    pcZ.SetKeylock();//行動不能だったと思う
                    Invoke("SetActive", time);
                }
            }
		}
	}

    IEnumerator Blink()
    {

        while (!pcZ.GetF_Damage())
        {
            for (int i = 0; i < PlayerRenderer.Length; i++)
            {
                PlayerRenderer[i].enabled = !PlayerRenderer[i].enabled;
            }
            for (int i = 0; i < PlayerSkinRenderer.Length; i++)
            {
                PlayerSkinRenderer[i].enabled = !PlayerSkinRenderer[i].enabled;
            }

            yield return new WaitForSeconds(0.1f);
        }

        //消えるの対策
        for (int i = 0; i < PlayerRenderer.Length; i++)
        {
            PlayerRenderer[i].enabled = true;
        }
        for (int i = 0; i < PlayerSkinRenderer.Length; i++)
        {
            PlayerSkinRenderer[i].enabled = true;
        }

    }

    void Reverse_Damage()
    {
        StopCoroutine("Blink");
        pcZ.Reverse_Damage();//無敵時間解除
    }

    void SetActive()
    {
        pcZ.SetActive();
    }
}
