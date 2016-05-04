using UnityEngine;
using System.Collections;
using System.Collections.Generic;//List用


public class Block_Controller : MonoBehaviour {

    /*
     Wave 自分の周りが持ち上がる。
     
     * RouteSearch();//0:前,1:後,2:右,3:左
     * 並び替えは技出すときだけにする(重いから)
     * RouteSearchは並び替えが必要ない

        Dammyがないとブロックがないとき挙動がおかしくなる
     
     */

    GameObject DirectionPosZ = null;//前
    GameObject DirectionNegZ = null;//後ろ
    GameObject DirectionPosX = null;//右
    GameObject DirectionNegX = null;//左

    public GameObject Character;//ボス
    public GameObject Dammy_Block;//近くにブロックがない場合のダミー(めっちゃ遠くにしとく)

    private List<GameObject> Blocks = new List<GameObject>();//近くのブロックをすべて格納
    private List<GameObject> Blocks_Length1 = new List<GameObject>();//距離が1のブロックを格納
    private List<GameObject> Blocks_Length2 = new List<GameObject>();//距離が2のブロックを格納
    private List<GameObject> Blocks_Length3 = new List<GameObject>();//距離が3のブロックを格納
    

    public GameObject nearBlock = null;//一番近いブロック
    public float ReleaseLength = 9;//ブロックを手放す距離

    public Vector3 LerpMoveDirection;//とりあえず動かす用。あとで何とかするべき

    public bool flag_Route = false;//ルート検索中。使ったら戻す。

    //private int count = 0;//汎用　使ったら戻す

    // Use this for initialization
    void Start()
    {
        //設定されてなかったら入れとく
        if(Character == null)
        Character = GameObject.FindGameObjectWithTag("Player");//とりあえずPlayer

    }

    // Update is called once per frame
    void Update()
    {

        if (Blocks.Count > 0)
        {

            MeasureTarget();//ブロックとの距離を計算
            //Visible();//隠れてるブロックは動かせない

        }//ターゲットがいなかったら回さない 
       
        if (Input.GetKeyDown(KeyCode.B)) 
        {
            StartCoroutine(Wave());
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(Wall());
        }

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Gimmick")
        {

            for (int i = 0; i < Blocks.Count; i++)
            {
                if (col.gameObject == Blocks[i])//すでに持ってるブロックはリストに加えない
                {

                    return;//これでよさそう

                }
            }

            Blocks.Add(col.gameObject);

        }

    }

    void ReleaseTarget(int num)//距離が一定以上離れた敵を配列から外す
    {

        Blocks.RemoveAt(num);//遠いターゲットをリリース

    }

    void MeasureTarget()//ブロックとの距離を計測。遠かったら捨てる。距離に応じた配列に格納
    {
        //nearBlock = null;//関数の中でnullになるとなぜかダメになる
        float length = 0;//ブロックとの距離
        float near = 100;//一番近い距離

        for (int i = 0; i < Blocks.Count; i++)
        {
            length = (Character.transform.position - Blocks[i].transform.position).magnitude;

			//なぜか距離が近くてもリリースするので消しとく　重くなったら考える
            //if (length > ReleaseLength) { Blocks.Remove(Blocks[i]); }//Visionからの距離のほうがいい

            if (length < near)
            {//近かったら

                near = length;
                if (Blocks.Count > 0) nearBlock = Blocks[i];//条件入れとかないと0でも処理が入る

            }            
        }

    }

