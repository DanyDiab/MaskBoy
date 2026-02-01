using UnityEngine;

public class UpgradeDropManager : MonoBehaviour
{
    [Header("Drop Settings")]
    [Range(0f, 1f)]
    [SerializeField] float dropChance = 0.2f;
    [SerializeField] UpgradeConfig[] possibleUpgrades;

    [Header("Pickup Prefab")]
    [SerializeField] GameObject upgradePickupPrefab;
    [SerializeField] bool debugLogs = true;

    void OnEnable()
    {
        Enemy.OnEnemyDeath += OnEnemyDeath;
    }

    void OnDisable()
    {
        Enemy.OnEnemyDeath -= OnEnemyDeath;
    }

    void OnEnemyDeath(Vector3 pos)
    {
        if (upgradePickupPrefab == null) return;
        if (possibleUpgrades == null || possibleUpgrades.Length == 0) return;

        if (Random.value > dropChance) return;

        UpgradeConfig chosen = ChooseWeighted(possibleUpgrades);
        if (chosen == null) return;

        GameObject pickup = Instantiate(upgradePickupPrefab, pos, Quaternion.identity);
        if (debugLogs) Debug.Log($"UpgradeDropManager: Dropped '{chosen.displayName}' at {pos}");
        UpgradePickup pickupScript = pickup.GetComponent<UpgradePickup>();
        if (pickupScript != null)
        {
            pickupScript.SetConfig(chosen);
        }
    }

    static UpgradeConfig ChooseWeighted(UpgradeConfig[] upgrades)
    {
        float total = 0f;
        for (int i = 0; i < upgrades.Length; i++)
        {
            if (upgrades[i] == null) continue;
            total += Mathf.Max(0f, upgrades[i].dropWeight);
        }

        if (total <= 0f) return upgrades[0];

        float r = Random.Range(0f, total);
        float c = 0f;
        for (int i = 0; i < upgrades.Length; i++)
        {
            UpgradeConfig u = upgrades[i];
            if (u == null) continue;
            c += Mathf.Max(0f, u.dropWeight);
            if (r <= c) return u;
        }

        return upgrades[0];
    }
}

