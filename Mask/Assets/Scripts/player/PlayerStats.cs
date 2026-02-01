using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] float baseMoveSpeed = 5f;
    [SerializeField] float baseDamage = 10f;
    [SerializeField] float baseFireRate = 2f; // Shots per second
    [SerializeField] float baseProjectileSpeed = 10f;
    [SerializeField] float baseMaxHealth = 100f;

    [Header("Mask Multipliers (runtime)")]
    [SerializeField] float maskMoveSpeedMultiplier = 1f;
    [SerializeField] float maskDamageMultiplier = 1f;
    [SerializeField] float maskFireRateMultiplier = 1f;
    [SerializeField] float maskProjectileSpeedMultiplier = 1f;
    [SerializeField] float maskMaxHealthMultiplier = 1f;

    [Header("Upgrade Multipliers (runtime)")]
    [SerializeField] float upgradeMoveSpeedMultiplier = 1f;
    [SerializeField] float upgradeDamageMultiplier = 1f;
    [SerializeField] float upgradeFireRateMultiplier = 1f;
    [SerializeField] float upgradeProjectileSpeedMultiplier = 1f;
    [SerializeField] float upgradeMaxHealthMultiplier = 1f;

    public float CurrentMoveSpeed => baseMoveSpeed * maskMoveSpeedMultiplier * upgradeMoveSpeedMultiplier;
    public float CurrentDamage => baseDamage * maskDamageMultiplier * upgradeDamageMultiplier;
    public float CurrentFireRate => baseFireRate * maskFireRateMultiplier * upgradeFireRateMultiplier;
    public float CurrentProjectileSpeed => baseProjectileSpeed * maskProjectileSpeedMultiplier * upgradeProjectileSpeedMultiplier;
    public float CurrentMaxHealth => baseMaxHealth * maskMaxHealthMultiplier * upgradeMaxHealthMultiplier;

    public void SetBaseMoveSpeed(float value) => baseMoveSpeed = value;
    public void SetBaseDamage(float value) => baseDamage = value;
    public void SetBaseFireRate(float value) => baseFireRate = value;
    public void SetBaseProjectileSpeed(float value) => baseProjectileSpeed = value;
    public void SetBaseMaxHealth(float value) => baseMaxHealth = value;

    public void ApplyMultipliers(float newMoveSpeedMultiplier, float newDamageMultiplier, float newFireRateMultiplier, float newProjectileSpeedMultiplier, float newMaxHealthMultiplier)
    {
        // Backwards-compatible name: this sets MASK multipliers.
        maskMoveSpeedMultiplier = Mathf.Max(0f, newMoveSpeedMultiplier);
        maskDamageMultiplier = Mathf.Max(0f, newDamageMultiplier);
        maskFireRateMultiplier = Mathf.Max(0f, newFireRateMultiplier);
        maskProjectileSpeedMultiplier = Mathf.Max(0f, newProjectileSpeedMultiplier);
        maskMaxHealthMultiplier = Mathf.Max(0f, newMaxHealthMultiplier);
    }

    public void ResetUpgradeMultipliers()
    {
        upgradeMoveSpeedMultiplier = 1f;
        upgradeDamageMultiplier = 1f;
        upgradeFireRateMultiplier = 1f;
        upgradeProjectileSpeedMultiplier = 1f;
        upgradeMaxHealthMultiplier = 1f;
    }

    public void MultiplyUpgradeDamage(float factor) => upgradeDamageMultiplier *= Mathf.Max(0f, factor);
    public void MultiplyUpgradeFireRate(float factor) => upgradeFireRateMultiplier *= Mathf.Max(0f, factor);
    public void MultiplyUpgradeMoveSpeed(float factor) => upgradeMoveSpeedMultiplier *= Mathf.Max(0f, factor);
    public void MultiplyUpgradeProjectileSpeed(float factor) => upgradeProjectileSpeedMultiplier *= Mathf.Max(0f, factor);
    public void MultiplyUpgradeMaxHealth(float factor) => upgradeMaxHealthMultiplier *= Mathf.Max(0f, factor);
}

