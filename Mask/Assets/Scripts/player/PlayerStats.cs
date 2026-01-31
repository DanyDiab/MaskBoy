using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] float baseMoveSpeed = 5f;
    [SerializeField] float baseDamage = 10f;

    [Header("Mask Multipliers (runtime)")]
    [SerializeField] float moveSpeedMultiplier = 1f;
    [SerializeField] float damageMultiplier = 1f;

    public float CurrentMoveSpeed => baseMoveSpeed * moveSpeedMultiplier;
    public float CurrentDamage => baseDamage * damageMultiplier;

    public void SetBaseMoveSpeed(float value) => baseMoveSpeed = value;
    public void SetBaseDamage(float value) => baseDamage = value;

    public void ApplyMultipliers(float newMoveSpeedMultiplier, float newDamageMultiplier)
    {
        moveSpeedMultiplier = Mathf.Max(0f, newMoveSpeedMultiplier);
        damageMultiplier = Mathf.Max(0f, newDamageMultiplier);
    }
}

