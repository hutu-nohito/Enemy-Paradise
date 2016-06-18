using UnityEngine;
using System.Collections;

public class Enemy_Base : Character_Parameters
{

    /******************************************************************************/
    /** @brief 敵基底クラス
    * @date 2016/05/08
    * @author 石川
    * @param[in] m_fringe 干渉縞の計算結果を格納
    * @func BaseStart 共通の初期化(敵毎のStart()の中で呼び出す)
    * @func BaseUpdate 共通の処理(敵毎のUpdate()の中で呼び出す)
    */
    /******************************************************************************/
    /* 更新履歴
    *   どの敵にも必要な初期化、判定はこっち
    *   どんな敵にでもアニメーションはついてるのもとする(ない場合はダミーつけとく)
    */
    /******************************************************************************/

    [System.NonSerialized]
    public Animator animator;

    [System.NonSerialized]
    public GameObject Player;

    [System.NonSerialized]
    public Vector3 Old_position;//計測用の1フレーム前の位置

    public GameObject Model;//アーマチュア
    private SkinnedMeshRenderer Skin;
    private float color = 0.25f;//透明度

    // Use this for initialization
    public void BaseStart () {

        //アニメーションセット
        animator = GetComponentInChildren<Animator>();

        //プレイヤをセット
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player");
        }

        //初期位置
        home_position = transform.position;//初期位置を
        Old_position = home_position;

        Skin = Model.GetComponentInChildren<SkinnedMeshRenderer>();

    }
	
	// Update is called once per frame
	public void BaseUpdate () {

        //現在向いている方向をVector3で保持
        SetDirection(transform.TransformDirection(Vector3.forward).normalized);

        //フェードも登場時・死亡時に使いたいのでここに
        StartCoroutine(Fade());

        //進行方向を取る
        if (transform.position != Old_position)//位置が変化していたら
        {
            SetMove((transform.position - Old_position).normalized);//進行方向の向きベクトルを渡す
        }

        Old_position = transform.position;//OldPosを更新しないと動きません
    }

    //残像
    public IEnumerator AfterImage()
    {
        yield return new WaitForSeconds(0);

        GameObject AfterImage = Instantiate(Model);
        AfterImage.transform.position = Old_position;
        AfterImage.transform.rotation = Quaternion.Euler(Model.transform.eulerAngles.x, transform.eulerAngles.y, Model.transform.eulerAngles.z);

        SkinnedMeshRenderer[] AfterImageMesh = AfterImage.GetComponentsInChildren<SkinnedMeshRenderer>();

        //消したいメッシュの時だけ消す
        for (int i = 0; i < AfterImageMesh.Length; i++)
        {
            Material[] AfterImageMaterial = AfterImageMesh[i].materials;

            //消したいマテリアルの時だけ消す(条件分岐で何とかする)
            for (int j = 0; j < AfterImageMaterial.Length; j++)
            {
                //RenderingModeを切り替えるためにはこの7個の設定を変えなければならない(Standard Shader)
                AfterImageMaterial[j].SetFloat("_Mode", 2);//たぶんFade
                AfterImageMaterial[j].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                AfterImageMaterial[j].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                AfterImageMaterial[j].SetInt("_ZWrite", 0);
                AfterImageMaterial[j].DisableKeyword("_ALPHATEST_ON");
                AfterImageMaterial[j].EnableKeyword("_ALPHABLEND_ON");
                AfterImageMaterial[j].DisableKeyword("_ALPHAPREMULTIPLY_ON");

                AfterImageMaterial[j].color = new Color(AfterImageMaterial[j].color.r, AfterImageMaterial[j].color.g, AfterImageMaterial[j].color.b, 0.2f); ;
                
            }

        }

        Destroy(AfterImage, 0.5f / speed);

    }

    public IEnumerator Fade()
    {
        yield return new WaitForSeconds(0);

        Material[] ml = Skin.materials;

        if (flag_fade)
        {
            //RenderingModeを切り替えるためにはこの7個の設定を変えなければならない(Standard Shader)
            for(int j = 0;j < ml.Length; j++)
            {
                ml[j].SetOverrideTag("RenderType", "Transparent");
                ml[j].SetFloat("_Mode", 2);//たぶんFade
                ml[j].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                ml[j].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                ml[j].SetInt("_ZWrite", 0);
                ml[j].renderQueue = -1;
            }
            
            if (color > 0)
            {
                color -= Time.deltaTime * 1;
            }
            else
            {
                color = 0;
            }
        }
        else
        {
            
            if (color < 1)
            {
                color += Time.deltaTime * 1;
            }
            else
            {
                color = 1;
                //RenderingModeを切り替えるためにはこの7個の設定を変えなければならない(Standard Shader)
                for (int j = 0; j < Skin.materials.Length; j++)
                {
                    ml[j].SetOverrideTag("RenderType", "");
                    ml[j].SetFloat("_Mode", 0);//Opaque
                    ml[j].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    ml[j].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    ml[j].SetInt("_ZWrite", 1);
                    ml[j].renderQueue = 3000;
                }
            }
        }

        for (int i = 0; i < Skin.materials.Length; i++)
        {
            Skin.materials[i].color = new Color(Skin.materials[i].color.r, Skin.materials[i].color.g, Skin.materials[i].color.b, color);
        }
        
    }

}
