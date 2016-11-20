using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextEffect : MonoBehaviour {

    private Text text;
    public float fade_speed = 0.5f;
    public bool flag_Fade = false;
    private float alpha = 255;

	// Use this for initialization
	void Start () {

        text = GetComponent<Text>();
        alpha = text.color.a;
	}
	
	// Update is called once per frame
	void Update () {

        if (flag_Fade)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime * fade_speed);
        }
        else
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + Time.deltaTime * fade_speed);
        }

    }

    public void ReverseFade()
    {
        flag_Fade = !flag_Fade;
    }
    public void Reset()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }
}
