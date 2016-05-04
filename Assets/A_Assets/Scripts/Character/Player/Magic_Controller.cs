using UnityEngine;
using System.Collections;
using System.Collections.Generic;//List用

public class Magic_Controller : MonoBehaviour{

    /*

    魔法の選択
    魔法発動

    */

    private Player_ControllerZ Pz;
    private Static _static;

    //変数(ex:time)////////////////////////////////

    public int magic_num = 0;//選択している弾の種類
    private int old_magic_num = 0;//1つ前の選択してる魔法の種類
    public int[] selectmagic = new int[5];//選ばれた魔法の番号

    private bool isHold;//ホールド中　魔法が切り替わらないようにする
    private List<GameObject> Bullet0 = new List<GameObject>();//0個目に登録されてる弾の現在存在してる数
    //バレット切り替えた時用(それぞれの魔法の弾を格納)
    private List<GameObject> oldBullet0 = new List<GameObject>();
    private List<GameObject> oldBullet1 = new List<GameObject>();
    private List<GameObject> oldBullet2 = new List<GameObject>();
    private List<GameObject> oldBullet3 = new List<GameObject>();
    private List<GameObject> oldBullet4 = new List<GameObject>();
    //増やすのは各自でやってもらう
    public void AddExistBullet(GameObject bullet)//バレットを増やす
    {
        Bullet0.Add(bullet);
    }

    //GameObject/////////////////////////////////////////////
    public GameObject[] Magic;//魔法の大本。PlayerのみこれをMuzzleとして使う
    public GameObject[] SelectMagic = new GameObject[5];//隙間にセットされた魔法
    

	//コルーチン
	private Coroutine coroutine;
	//private int count;//汎用のカウント用の箱(使い終わったら0に戻すこと)
	private bool isCoroutine = false;

    //使うもの

    private Magic_Parameter.InputType InputType;

    void Awake()
    {

        Pz = GetComponent<Player_ControllerZ>();
        _static = GameObject.FindGameObjectWithTag("Manager").GetComponent<Static>();

        //選択されてる魔法の番号を渡す。
        MagicSet (8,2,1,6,8);
        /*
        MagicSet(
            _static.SelectMagicID[0],
            _static.SelectMagicID[1],
            _static.SelectMagicID[2], 
            _static.SelectMagicID[3],
            _static.SelectMagicID[4]);
        */

        for (int i = 0;i < Magic.Length;i++){

            Magic[i].GetComponent<Magic_Parameter>().SetParent(this.gameObject);//親はプレイヤー

        }
        
    }

    void MagicSet(int a,int b,int c,int d,int e)
    {
        /*
         * 
         * Magic[0]にもらったIDと同じ魔法をInstanseして格納
         * Magic[0]をPlayerの子オブジェクトに
         * インスタンスしたものにPlayerが親だと伝える
         * 
         */

        //選ばれた魔法を格納
        SelectMagic[0] = Magic[a];
        SelectMagic[1] = Magic[b];
        SelectMagic[2] = Magic[c];
        SelectMagic[3] = Magic[d];
        SelectMagic[4] = Magic[e];

        selectmagic[0] = a;
        selectmagic[1] = b;
        selectmagic[2] = c;
        selectmagic[3] = d;
        selectmagic[4] = e;

    }