    //ブロックを距離で並び替えてるだけ
    void BlockMarge(GameObject nearBlock)
    {
        //float length = 0;//足元のブロックとの距離

        //一回一回消した方が確実
        Blocks_Length1.Clear();
        Blocks_Length2.Clear();
        Blocks_Length3.Clear();

        //1ます目
        for (int i = 0; i < 4; i++)
        {
            Blocks_Length1.Add(RouteSearch(nearBlock, i));
        }

        //2マス目
		Blocks_Length2.Add(RouteSearch(Blocks_Length1[0], 0));
		Blocks_Length2.Add(RouteSearch(Blocks_Length1[0], 2));
		Blocks_Length2.Add(RouteSearch(Blocks_Length1[2], 2));
        Blocks_Length2.Add(RouteSearch(Blocks_Length1[2], 1));
        Blocks_Length2.Add(RouteSearch(Blocks_Length1[1], 1));
        Blocks_Length2.Add(RouteSearch(Blocks_Length1[1], 3));
        Blocks_Length2.Add(RouteSearch(Blocks_Length1[3], 3));
        Blocks_Length2.Add(RouteSearch(Blocks_Length1[3], 0));

        //3マス目
        Blocks_Length3.Add(RouteSearch(Blocks_Length2[0], 0));
        Blocks_Length3.Add(RouteSearch(Blocks_Length2[0], 2));
        Blocks_Length3.Add(RouteSearch(Blocks_Length2[2], 0));
        Blocks_Length3.Add(RouteSearch(Blocks_Length2[2], 2));
        Blocks_Length3.Add(RouteSearch(Blocks_Length2[2], 1));
        Blocks_Length3.Add(RouteSearch(Blocks_Length2[4], 2));
        Blocks_Length3.Add(RouteSearch(Blocks_Length2[4], 1));
        Blocks_Length3.Add(RouteSearch(Blocks_Length2[4], 3));
        Blocks_Length3.Add(RouteSearch(Blocks_Length2[6], 1));
        Blocks_Length3.Add(RouteSearch(Blocks_Length2[6], 3));
        Blocks_Length3.Add(RouteSearch(Blocks_Length2[6], 0));
        Blocks_Length3.Add(RouteSearch(Blocks_Length2[0], 3));

    }

    //経路検索用。一回一回止める。欲しい方向のものを返してもらう
    GameObject RouteSearch(GameObject nowPos,int DirectionNum)
    {
        float length = 0;//計測用
        Vector3 Direction;//方向用
        GameObject[] Enpty = new GameObject[10];//入れ替え用
        List<GameObject> Blocks_1 = new List<GameObject>();//1ます以内のブロックをすべて格納
        
        //1ます以内検索
        for (int i = 0; i < Blocks.Count; i++)
        {
            length = (nowPos.transform.position - Blocks[i].transform.position).magnitude;

            if (length > 2.7f && length < 3.3f)
            {//1ます以内だったら

                Blocks_1.Add(Blocks[i]);

            }            
        }

        //周囲のマスが4つ無い(端っこ)のときはDammyを入れて計算してもらう
        if (Blocks_1.Count < 4)
        {
            flag_Route = false;//使ったら戻す
            Blocks_1.Clear();
            return Dammy_Block;
        }

        //前後左右を格納
        for(int i = 0;i < Blocks_1.Count;i++)
        {
            Direction = Blocks_1[i].transform.position - nowPos.transform.position;

            if (Direction.x < 0.3f && Direction.x > -0.3f)
            {
                if (Direction.z > 0) DirectionPosZ = Blocks_1[i];
                if (Direction.z < 0) DirectionNegZ = Blocks_1[i];
            }
            if (Direction.z < 0.3f && Direction.z > -0.3f)
            {
                if (Direction.x > 0) DirectionPosX = Blocks_1[i];
                if (Direction.x < 0) DirectionNegX = Blocks_1[i];
            }
        }

        //キャラの方向によって前後左右を入れ替え
        if (Character.transform.TransformDirection(Vector3.forward).z < -0.7f)//後ろ
        {

            Enpty[0] = DirectionPosZ;
            DirectionPosZ = DirectionNegZ;
            DirectionNegZ = Enpty[0];
            Enpty[0] = DirectionPosX;
            DirectionPosX = DirectionNegX;
            DirectionNegX = Enpty[0];

        }
        else if(Character.transform.TransformDirection(Vector3.forward).z > 0.7f)//前
        {

        }
        else if(Character.transform.TransformDirection(Vector3.forward).x > 0.7f)//右
        {
            Enpty[0] = DirectionPosZ;
            Enpty[1] = DirectionNegZ;
            Enpty[2] = DirectionPosX;
            Enpty[3] = DirectionNegX;
            DirectionPosZ = Enpty[2];
            DirectionNegZ = Enpty[3];
            DirectionPosX = Enpty[1];
            DirectionNegX = Enpty[0];
            
        }
        else if (Character.transform.TransformDirection(Vector3.forward).x < -0.7f)//左
        {
            Enpty[0] = DirectionPosZ;
            Enpty[1] = DirectionNegZ;
            Enpty[2] = DirectionPosX;
            Enpty[3] = DirectionNegX;
            DirectionPosZ = Enpty[3];
            DirectionNegZ = Enpty[2];
            DirectionPosX = Enpty[0];
            DirectionNegX = Enpty[1];
            
        }

        /*Debug.Log(DirectionPosZ);
        Debug.Log(DirectionNegZ);
        Debug.Log(DirectionPosX);
        Debug.Log(DirectionNegX);*/

        flag_Route = false;//使ったら戻す
        Blocks_1.Clear();
        Enpty = null;

		//番号で上下左右のどこを返すか決める
        if (DirectionNum == 1) return DirectionNegZ;
        else if (DirectionNum == 2) return DirectionPosX;
        else if (DirectionNum == 3) return DirectionNegX;
        return DirectionPosZ;
    }

