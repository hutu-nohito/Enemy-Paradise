using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Text_C : MonoBehaviour 
{
	public string[] scenarios;
	[SerializeField] Text uiText;
	
	[SerializeField][Range(0.001f, 0.3f)]
	float intervalForCharacterDisplay = 0.05f;
	
	private string currentText = string.Empty;//現在の文字
	private float timeUntilDisplay = 0;
	private float timeElapsed = 1;
	private int currentLine = 0;
	private int lastUpdateCharacter = -1;

	//自作
	public TextAsset _Text;//ここにテキスト
	public string[] text;

	// 文字の表示が完了しているかどうか
	public bool IsCompleteDisplayText 
	{
		get { return  Time.time > timeElapsed + timeUntilDisplay; }
	}
	
	void Start()
	{

		this.readMap();//文字読み込み
		SetNextLine();//

	}

	//文字読み込み
	void readMap()
	{
		char[] kugiri = {'#'};//テキストを区切る記号設定
		//テキストを一行づつ区切る
		text = _Text.text.Split(kugiri);
		for(int i = 0;i < text.Length ;i++){

			scenarios[i] = text[i];

		}
	}
	
	//
	
	void Update () 
	{
		// 文字の表示が完了してるならクリック時に次の行を表示する
		if( IsCompleteDisplayText ){
			if(currentLine < scenarios.Length && Input.GetMouseButtonDown(0)){
				SetNextLine();
			}
		}else{
			// 完了してないなら文字をすべて表示する
			if(Input.GetMouseButtonDown(0)){
				timeUntilDisplay = 0;
			}
		}
		
		int displayCharacterCount = (int)(Mathf.Clamp01((Time.time - timeElapsed) / timeUntilDisplay) * currentText.Length);
		if( displayCharacterCount != lastUpdateCharacter ){
			uiText.text = currentText.Substring(0, displayCharacterCount);
			lastUpdateCharacter = displayCharacterCount;
		}

		if(currentLine >= text.Length){

			currentLine = 1;

		}
	}
	
	
	void SetNextLine()
	{
		currentText = scenarios[currentLine];
		timeUntilDisplay = currentText.Length * intervalForCharacterDisplay;
		timeElapsed = Time.time;
		currentLine++;
		lastUpdateCharacter = -1;
	}

}