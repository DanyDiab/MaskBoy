using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static event Action<float, Vector3> OnPlayerDamage;
    public static event Action<float, float> OnPlayerHealthChanged;

    [Header("Current")]
    [SerializeField] float currentHealth = 100f;
    [SerializeField] bool clampToMaxHealth = true;

    [Header("Over Time (runtime)")]
    [SerializeField] float regenPerSecond = 0f;
    [SerializeField] float drainPerSecond = 0f;

    [Header("Ticking")]
    [SerializeField] bool useDiscreteTicks = true;
    [SerializeField] float tickIntervalSeconds = 1f;

    PlayerStats playerStats;
    float tickTimer = 0f;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => playerStats != null ? playerStats.CurrentMaxHealth : 100f;

    public float RegenPerSecond => regenPerSecond;
    public float DrainPerSecond => drainPerSecond;
    public float NetPerSecond => regenPerSecond - drainPerSecond;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

        // If we spawned with a bogus value, normalize to max.
        float max = Mathf.Max(1f, MaxHealth);
        if (currentHealth <= 0f || currentHealth > max) currentHealth = max;

        OnPlayerHealthChanged?.Invoke(currentHealth, max);
    }

    void Update()
    {
        float netPerSecond = regenPerSecond - drainPerSecond;
        if (Mathf.Approximately(netPerSecond, 0f)) return;

        if (useDiscreteTicks)
        {
            tickTimer += Time.deltaTime;
            float interval = Mathf.Max(0.01f, tickIntervalSeconds);

            while (tickTimer >= interval)
            {
                tickTimer -= interval;
                ApplyHealthDelta(netPerSecond * interval);
            }
        }
        else
        {
            ApplyHealthDelta(netPerSecond * Time.deltaTime);
        }
    }

    public void SetOverTime(float regen, float drain)
    {
        regenPerSecond = Mathf.Max(0f, regen);
        drainPerSecond = Mathf.Max(0f, drain);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f) return;

        float max = Mathf.Max(1f, MaxHealth);
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, max);

        // Positive numbers mean "damage" for UI (red).
        OnPlayerDamage?.Invoke(damage, transform.position);
        OnPlayerHealthChanged?.Invoke(currentHealth, max);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void ApplyHealthDelta(float delta)
    {
        if (Mathf.Approximately(delta, 0f)) return;

        float max = Mathf.Max(1f, MaxHealth);
        float before = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + delta, 0f, max);

        float applied = currentHealth - before; // signed
        if (!Mathf.Approximately(applied, 0f))
        {
            // For UI: negative values mean healing (green), positive values mean damage/drain (red).
            float uiValue = applied > 0f ? -applied : Mathf.Abs(applied);
            OnPlayerDamage?.Invoke(uiValue, transform.position);
            OnPlayerHealthChanged?.Invoke(currentHealth, max);
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void ScaleHealth(float ratio)
    {
        if (ratio <= 0f) return;
        currentHealth *= ratio;
        if (clampToMaxHealth && currentHealth > MaxHealth) currentHealth = MaxHealth;
    }

    void Die()
    {
        float max = Mathf.Max(1f, MaxHealth);
        currentHealth = max * 0.1f; // keep UI from showing 0
        OnPlayerHealthChanged?.Invoke(currentHealth, max);

        GameSceneManager.LoadScene("GameOver");
    }
}

