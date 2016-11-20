using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TextMove : MonoBehaviour {

    private Text text;
    public int speed = 2;
    public int count = 1;
    public bool flag_True = true;
    private int font_size = 70;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        font_size = text.fontSize;
    }

    // Update is called once per frame
    void Update()
    {

        if (flag_True)
        {
            if(count % speed == 0)
            {
                text.fontSize += 1;
                count = 0;
            }
        }
        else
        {
            if (count % speed == 0)
            {
                text.fontSize -= 1;
                count = 0;
            }
        }

        count++;

    }

    public void ReverseFade()
    {
        flag_True = !flag_True;
    }

    public void Reset()
    {
        text.fontSize = font_size;
    }
}
