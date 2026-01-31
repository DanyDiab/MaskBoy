using UnityEngine;
using UnityEngine.UI;

public class MaskManager : MonoBehaviour
{
    [Header("Current")]
    [SerializeField] MaskConfig currentMask;

    [Header("Optional References (assign in Inspector)")]
    [SerializeField] PlayerStats playerStats;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] SpriteRenderer playerSpriteRenderer;
    [SerializeField] Image screenOverlayImage;
    [SerializeField] AudioSource loopAudioSource;

    Color defaultPlayerTint = Color.white;
    Color defaultOverlayColor = new Color(0, 0, 0, 0);

    void Awake()
    {
        if (playerStats == null) playerStats = GetComponent<PlayerStats>();
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();
        if (playerSpriteRenderer == null) playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (loopAudioSource == null) loopAudioSource = GetComponent<AudioSource>();

        if (playerSpriteRenderer != null) defaultPlayerTint = playerSpriteRenderer.color;
        if (screenOverlayImage != null) defaultOverlayColor = screenOverlayImage.color;
    }

    public void Equip(MaskConfig mask)
    {
        currentMask = mask;

        if (mask == null)
        {
            // Reset to defaults
            if (playerStats != null) playerStats.ApplyMultipliers(1f, 1f, 1f, 1f, 1f);
            if (playerHealth != null) playerHealth.SetOverTime(0f, 0f);
            if (playerSpriteRenderer != null) playerSpriteRenderer.color = defaultPlayerTint;
            if (screenOverlayImage != null) screenOverlayImage.color = defaultOverlayColor;
            StopLoopSfx();
            return;
        }

        if (playerStats != null) playerStats.ApplyMultipliers(mask.moveSpeedMultiplier, mask.damageMultiplier, mask.fireRateMultiplier, mask.projectileSpeedMultiplier, mask.maxHealthMultiplier);
        if (playerHealth != null) playerHealth.SetOverTime(mask.regenPerSecond, mask.hpDrainPerSecond);

        if (playerSpriteRenderer != null) playerSpriteRenderer.color = mask.playerTint;
        if (screenOverlayImage != null) screenOverlayImage.color = mask.screenOverlayColor;

        PlayLoopSfx(mask.loopSfx);
    }

    void PlayLoopSfx(AudioClip clip)
    {
        if (loopAudioSource == null) return;

        if (clip == null)
        {
            StopLoopSfx();
            return;
        }

        loopAudioSource.clip = clip;
        loopAudioSource.loop = true;
        if (!loopAudioSource.isPlaying) loopAudioSource.Play();
    }

    void StopLoopSfx()
    {
        if (loopAudioSource == null) return;
        loopAudioSource.Stop();
        loopAudioSource.clip = null;
    }

    // Used by UI manager to set overlay reference at runtime
    public void SetOverlay(Image overlay) => screenOverlayImage = overlay;
}

