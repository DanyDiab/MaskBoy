using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    private static GameSceneManager _instance;
    private CanvasGroup _transitionCanvasGroup;
    private Canvas _transitionCanvas;
    private float _transitionDuration = 0.5f;
    private bool _isTransitioning = false;

    public static GameSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find existing instance
                _instance = FindObjectOfType<GameSceneManager>();

                // If none exists, create a new GameObject with the manager
                if (_instance == null)
                {
                    GameObject managerGO = new GameObject("GameSceneManager");
                    _instance = managerGO.AddComponent<GameSceneManager>();
                    DontDestroyOnLoad(managerGO);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeTransitionUI();
    }

    private void InitializeTransitionUI()
    {
        if (_transitionCanvas != null) return;

        // Create Canvas for transition overlay
        GameObject canvasGO = new GameObject("TransitionCanvas");
        canvasGO.transform.SetParent(transform);
        
        _transitionCanvas = canvasGO.AddComponent<Canvas>();
        _transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _transitionCanvas.sortingOrder = 9999; // Ensure it's on top of everything

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create Image for fading
        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform);
        
        Image image = imageGO.AddComponent<Image>();
        image.color = Color.black;
        
        // Stretch image to fill screen
        RectTransform rt = image.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        _transitionCanvasGroup = imageGO.AddComponent<CanvasGroup>();
        _transitionCanvasGroup.alpha = 0f; // Start transparent
        _transitionCanvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Static method to load a scene with a transition.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load.</param>
    public static void LoadScene(string sceneName)
    {
        if (Instance._isTransitioning) return;
        Instance.StartCoroutine(Instance.TransitionToScene(sceneName));
    }

    /// <summary>
    /// Static method to reload the current scene with a transition.
    /// </summary>
    public static void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Static method to quit the application.
    /// </summary>
    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        _isTransitioning = true;
        _transitionCanvasGroup.blocksRaycasts = true; // Block input during transition

        // Fade Out (Screen becomes black)
        float timer = 0f;
        while (timer < _transitionDuration)
        {
            timer += Time.deltaTime;
            _transitionCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / _transitionDuration);
            yield return null;
        }
        _transitionCanvasGroup.alpha = 1f;

        // Load Scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        if (asyncLoad == null)
        {
            Debug.LogError($"Scene '{sceneName}' could not be loaded. Make sure it is added to the Build Settings.");
            
            // Fade back in so the game isn't stuck on black
            timer = 0f;
            while (timer < _transitionDuration)
            {
                timer += Time.deltaTime;
                _transitionCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / _transitionDuration);
                yield return null;
            }
            _transitionCanvasGroup.alpha = 0f;
            _transitionCanvasGroup.blocksRaycasts = false;
            _isTransitioning = false;
            yield break;
        }

        asyncLoad.allowSceneActivation = false;

        // Wait until scene is fully loaded (at 90%)
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Activate Scene
        asyncLoad.allowSceneActivation = true;
        
        // Wait for the scene to actually switch
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade In (Screen reveals new scene)
        timer = 0f;
        while (timer < _transitionDuration)
        {
            timer += Time.deltaTime;
            _transitionCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / _transitionDuration);
            yield return null;
        }
        _transitionCanvasGroup.alpha = 0f;
        
        _transitionCanvasGroup.blocksRaycasts = false;
        _isTransitioning = false;
    }
}
