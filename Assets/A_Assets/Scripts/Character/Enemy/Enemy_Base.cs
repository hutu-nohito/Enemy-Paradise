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
    *   プレイヤーとの距離を常に拾ってるので重くなったら消す
    *   プレイヤーを基準とした座標「Playeraxis」を作る
    */
    /******************************************************************************/

    //publicにしとかないと継承先で使えないので注意

    [System.NonSerialized]
    public Animator animator;

    [System.NonSerialized]
    public GameObject Player;

    private Vector3 Old_position;//計測用の1フレーム前の位置

    public bool flag_vsCom;//コンピュータ同士で戦わせたいときにつける(コロシアム専用)(1対1しか想定してない)

    public GameObject Model;//アーマチュア(残像用)
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
        if (flag_vsCom)
        {
            //とりあえず2体しかいない想定。自分と名前が同じなら違う方が敵
            GameObject[] enemy = new GameObject[2];
            enemy = GameObject.FindGameObjectsWithTag("Enemy");
            Player = enemy[0];
            if(Player.name == this.name)
            {
                Player = enemy[1];
            }
        }

        //初期位置
        home_position = transform.position;//初期位置を
        Old_position = home_position;

        //メッシュは分かれてたりするから対策した方がいいかも
        Skin = Model.GetComponentInChildren<SkinnedMeshRenderer>();

    }
	
	// Update is called once per frame
	public void BaseUpdate () {

        //Y軸以外の回転を補正(基本的に0で固定、見上げたりしたい場合は別途モーションを作製)
        if(GetComponent<Rigidbody>() != null)//RigidBodyの有無
        {
            if(GetComponent<Rigidbody>().constraints == RigidbodyConstraints.FreezePositionX)//Xが止められてたら
            {
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            }
        }

        //現在向いている方向をVector3で保持
        SetDirection(transform.TransformDirection(Vector3.forward).normalized);

        //フェードも登場時・死亡時に使いたいのでここに(マテリアルで対応するのは難しい)
        StartCoroutine(Fade());

        //進行方向を取る
        if (transform.position != Old_position)//位置が変化していたら
        {
            SetMove((transform.position - Old_position).normalized);//進行方向の向きベクトルを渡す
        }

        //残像を出す
        if (GetAfterImage())
        {
            StartCoroutine(AfterImage());
        }

        Old_position = transform.position;//OldPosを更新しないと動きません
    }

    //プレイヤーとの位置関係/////////////////////////////////////
    //プレイヤーとの距離
    public float GetDistansP()
    {
        Vector3 dist = Player.transform.position - transform.position;
        return dist.magnitude;
    }

    //プレイヤーから見た敵の座標
    public Vector3 GetPositionP()
    {
        Vector3 pos = transform.position - Player.transform.position;
        return pos;
    }

    //内積(Playerを基準にした敵の角度)
    public float GetAngleP()
    {
        //内積(Playerを基準にした敵の角度)
        //(どっちかの位置が0だとまずいのでずらしておくようにする)
        Vector3 PlayerEnemyPos = GetPositionP();
        float EnemyLength = Mathf.Sqrt(PlayerEnemyPos.x * PlayerEnemyPos.x + PlayerEnemyPos.z * PlayerEnemyPos.z);
        float dot_EP = PlayerEnemyPos.x;//内積
        float theta = Mathf.Acos(dot_EP / EnemyLength);//中で複雑な計算はしないほうがいい

        return theta;//radian
    }

    //敵とプレイヤーの位置関係によって座標を入れ替える
    public Vector3 AffineRot(Vector3 Position)
    {
        var theta = GetAngleP();
        var cal = Vector3.zero;
        var relativePos = Position - Player.transform.position;
        cal.x = relativePos.x * Mathf.Cos(theta) - relativePos.z * Mathf.Sin(theta);
        cal.z = relativePos.x * Mathf.Sin(theta) + relativePos.z * Mathf.Cos(theta);
        var calPos = cal;//座標に代入
        calPos.y = Position.y - Player.transform.position.y;//ジャンプさせるからYの値は変えない

        return calPos + Player.transform.position;//ワールド座標を返す

    }

    //プレイヤーからみてワールド座標系でどこにいるか
    public enum Direction
    {
        front,//(316~45)
        right,//(46~135)
        back,//(136~225)
        left,//(226~315)

    }
    public Direction GetDirectionP()
    {
        
        int thetaDegree = (int)(GetAngleP() * 180 / Mathf.PI);
        Direction state = Direction.front;
        if (GetPositionP().x >=0 && GetPositionP().z >= 0)
        {
            if (thetaDegree >= 45 && thetaDegree < 135)
            {
                state = Direction.front;
            }
            if (thetaDegree >= 0 && thetaDegree < 45)
            {
                state = Direction.right;
            }
        }
        if (GetPositionP().x < 0 && GetPositionP().z >= 0)
        {
            if (thetaDegree >= 45 && thetaDegree < 135)
            {
                state = Direction.front;
            }
            if (thetaDegree >= 135 && thetaDegree < 180)
            {
                state = Direction.left;
            }
        }
        if (GetPositionP().x < 0 && GetPositionP().z < 0)
        {
            if (thetaDegree >= 45 && thetaDegree < 135)
            {
                state = Direction.back;
            }
            if (thetaDegree >= 0 && thetaDegree < 45)
            {
                state = Direction.right;
            }
        }
        if (GetPositionP().x >= 0 && GetPositionP().z < 0)
        {
            if (thetaDegree >= 45 && thetaDegree < 135)
            {
                state = Direction.back;
            }
            if (thetaDegree >= 135 && thetaDegree < 180)
            {
                state = Direction.left;
            }
        }
        
        return state;
    }
    ////////////////////////////////////////////////////////

    //残像
    public IEnumerator AfterImage()
    {
        yield return new WaitForSeconds(0);

        GameObject AfterImage = Instantiate(Model);
        AfterImage.transform.position = Old_position;
        //Debug.Log(Model.transform.eulerAngles.z);
        //AfterImage.transform.rotation = Quaternion.Euler(Model.transform.eulerAngles.x, transform.eulerAngles.y, Model.transform.eulerAngles.z);
        AfterImage.transform.rotation = Quaternion.Euler(Model.transform.eulerAngles.x, transform.eulerAngles.y, 0);//zが必要になったら何とかする

        SkinnedMeshRenderer[] AfterImageMesh = AfterImage.GetComponentsInChildren<SkinnedMeshRenderer>();

        //消したいメッシュの時だけ消す
        for (int i = 0; i < AfterImageMesh.Length; i++)
        {
            Material[] AfterImageMaterial = AfterImageMesh[i].materials;

            //消したいマテリアルの時だけ消す(条件分岐で何とかする)
            for (int j = 0; j < AfterImageMaterial.Length; j++)
            {
                //RenderingModeを切り替えるためにはこの7個の設定を変えなければならない(Standard Shader)
                //AfterImageMaterial[j].SetOverrideTag("RenderType", "Transparent");
                //AfterImageMaterial[j].SetFloat("_Mode", 2);//たぶんFade
                //AfterImageMaterial[j].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                //AfterImageMaterial[j].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                //AfterImageMaterial[j].SetInt("_ZWrite", 0);
                //AfterImageMaterial[j].DisableKeyword("_ALPHATEST_ON");
                //AfterImageMaterial[j].EnableKeyword("_ALPHABLEND_ON");
                //AfterImageMaterial[j].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                //AfterImageMaterial[j].renderQueue = 3000;

                AfterImageMaterial[j].color = new Color(AfterImageMaterial[j].color.r, AfterImageMaterial[j].color.g, AfterImageMaterial[j].color.b, 0.5f); ;
                
            }

        }

        Destroy(AfterImage, 1 / speed);

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
                ml[j].DisableKeyword("_ALPHATEST_ON");
                ml[j].EnableKeyword("_ALPHABLEND_ON");
                ml[j].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                ml[j].renderQueue = 3000;
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
                    ml[j].DisableKeyword("_ALPHATEST_ON");
                    ml[j].EnableKeyword("_ALPHABLEND_ON");
                    ml[j].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    ml[j].renderQueue = -1;
                }
            }
        }

        for (int i = 0; i < Skin.materials.Length; i++)
        {
            ml[i].color = new Color(Skin.materials[i].color.r, Skin.materials[i].color.g, Skin.materials[i].color.b, color);
        }
        
    }

}