    void Update()
    {

        MagicSelect();

        //InputType = Magic[magic_num].GetComponent<Magic_Parameter>().inputtype;//選択してる弾のパラメタ読み込み
        InputType = SelectMagic[magic_num].GetComponent<Magic_Parameter>().inputtype;//選択してる弾のパラメタ読み込み

        if (Pz.GetF_Magic())//魔法が打てる状態かどうかを確認
        {
            //if (Magic[magic_num].GetComponent<Magic_Parameter>().spend_MP <= Pz.M_point)//使うMP < 現MPだったら魔法が打てる
            if (SelectMagic[magic_num].GetComponent<Magic_Parameter>().spend_MP <= Pz.M_point)//使うMP < 現MPだったら魔法が打てる
            {
                //if(this.existBullet[magic_num] < Magic[magic_num].GetComponent<Magic_Parameter>().GetExNum())//いまだしてる弾の数 < 出せる弾の数
                //if (this.Bullet0.Count < Magic[magic_num].GetComponent<Magic_Parameter>().GetExNum())//いまだしてる弾の数 < 出せる弾の数
                if (this.Bullet0.Count < SelectMagic[magic_num].GetComponent<Magic_Parameter>().GetExNum())//いまだしてる弾の数 < 出せる弾の数
                    MagicFire();
            }

        }

		coroutine = StartCoroutine (MPRecover ());

        if(magic_num != old_magic_num)
        {

            switch (magic_num)
            {
                case 0:
                    Bullet0 = oldBullet0;
                    break;
                case 1:
                    Bullet0 = oldBullet1;
                    break;
                case 2:
                    Bullet0 = oldBullet2;
                    break;
                case 3:
                    Bullet0 = oldBullet3;
                    break;
                case 4:
                    Bullet0 = oldBullet4;
                    break;
                default:
                    break;
            }

        }

        switch (magic_num)
        {
            case 0:
                oldBullet0 = Bullet0;
                break;
            case 1:
                oldBullet1 = Bullet0;
                break;
            case 2:
                oldBullet2 = Bullet0;
                break;
            case 3:
                oldBullet3 = Bullet0;
                break;
            case 4:
                oldBullet4 = Bullet0;
                break;
            default:
                break;
        }

        //弾がなくなったかどうかはこっちで判断
        for (int i = 0;i < Bullet0.Count;i++)
        {
            if (Bullet0[i] == null)
            {
                Bullet0.RemoveAt(i);

            }
        }
        for (int i = 0; i < oldBullet0.Count; i++)
        {
            if (oldBullet0[i] == null)
            {
                oldBullet0.RemoveAt(i);

            }
        }
        for (int i = 0; i < oldBullet1.Count; i++)
        {
            if (oldBullet1[i] == null)
            {
                oldBullet1.RemoveAt(i);

            }
        }
        for (int i = 0; i < oldBullet2.Count; i++)
        {
            if (oldBullet2[i] == null)
            {
                oldBullet2.RemoveAt(i);

            }
        }
        for (int i = 0; i < oldBullet3.Count; i++)
        {
            if (oldBullet3[i] == null)
            {
                oldBullet3.RemoveAt(i);

            }
        }
        for (int i = 0; i < oldBullet4.Count; i++)
        {
            if (oldBullet4[i] == null)
            {
                oldBullet4.RemoveAt(i);

            }
        }

        old_magic_num = magic_num;

    }

    void MagicSelect()
    {

        //マウスホイール式
        if (!isHold){
        }
        if (Input.GetAxis("Mouse ScrollWheel") == 1.0f)
        {

            if (magic_num == 0)
            {
                
                magic_num = selectmagic.Length - 1;

            }
            else
            {
                
                magic_num -= (int)Input.GetAxis("Mouse ScrollWheel");

            }

            //bullet_parameter = null;//念のためパラメタリセット

        }

        if (Input.GetAxis("Mouse ScrollWheel") == -1.0f)
        {
            if (magic_num == SelectMagic.Length - 1)
            {

                magic_num = 0;

            }
            else
            {
                
                magic_num -= (int)Input.GetAxis("Mouse ScrollWheel");

            }

            //bullet_parameter = null;//念のためパラメタリセット

        }

        //ボタン式
        if (!isHold)
        {
            /*
            if (Input.GetKeyDown(KeyCode.Z))
            {

                if (magic_num == 0)
                {

                    magic_num = SelectMagic.Length - 1;

                }
                else
                {

                    magic_num -= 1;

                }

            }

            if (Input.GetKeyDown(KeyCode.X))
            {

                if (magic_num == SelectMagic.Length - 1)
                {

                    magic_num = 0;

                }
                else
                {

                    magic_num += 1;

                }

            }*/

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                magic_num = 0;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                magic_num = 1;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                magic_num = 2;
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                magic_num = 3;
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                magic_num = 4;
            }


        }


    }

    void MagicFire()
    {

        switch (InputType)
        {
            case Magic_Parameter.InputType.GetDown:
                if (Input.GetButtonDown("Fire1"))
                {

                    //Magic[magic_num].SendMessage("Fire");
                    SelectMagic[magic_num].SendMessage("Fire");

                }
                break;
            case Magic_Parameter.InputType.Get:
                if (Input.GetButton("Fire1"))
                {

                    //Magic[magic_num].SendMessage("Hold");//ボタンおしっぱのとき
                    SelectMagic[magic_num].SendMessage("Hold");//ボタンおしっぱのとき
                    isHold = true;

                }
                if (Input.GetButtonUp("Fire1"))
                {

                    //Magic[magic_num].SendMessage("Fire");
                    SelectMagic[magic_num].SendMessage("Fire");
                    isHold = false;

                }
                break;
            default:
                if (Input.GetButtonDown("Fire1"))
                {

                    //Magic[magic_num].SendMessage("Fire");//魔法を制御しているScriptは1つ1つ名前が違うからしょうがない
                    SelectMagic[magic_num].SendMessage("Fire");//魔法を制御しているScriptは1つ1つ名前が違うからしょうがない

                }
                break;
        }
    }

	IEnumerator MPRecover(){//MPは自然回復

		if(isCoroutine){yield break;}
		isCoroutine = true;
		
		yield return new WaitForSeconds(1.0f);//回復スピード
		
        if(Pz.M_point < Pz.max_MP){Pz.M_point += 1;}//最大MPを超えないようにする
		
		isCoroutine = false;

	}
}
