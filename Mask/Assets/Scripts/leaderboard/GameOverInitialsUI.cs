using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverInitialsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_InputField initialsInput;
    [SerializeField] Button submitButton;
    [SerializeField] TextMeshProUGUI helperText;

    [Header("Confirm Modal (optional)")]
    [SerializeField] bool requireConfirmPrompt = true;
    [SerializeField] GameObject confirmPanel; // contains Yes/No buttons + text
    [SerializeField] Button confirmYesButton;
    [SerializeField] Button confirmNoButton;
    [SerializeField] Image dimBackgroundImage; // full-screen Image, alpha ~0.5, raycastTarget=true

    [Header("Screen Roots (optional)")]
    [Tooltip("Your normal GameOver buttons/menu root (Top Scores / One More / Quit).")]
    [SerializeField] GameObject regularScreenRoot;
    [Tooltip("The initials input + submit UI root.")]
    [SerializeField] GameObject leaderboardEntryRoot;

    [Header("Settings")]
    [SerializeField] int leaderboardSize = 150;

    void Awake()
    {
        // Force a clean initial state (prevents “both panels visible” when scene objects
        // are left enabled in the hierarchy).
        SetActiveSafe(confirmPanel, false);
        SetActiveSafeDim(false);
        SetLeaderboardEntryVisible(false);

        if (initialsInput != null)
        {
            initialsInput.characterLimit = 3;
            initialsInput.onValueChanged.AddListener(OnInputChanged);
        }

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(Submit);
        }

        if (confirmYesButton != null) confirmYesButton.onClick.AddListener(OnConfirmYes);
        if (confirmNoButton != null) confirmNoButton.onClick.AddListener(OnConfirmNo);

        // Initial state
        if (requireConfirmPrompt)
        {
            ShowConfirmPrompt();
        }
        else
        {
            ShowLeaderboardEntry();
        }

        UpdateSubmitState();
    }

    void OnDestroy()
    {
        if (initialsInput != null) initialsInput.onValueChanged.RemoveListener(OnInputChanged);
        if (submitButton != null) submitButton.onClick.RemoveListener(Submit);
        if (confirmYesButton != null) confirmYesButton.onClick.RemoveListener(OnConfirmYes);
        if (confirmNoButton != null) confirmNoButton.onClick.RemoveListener(OnConfirmNo);
    }

    void OnInputChanged(string value)
    {
        if (initialsInput == null) return;

        string cleaned = Clean(value);
        if (cleaned != value)
        {
            initialsInput.SetTextWithoutNotify(cleaned);
        }

        UpdateSubmitState();
    }

    void UpdateSubmitState()
    {
        string text = initialsInput != null ? initialsInput.text : "";
        bool ok = !string.IsNullOrEmpty(text) && text.Length == 3;
        if (submitButton != null) submitButton.interactable = ok;
    }

    static string Clean(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        s = s.ToUpperInvariant();

        // Keep only A-Z and 0-9, max 3 chars
        System.Text.StringBuilder sb = new System.Text.StringBuilder(3);
        for (int i = 0; i < s.Length && sb.Length < 3; i++)
        {
            char c = s[i];
            if ((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    void ShowConfirmPrompt()
    {
        // Regular screen visible behind prompt (user asked: if No -> regular screen)
        if (regularScreenRoot != null) regularScreenRoot.SetActive(true);

        SetLeaderboardEntryVisible(false);
        SetActiveSafeDim(false);
        SetActiveSafe(confirmPanel, true);
    }

    void ShowLeaderboardEntry()
    {
        // Dim/tint background while entering initials
        SetActiveSafeDim(true);
        SetActiveSafe(confirmPanel, false);
        SetLeaderboardEntryVisible(true);

        if (helperText != null) helperText.text = "";

        if (initialsInput != null)
        {
            initialsInput.SetTextWithoutNotify("");
            initialsInput.ActivateInputField();
        }

        UpdateSubmitState();
    }

    void OnConfirmYes()
    {
        ShowLeaderboardEntry();
    }

    void OnConfirmNo()
    {
        // Return to the regular game over screen (no tint, no input)
        SetActiveSafeDim(false);
        SetActiveSafe(confirmPanel, false);

        if (regularScreenRoot != null) regularScreenRoot.SetActive(true);

        SetLeaderboardEntryVisible(false);
    }

    void SetLeaderboardEntryVisible(bool visible)
    {
        // Prefer root, but ALSO force the specific widgets (covers mis-parented objects).
        if (leaderboardEntryRoot != null) leaderboardEntryRoot.SetActive(visible);
        if (initialsInput != null) initialsInput.gameObject.SetActive(visible);
        if (submitButton != null) submitButton.gameObject.SetActive(visible);
    }

    void SetActiveSafeDim(bool visible)
    {
        if (dimBackgroundImage != null) dimBackgroundImage.gameObject.SetActive(visible);
    }

    static void SetActiveSafe(GameObject go, bool active)
    {
        if (go != null) go.SetActive(active);
    }

    public void Submit()
    {
        if (initialsInput == null) return;

        string initials = Clean(initialsInput.text);
        if (initials.Length != 3)
        {
            if (helperText != null) helperText.text = "Enter 3 characters.";
            return;
        }

        int wave = 0;
        if (RunSession.Instance != null) wave = RunSession.Instance.CurrentWave;
        if (RunSession.Instance != null) RunSession.Instance.SetInitials(initials);

        LeaderboardStore.AddScore(initials, wave, leaderboardSize);
        GameSceneManager.LoadScene("Leaderboard");
    }
}

