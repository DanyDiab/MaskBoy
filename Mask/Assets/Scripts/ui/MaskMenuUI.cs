using UnityEngine;
using UnityEngine.UI;

public class MaskMenuUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject panel;
    [SerializeField] Image screenOverlayImage;
    [SerializeField] Button[] maskButtons;
    [SerializeField] MaskConfig[] maskConfigs;

    [Header("Animation")]
    [SerializeField] float openCloseDuration = 0.18f;
    [SerializeField] Vector2 hiddenOffset = new Vector2(0f, -250f); // relative to shown position

    [Header("Overlay")]
    [Range(0.05f, 1f)]
    [SerializeField] float overlayScreenHeightPercent = 0.25f; // bottom 25%

    [Header("Player")]
    [SerializeField] MaskManager maskManager;

    bool isOpen = false;
    float previousTimeScale = 1f;
    RectTransform panelRect;
    Vector2 panelShownPos;
    CanvasGroup panelCanvasGroup;
    Coroutine animRoutine;

    void Awake()
    {
        if (maskManager == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) maskManager = player.GetComponent<MaskManager>();
        }

        if (maskManager != null && screenOverlayImage != null)
        {
            maskManager.SetOverlay(screenOverlayImage);
        }

        ConfigureOverlayRect();

        panelRect = panel != null ? panel.GetComponent<RectTransform>() : null;
        if (panelRect != null)
        {
            panelShownPos = panelRect.anchoredPosition;
            panelCanvasGroup = panel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null) panelCanvasGroup = panel.AddComponent<CanvasGroup>();
        }

        WireButtons();
        SetOpen(false, immediate: true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetOpen(!isOpen, immediate: false);
        }
    }

    void WireButtons()
    {
        if (maskButtons == null || maskConfigs == null) return;
        int count = Mathf.Min(maskButtons.Length, maskConfigs.Length);

        for (int i = 0; i < count; i++)
        {
            Button btn = maskButtons[i];
            MaskConfig cfg = maskConfigs[i];
            if (btn == null) continue;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                if (maskManager != null) maskManager.Equip(cfg);
                SetOpen(false, immediate: false);
            });

            // Apply icon + tint to the button Image (optional)
            Image img = btn.GetComponent<Image>();
            if (img != null && cfg != null)
            {
                if (cfg.icon != null) img.sprite = cfg.icon;
                img.color = cfg.iconTint;
            }
        }
    }

    void SetOpen(bool open, bool immediate)
    {
        isOpen = open;

        if (open)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            if (panel != null) panel.SetActive(true);
            if (immediate)
            {
                ApplyPanelVisual(open: true);
            }
            else
            {
                StartPanelAnim(open: true);
            }
        }
        else
        {
            if (immediate)
            {
                ApplyPanelVisual(open: false);
                if (panel != null) panel.SetActive(false);
                RestoreTimeAndCursor();
            }
            else
            {
                StartPanelAnim(open: false);
            }
        }
    }

    void RestoreTimeAndCursor()
    {
        Time.timeScale = previousTimeScale <= 0f ? 1f : previousTimeScale;
    }

    void StartPanelAnim(bool open)
    {
        if (panel == null || panelRect == null) return;

        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(AnimatePanel(open));
    }

    System.Collections.IEnumerator AnimatePanel(bool open)
    {
        // Ensure active while animating
        if (panel != null) panel.SetActive(true);

        float t = 0f;
        float duration = Mathf.Max(0.01f, openCloseDuration);

        Vector2 startPos = open ? panelShownPos + hiddenOffset : panelShownPos;
        Vector2 endPos = open ? panelShownPos : panelShownPos + hiddenOffset;

        float startA = open ? 0f : 1f;
        float endA = open ? 1f : 0f;

        panelRect.anchoredPosition = startPos;
        if (panelCanvasGroup != null) panelCanvasGroup.alpha = startA;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            // easeOutCubic
            float eased = 1f - Mathf.Pow(1f - k, 3f);

            panelRect.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, eased);
            if (panelCanvasGroup != null) panelCanvasGroup.alpha = Mathf.LerpUnclamped(startA, endA, eased);

            yield return null;
        }

        panelRect.anchoredPosition = endPos;
        if (panelCanvasGroup != null) panelCanvasGroup.alpha = endA;

        if (!open)
        {
            if (panel != null) panel.SetActive(false);
            RestoreTimeAndCursor();
        }

        animRoutine = null;
    }

    void ApplyPanelVisual(bool open)
    {
        if (panelRect == null) return;
        panelRect.anchoredPosition = open ? panelShownPos : (panelShownPos + hiddenOffset);
        if (panelCanvasGroup != null) panelCanvasGroup.alpha = open ? 1f : 0f;
    }

    void ConfigureOverlayRect()
    {
        if (screenOverlayImage == null) return;
        RectTransform rt = screenOverlayImage.GetComponent<RectTransform>();
        if (rt == null) return;

        float h = Mathf.Clamp01(overlayScreenHeightPercent);

        // Stretch full width, bottom portion of screen
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, h);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Don't block button clicks
        screenOverlayImage.raycastTarget = false;
    }
}

