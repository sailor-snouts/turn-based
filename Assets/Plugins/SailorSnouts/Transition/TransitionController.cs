using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionController : Transition
{
    private string loadingScene = "";
    private static TransitionController instance = null;

    void Awake()
    {
        if (TransitionController.instance && TransitionController.instance != this)
            DestroyImmediate(this.gameObject);
        else 
            TransitionController.instance = this;
    }
    
    new void Start()
    {
        base.Start();
        DontDestroyOnLoad(this.gameObject);
        this.anim.Play("Visible");
    }

    public static TransitionController GetInstance()
    {
        return TransitionController.instance;
    }

    public void Load(string scene)
    {
        this.loadingScene = scene;
        this.FadeIn();
        this.onFadedIn += this.OnFadedIn;
    }

    private void OnFadedIn()
    {
        SceneManager.LoadScene(this.loadingScene);
        this.onFadedIn -= this.OnFadedIn;
    }

    private void OnLoaded(Scene scene, LoadSceneMode mode)
    {
        this.Start();
        this.FadeOut();
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += this.OnLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= this.OnLoaded;
    }
}