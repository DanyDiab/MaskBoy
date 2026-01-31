using UnityEngine;

[CreateAssetMenu(fileName = "NewMaskConfig", menuName = "Masks/Mask Config")]
public class MaskConfig : ScriptableObject
{
    [Header("UI")]
    public string displayName = "New Mask";
    public Sprite icon;
    public Color iconTint = Color.white;

    [Header("Stat Modifiers")]
    [Tooltip("Multiplies player projectile damage.")]
    public float damageMultiplier = 1f;
    [Tooltip("Multiplies player movement speed.")]
    public float moveSpeedMultiplier = 1f;

    [Header("Health Over Time")]
    [Tooltip("Positive values heal per second.")]
    public float regenPerSecond = 0f;
    [Tooltip("Positive values drain per second.")]
    public float hpDrainPerSecond = 0f;

    [Header("Simple Effects (v1)")]
    public Color screenOverlayColor = new Color(0, 0, 0, 0);
    public Color playerTint = Color.white;
    public AudioClip loopSfx;
}

