using UnityEngine;
using System;
using System.Text;

public static class UpgradeApplier
{
    // text, color, world position
    public static event Action<string, Color, Vector3> OnUpgradePopup;

    public static void ApplyToPlayer(GameObject playerObj, UpgradeConfig upgrade)
    {
        if (playerObj == null || upgrade == null) return;

        PlayerStats stats = playerObj.GetComponent<PlayerStats>();
        PlayerHealth health = playerObj.GetComponent<PlayerHealth>();

        if (upgrade.effects == null) return;

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < upgrade.effects.Length; i++)
        {
            UpgradeEffect e = upgrade.effects[i];
            ApplyEffect(stats, health, e);

            string line = FormatEffect(e);
            if (!string.IsNullOrEmpty(line))
            {
                if (sb.Length > 0) sb.Append('\n');
                sb.Append(line);
            }
        }

        Debug.Log($"Picked up upgrade: {upgrade.displayName}");

        if (sb.Length > 0)
        {
            // Yellow-ish for upgrades
            OnUpgradePopup?.Invoke(sb.ToString(), new Color(1f, 0.9f, 0.2f, 1f), playerObj.transform.position);
        }
    }

    static void ApplyEffect(PlayerStats stats, PlayerHealth health, UpgradeEffect e)
    {
        switch (e.stat)
        {
            case UpgradeStat.DamagePercent:
                if (stats != null) stats.MultiplyUpgradeDamage(1f + e.percent);
                break;
            case UpgradeStat.FireRatePercent:
                if (stats != null) stats.MultiplyUpgradeFireRate(1f + e.percent);
                break;
            case UpgradeStat.MoveSpeedPercent:
                if (stats != null) stats.MultiplyUpgradeMoveSpeed(1f + e.percent);
                break;
            case UpgradeStat.ProjectileSpeedPercent:
                if (stats != null) stats.MultiplyUpgradeProjectileSpeed(1f + e.percent);
                break;
            case UpgradeStat.MaxHealthPercent:
                if (stats != null) stats.MultiplyUpgradeMaxHealth(1f + e.percent);
                break;
            case UpgradeStat.RegenPerSecondAdd:
                if (health != null) health.SetOverTime(health.RegenPerSecond + e.flat, health.DrainPerSecond);
                break;
        }
    }

    static string FormatEffect(UpgradeEffect e)
    {
        switch (e.stat)
        {
            case UpgradeStat.DamagePercent:
                return $"+{Mathf.RoundToInt(e.percent * 100f)}% Damage";
            case UpgradeStat.FireRatePercent:
                return $"+{Mathf.RoundToInt(e.percent * 100f)}% ROF";
            case UpgradeStat.MoveSpeedPercent:
                return $"+{Mathf.RoundToInt(e.percent * 100f)}% Move Speed";
            case UpgradeStat.ProjectileSpeedPercent:
                return $"+{Mathf.RoundToInt(e.percent * 100f)}% Projectile Speed";
            case UpgradeStat.MaxHealthPercent:
                return $"+{Mathf.RoundToInt(e.percent * 100f)}% Max HP";
            case UpgradeStat.RegenPerSecondAdd:
                return $"+{e.flat:0.#} Regen";
            default:
                return null;
        }
    }
}