    void Visible()//TargetとPlayerの間に何もないことを確認。なんかあったら近くの敵から外す
    {
        RaycastHit hit;

        GameObject hitObject;
        Vector3 Line = new Vector3(Character.transform.position.x, Character.transform.position.y + 1.5f, Character.transform.position.z);

        //if (nearTarget != null)
        //{
        for(int i = 0;i < Blocks.Count; i++)
        {
            if (Physics.Raycast(Line, Blocks[i].transform.position - Character.transform.position, out hit, ReleaseLength))
            {
                hitObject = hit.collider.gameObject;

                if (hitObject != Blocks[i])
                {
                    ReleaseTarget(i);
                }
                
                Debug.DrawLine(Line, hit.point, Color.red);
                
            }
        }
    }

    public void StartWave()
    {
        StartCoroutine(Wave());
    }

    //自分の周りが持ち上がる。たぶん大丈夫。
    IEnumerator Wave()
    {
        BlockMarge(nearBlock);

        yield return new WaitForSeconds(2.0f);

        List<GameObject> MoveBlocks1 = new List<GameObject>();//動かしたブロックをすべて格納
        List<GameObject> MoveBlocks2 = new List<GameObject>();//動かしたブロックをすべて格納
        List<GameObject> MoveBlocks3 = new List<GameObject>();//動かしたブロックをすべて格納

        for (int i = 0; i < Blocks_Length2.Count; i++)
        {

            MoveBlocks2.Add(Blocks_Length2[i]);//距離が変わってしまう可能性が高いので、動かしたのを格納しとく

        }

        for (int i = 0; i < Blocks_Length3.Count; i++)
        {

            MoveBlocks3.Add(Blocks_Length3[i]);//距離が変わってしまう可能性が高いので、動かしたのを格納しとく

        }

        for(int i = 0;i < Blocks_Length1.Count;i++)
        {
            MoveBlocks1.Add(Blocks_Length1[i]);//距離が変わってしまう可能性が高いので、動かしたのを格納しとく
            MoveBlocks1[i].GetComponent<Block_Move>().LerpMove(LerpMoveDirection, 1.0f);

        }

        ///
        yield return new WaitForSeconds(2.0f);
        ///

        for (int i = 0; i < Blocks_Length1.Count; i++)
        {
            
            MoveBlocks1[i].GetComponent<Block_Move>().Stop();

        }

        for (int i = 0; i < MoveBlocks2.Count; i++)
        {
            MoveBlocks2[i].GetComponent<Block_Move>().LerpMove(LerpMoveDirection, 1.0f);

        }

        ///
        yield return new WaitForSeconds(2.0f);
        ///

        for (int i = 0; i < MoveBlocks1.Count; i++)
        {
            //MoveBlocks1[i].transform.position = new Vector3(MoveBlocks1[i].transform.position.x, 0, MoveBlocks1[i].transform.position.z);
            MoveBlocks1[i].GetComponent<Block_Move>().LerpMove(-LerpMoveDirection, 1.0f);

        }

        for (int i = 0; i < Blocks_Length2.Count; i++)
        {

            MoveBlocks2[i].GetComponent<Block_Move>().Stop();

        }

        for (int i = 0; i < MoveBlocks3.Count; i++)
        {

            MoveBlocks3[i].GetComponent<Block_Move>().LerpMove(LerpMoveDirection, 1.0f);

        }

        yield return new WaitForSeconds(2.0f);//動かす前にClearされそうだから入れとく

        for (int i = 0; i < MoveBlocks2.Count; i++)
        {
            //MoveBlocks2[i].transform.position = new Vector3(MoveBlocks2[i].transform.position.x, 0, MoveBlocks2[i].transform.position.z);
            MoveBlocks2[i].GetComponent<Block_Move>().LerpMove(-LerpMoveDirection, 1.0f);

        }

        for (int i = 0; i < Blocks_Length3.Count; i++)
        {

            MoveBlocks3[i].GetComponent<Block_Move>().Stop();

        }

        yield return new WaitForSeconds(2.0f);

        for (int i = 0; i < MoveBlocks3.Count; i++)
        {
            //MoveBlocks3[i].transform.position = new Vector3(MoveBlocks3[i].transform.position.x, 0, MoveBlocks3[i].transform.position.z);
            MoveBlocks3[i].GetComponent<Block_Move>().LerpMove(-LerpMoveDirection, 1.0f);

        }

        yield return new WaitForSeconds(1.0f);

        MoveBlocks1.Clear();
        MoveBlocks2.Clear();
        MoveBlocks3.Clear();
        
    }

