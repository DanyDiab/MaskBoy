using UnityEngine;
using UnityEngine.UI;

public class SceneButton : MonoBehaviour
{
    [SerializeField] private string targetScene;
    
    // Optional: Auto-hook into the button on this object
    private void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(LoadTargetScene);
        }
    }

    // Public method to be called from OnClick (if manual assignment is preferred)
    public void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetScene))
        {
            GameSceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("Target scene name is empty on SceneButton!", this);
        }
    }

    public void QuitGame()
    {
        GameSceneManager.QuitGame();
    }

    public void SetTargetScene(string sceneName)
    {
        targetScene = sceneName;
    }
}
