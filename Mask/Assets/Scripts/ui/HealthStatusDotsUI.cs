using UnityEngine;
using UnityEngine.UI;

public class HealthStatusDotsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] Transform playerTransform;
    [SerializeField] Image healDot;
    [SerializeField] Image damageDot;

    [Header("Follow Player (screen space UI)")]
    [SerializeField] Canvas canvas;
    [Tooltip("Offset from player in canvas pixels (X left -, Y up +).")]
    [SerializeField] Vector2 playerOffset = new Vector2(-60f, 60f);
    [Tooltip("Offset between heal dot and damage dot.")]
    [SerializeField] Vector2 dotSpacing = new Vector2(22f, 0f);

    [Header("Behavior")]
    [SerializeField] float damageFlashSeconds = 0.25f;
    [SerializeField] float healFlashSeconds = 0.25f;
    [SerializeField] float healthChangeEpsilon = 0.001f;

    float damageFlashTimer = 0f;
    float healFlashTimer = 0f;
    float lastHealth = float.NaN;
    RectTransform canvasRect;
    RectTransform healRect;
    RectTransform damageRect;

    void Awake()
    {
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (canvas != null) canvasRect = canvas.GetComponent<RectTransform>();

        if (playerHealth == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
                playerTransform = player.transform;
            }
        }

        if (playerTransform == null && playerHealth != null) playerTransform = playerHealth.transform;

        if (healDot != null) healRect = healDot.GetComponent<RectTransform>();
        if (damageDot != null) damageRect = damageDot.GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        PlayerHealth.OnPlayerDamage += HandlePlayerDamage;
    }

    void OnDisable()
    {
        PlayerHealth.OnPlayerDamage -= HandlePlayerDamage;
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        if (damageFlashTimer > 0f) damageFlashTimer -= dt;
        if (healFlashTimer > 0f) healFlashTimer -= dt;

        UpdateDotPositions();

        if (playerHealth == null)
        {
            SetVisible(heal: healFlashTimer > 0f, damage: damageFlashTimer > 0f);
            return;
        }

        // Flash only when HP changes (heal tick / drain tick / damage hit).
        float current = playerHealth.CurrentHealth;
        if (float.IsNaN(lastHealth)) lastHealth = current;

        float diff = current - lastHealth;
        if (Mathf.Abs(diff) > healthChangeEpsilon)
        {
            if (diff > 0f) healFlashTimer = healFlashSeconds;
            else damageFlashTimer = damageFlashSeconds;

            lastHealth = current;
        }

        SetVisible(heal: healFlashTimer > 0f, damage: damageFlashTimer > 0f);
    }

    void UpdateDotPositions()
    {
        if (canvasRect == null || playerTransform == null || healRect == null || damageRect == null) return;

        Camera cam = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        }

        Vector3 screenPos = cam != null
            ? cam.WorldToScreenPoint(playerTransform.position)
            : Camera.main.WorldToScreenPoint(playerTransform.position);

        // Convert screen -> canvas local
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out Vector2 localPoint))
        {
            Vector2 basePos = localPoint + playerOffset;
            healRect.anchoredPosition = basePos;
            damageRect.anchoredPosition = basePos + dotSpacing;
        }
    }

    void HandlePlayerDamage(float damage, Vector3 worldPos)
    {
        // Only flash for real incoming damage (positive values)
        if (damage > 0f) damageFlashTimer = damageFlashSeconds;
    }

    void SetVisible(bool heal, bool damage)
    {
        if (healDot != null) healDot.enabled = heal;
        if (damageDot != null) damageDot.enabled = damage;
    }
}

