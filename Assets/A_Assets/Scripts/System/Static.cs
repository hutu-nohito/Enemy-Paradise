using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class Static : MonoBehaviour {

    //使いまわすパラメタ//
    //資料：StreamWriter クラス (System.IO)
    //http://msdn.microsoft.com/ja-jp/library/system.io.streamwriter(v=vs.110).aspx


    //保存したいデータを常にここで更新しとく
    public float day = 1;//何日目か
    public float GetDay() { return day; }
    public void SetDay(float day) { this.day += day; }

	public int count_Start = 0;//何回起動したか
    public int GetCountStart() { return count_Start; }
    public void SetCountStart() { count_Start++; }

    //HPとCPはクエストが終わっても引き継ぎ
    public int H_Point = 10;
    public int GetHP() { return H_Point; }
    public void SetHP(int HP) { this.H_Point = HP; }
    public int M_Point = 10;
    public int GetMP() { return M_Point; }
    public void SetMP(int MP) { this.M_Point = MP; }

    public int lank_P = 0;//名声
    public int GetLP() { return lank_P; }
    public void SetLP(int LP) { this.lank_P = LP; }

    public int bonus_P = 0;//名声
    public int GetBP() { return bonus_P; }
    public void SetBP(int BP) { this.bonus_P = BP; }

    public int guild_level = 0;
    public int GetGL() { return guild_level; }
    public void SetGL(int GL) { this.guild_level = GL; }

    /*
     * ここに隙間の状態と持ってるデータの情報 
     */
    //public int[] MasekiNum;
    public int currentPallete = 1;//現在装備中のパレット
    public int[] SelectMagicID = new int[5];//パレットで選択されてる魔法

    private static bool flag_exist = false;
    void Awake()
    {

        if(!flag_exist){

            DontDestroyOnLoad(this.gameObject);
            flag_exist = true;

        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    //セーブ&ロード

    public string folder;    //これだけでunityの実行ファイルがあるフォルダがわかる
    private string[] SaveData = new string[5];//保存したい一個一個の要素
    //public TextAsset _Text;//保存用テキストファイル

    void Start()
    {
        //このObjectはずっと残るので、とりあえずここで初期化しとく。順番は固定。
        SaveData[0] = GetDay().ToString();
        SaveData[1] = GetHP().ToString();
        SaveData[2] = GetMP().ToString();
        SaveData[3] = GetLP().ToString();
        SaveData[4] = GetBP().ToString();

        folder = Application.dataPath;

    }

    void Update()
    {
        //debug
        /*if (Input.GetKeyDown(KeyCode.K))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }*/

    }

    public void Save()
    {
        //セーブを呼び出した段階でのパラメタを格納
        SaveData[0] = GetDay().ToString();
        SaveData[1] = GetHP().ToString();
        SaveData[2] = GetMP().ToString();
        SaveData[3] = GetLP().ToString();
        SaveData[4] = GetBP().ToString();

        SaveText(folder, @"\savedata.txt", SaveData);
    }

    public void Load()
    {
        SaveData = LoadText(folder, @"\savedata.txt");

        SetDay(float.Parse(SaveData[0]));
        SetHP(int.Parse(SaveData[1]));
        SetMP(int.Parse(SaveData[2]));
        SetLP(int.Parse(SaveData[3]));
        SetBP(int.Parse(SaveData[4]));
    }

    //テキストファイルとしてセーブ,上書き
    //ファイルを作るパス、作るファイルの名前、書きこむ文字列
    public void SaveText(string fileFolder, string filename, string[] dataStr)
    {
        using (StreamWriter w = new StreamWriter(fileFolder + filename))//たぶんパスを作ってそこに書き込んでる、あったら作らずに書き込む
        {
            foreach (var item in dataStr)//foreachはitemにいったんdataStrを格納してから処理をしてる気がする
            {
                w.WriteLine(item);
            }
        }
    }

    //ローダー
    public string[] LoadText(string fileFolder, string filename)
    {
        List<string> strList = new List<string>();
        string line = "";
        using (StreamReader sr = new StreamReader(fileFolder + filename))
        {
            while ((line = sr.ReadLine()) != null)
            {
                strList.Add(line);
            }
        }
        return strList.ToArray();
    }
    
}