    public void StartWall()
    {
        StartCoroutine(Wall());
    }

    //プレイヤーの両側に壁を出す。戻すときの挙動をどうにかする。(動いた距離と合わせればいいと思う)
	//配列クリア前にブロックが追加されるとおかしな挙動をするので使用する際は十分間隔をあけること　できれば直す
    IEnumerator Wall()
    {

        GameObject nowPos;//今の場所
        List<GameObject> nowPos_L = new List<GameObject>();//左列
        List<GameObject> nowPos_R = new List<GameObject>();//右列
        flag_Route = true;
        
        //1個目はnearBlocks
        nowPos = RouteSearch(nearBlock, 0);//1つ前
        nowPos_R.Add(RouteSearch(nowPos, 2));//1つ前1つ右
        nowPos_L.Add(RouteSearch(nowPos, 3));//1つ前1つ左

        //2個目以降
        ///4のとこに何個ブロックを持ち上げるかを入れる。
        for (int i = 0; i < 3;i++ )
        {
            if (nowPos_R[i] != null)
            {
                nowPos_R.Add(RouteSearch(nowPos_R[i], 0));//2つ前1つ右
                nowPos_L.Add(RouteSearch(nowPos_L[i], 0));//2つ前1つ左

            }
            else
            {
                nowPos_R.Add(null);//2つ前1つ右
                nowPos_L.Add(null);//2つ前1つ左
            }
            
        }        

        for (int i = 0; i < nowPos_R.Count; i++)
        {
            if (nowPos_R[i] != null)
            {
                nowPos_R[i].GetComponent<Block_Move>().LerpMove(LerpMoveDirection, 1.0f);
            }
            if (nowPos_L[i] != null)
            {
                nowPos_L[i].GetComponent<Block_Move>().LerpMove(LerpMoveDirection, 1.0f);
            }
        }
            
        yield return new WaitForSeconds(2.0f);

        for (int i = 0; i < nowPos_R.Count; i++)
        {
            if (nowPos_R[i] != null)
            {
                nowPos_R[i].GetComponent<Block_Move>().Stop();
            }
            if (nowPos_L[i] != null)
            {
                nowPos_L[i].GetComponent<Block_Move>().Stop();
            }
        }

        yield return new WaitForSeconds(3.0f);

        for (int i = 0; i < nowPos_R.Count; i++)
        {
            if (nowPos_R[i] != null)
            {
                //nowPos_R[i].transform.position = new Vector3(nowPos_R[i].transform.position.x,0,nowPos_R[i].transform.position.z);
                nowPos_R[i].GetComponent<Block_Move>().LerpMove(-LerpMoveDirection, 1.0f);
            }
            if (nowPos_L[i] != null)
            {
                //nowPos_L[i].transform.position = new Vector3(nowPos_L[i].transform.position.x, 0, nowPos_L[i].transform.position.z);
                nowPos_L[i].GetComponent<Block_Move>().LerpMove(-LerpMoveDirection, 1.0f);
            }
            
        }

        yield return new WaitForSeconds(2.0f);

        for (int i = 0; i < nowPos_R.Count; i++)
        {
            if (nowPos_R[i] != null)
            {
                nowPos_R[i].GetComponent<Block_Move>().Stop();
            }
            if (nowPos_L[i] != null)
            {
                nowPos_L[i].GetComponent<Block_Move>().Stop();
            }
        }

        nowPos_R.Clear();
        nowPos_L.Clear();
        nowPos = null;

    }
}
