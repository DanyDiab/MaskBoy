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
    [SerializeField] float moveSpeedMultiplier = 1f;
    [SerializeField] float damageMultiplier = 1f;
    [SerializeField] float fireRateMultiplier = 1f;
    [SerializeField] float projectileSpeedMultiplier = 1f;
    [SerializeField] float maxHealthMultiplier = 1f;

    public float CurrentMoveSpeed => baseMoveSpeed * moveSpeedMultiplier;
    public float CurrentDamage => baseDamage * damageMultiplier;
    public float CurrentFireRate => baseFireRate * fireRateMultiplier;
    public float CurrentProjectileSpeed => baseProjectileSpeed * projectileSpeedMultiplier;
    public float CurrentMaxHealth => baseMaxHealth * maxHealthMultiplier;

    public void SetBaseMoveSpeed(float value) => baseMoveSpeed = value;
    public void SetBaseDamage(float value) => baseDamage = value;
    public void SetBaseFireRate(float value) => baseFireRate = value;
    public void SetBaseProjectileSpeed(float value) => baseProjectileSpeed = value;
    public void SetBaseMaxHealth(float value) => baseMaxHealth = value;

    public void ApplyMultipliers(float newMoveSpeedMultiplier, float newDamageMultiplier, float newFireRateMultiplier, float newProjectileSpeedMultiplier, float newMaxHealthMultiplier)
    {
        moveSpeedMultiplier = Mathf.Max(0f, newMoveSpeedMultiplier);
        damageMultiplier = Mathf.Max(0f, newDamageMultiplier);
        fireRateMultiplier = Mathf.Max(0f, newFireRateMultiplier);
        projectileSpeedMultiplier = Mathf.Max(0f, newProjectileSpeedMultiplier);
        maxHealthMultiplier = Mathf.Max(0f, newMaxHealthMultiplier);
    }
}

