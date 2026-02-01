using UnityEngine;
using UnityEngine.SceneManagement;

public class RunSession : MonoBehaviour
{
    public static RunSession Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void EnsureExists()
    {
        // Guarantees RunSession exists even if not placed in a scene.
        if (FindObjectOfType<RunSession>() != null) return;
        GameObject go = new GameObject("RunSession");
        go.AddComponent<RunSession>();
    }

    [Header("Run State")]
    [SerializeField] int currentWave = 0;
    [SerializeField] string initials = "";

    public int CurrentWave => currentWave;
    public string Initials => initials;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnemySpawner.OnWaveStart += OnWaveStart;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        EnemySpawner.OnWaveStart -= OnWaveStart;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset run state when starting a new run
        if (scene.name == "MainScene")
        {
            currentWave = 0;
        }
    }

    void OnWaveStart(int wave)
    {
        currentWave = wave;
    }

    public void SetInitials(string value)
    {
        initials = value ?? "";
    }
}

