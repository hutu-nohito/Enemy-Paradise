using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {

    //Scene系は全部ここで管理

    private Static _static;

    private string SceneName = "Home";
    private Coroutine coroutine;
    private bool isCoroutine;
    private Event_Manager EM;

    // Use this for initialization
    void Start()
    {

        _static = GetComponent<Static>();
        EM = GetComponent<Event_Manager>();

    }

    // Update is called once per frame
    void Update()
    {

        if (isFade)
        {
            //フェードアウト
            if (fadeOut)
            {
                elapsedTime += Time.deltaTime;
                fadeAlpha += Time.deltaTime;
                fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, fadeAlpha);
                if (elapsedTime > fadeTime)
                {
                    fadeAlpha = 1;
                    fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 1);
                    isFade = false;
                    elapsedTime = 0;
                    fadeOut = false;
                    Invoke("Fade", 2);//フェードアウトしたらたいていはフェードインする
                }
            }
            //フェードイン
            if (!fadeOut)
            {
                elapsedTime += Time.deltaTime;
                fadeAlpha -= Time.deltaTime;
                fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, fadeAlpha);
                if (elapsedTime > fadeTime)
                {
                    fadeAlpha = 0;
                    fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 0);
                    isFade = false;
                    elapsedTime = 0;
                    fadeOut = true;
                    fade.enabled = false;
                }
            }

        }
    }

    public void GameStart()
    {

        SceneName = "Home";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("Home");
        _static.count_Start++;

    }

    public void Guild()
    {

        SceneName = "guild";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("guild");

    }

    public void Home()
    {

        SceneName = "Home";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("Home");

    }

    public void Sukima()
    {

        SceneName = "Sukima";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("Home");

    }

    public void BattleField()//コロシアム
    {
        SceneName = "lilith2 battle field";
        coroutine = StartCoroutine(TransScene());

    }

    public void Magic_Lab()//デバック用ステージ
    {
        SceneName = "Magic_Lab";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("Magic_Lab");

    }

    //アクションステージ//////////////////////////////////

    public void Gaidou()
    {
        SceneName = "Gaidou";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("Magic_Lab");

    }

    public void Forest()
    {
        SceneName = "Forest";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("Magic_Lab");

    }

    public void Pond()
    {
        SceneName = "Pond";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("Magic_Lab");

    }

    public void Kougen()
    {
        SceneName = "Kougen";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("Magic_Lab");

    }

    public void Green()
    {
        SceneName = "Green1";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("Green1");

    }

    public void Mine()
    {
        SceneName = "Mine";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("Mine");

    }

    public void Town()
    {
        SceneName = "Town";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("Town");

    }

    public void Swamp()
    {
        SceneName = "Swamp";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("Swamp");

    }

    public void Ruins()
    {
        SceneName = "Ruins";
        coroutine = StartCoroutine(TransScene());
        //Application.LoadLevel("Ruins");

    }

    private AsyncOperation Asy = null;
    private IEnumerator TransScene()
    {
        if (isCoroutine) yield break;
        isCoroutine = true;
        Fade();

        //Asy = Application.LoadLevelAsync(SceneName);
        Asy = SceneManager.LoadSceneAsync(SceneName);

        Asy.allowSceneActivation = false;

        yield return new WaitForSeconds(fadeTime);

        Asy.allowSceneActivation = true;

        yield return new WaitForSeconds(0.5f);//シーンが完全に変わりきってからイベントチェック

        EM.SendMessage("Check_Event");

        isCoroutine = false;
    }

    //フェード用//////////////////////
    public Image fade;
    private float fadeAlpha = 0;
    public bool isFade = false;
    private bool fadeOut = true;
    public float fadeTime = 2.0f;
    private float elapsedTime = 0.0f;
    public void Fade()
    {
        fade.enabled = true;
        isFade = true;
    }

}
