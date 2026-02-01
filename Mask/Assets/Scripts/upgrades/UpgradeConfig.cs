using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgradeConfig", menuName = "Upgrades/Upgrade Config")]
public class UpgradeConfig : ScriptableObject
{
    [Header("UI")]
    public string displayName = "New Upgrade";
    public Sprite icon;

    [Header("Drop Tuning")]
    [Tooltip("Relative weight when randomly choosing which upgrade to drop.")]
    public float dropWeight = 1f;

    [Header("Effects")]
    public UpgradeEffect[] effects;
}

[Serializable]
public struct UpgradeEffect
{
    public UpgradeStat stat;

    [Tooltip("Percent boost as a decimal. Example: 0.15 = +15%.")]
    public float percent;

    [Tooltip("Flat additive amount (used for stats like regen).")]
    public float flat;
}

public enum UpgradeStat
{
    DamagePercent,
    FireRatePercent,
    MoveSpeedPercent,
    ProjectileSpeedPercent,
    MaxHealthPercent,
    RegenPerSecondAdd,
}

