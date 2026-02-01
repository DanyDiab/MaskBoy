using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] bool clampToMaxHealth = true;
    [SerializeField] float currentHealth;
    PlayerStats playerStats;

    [Header("Over Time (runtime)")]
    [SerializeField] float regenPerSecond = 0f;
    [SerializeField] float drainPerSecond = 0f;
    [SerializeField] bool useDiscreteTicks = true;
    [SerializeField] float tickIntervalSeconds = 1f;
    float tickTimer = 0f;
    [SerializeField] bool showOverTimePopups = true;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => playerStats != null ? playerStats.CurrentMaxHealth : 100f;
    public float RegenPerSecond => regenPerSecond;
    public float DrainPerSecond => drainPerSecond;
    public float NetPerSecond => regenPerSecond - drainPerSecond;

    // Event for damage shower
    public static event Action<float, Vector3> OnPlayerDamage;
    // Event for HUD (current, max)
    public static event Action<float, float> OnPlayerHealthChanged;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        currentHealth = MaxHealth;
        OnPlayerHealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        float perSecondNet = (regenPerSecond - drainPerSecond);
        if (Mathf.Abs(perSecondNet) <= 0f) return;

        if (useDiscreteTicks)
        {
            tickTimer += dt;
            float interval = Mathf.Max(0.05f, tickIntervalSeconds);

            while (tickTimer >= interval)
            {
                tickTimer -= interval;
                ApplyHealthDelta(perSecondNet * interval);
            }
        }
        else
        {
            // Smooth over-time (per frame)
            ApplyHealthDelta(perSecondNet * dt);
        }
    }

    void ApplyHealthDelta(float delta)
    {
        if (Mathf.Abs(delta) <= 0f) return;

        float before = currentHealth;
        currentHealth += delta;

        if (clampToMaxHealth)
        {
            float max = MaxHealth;
            if (currentHealth > max) currentHealth = max;
        }

        float actualDelta = currentHealth - before;

        // Drive DamageShower/DamageText:
        // - positive value = damage (red)
        // - negative value = heal (green)
        if (showOverTimePopups && Mathf.Abs(actualDelta) > 0f)
        {
            // convert health delta to "damage amount" convention
            OnPlayerDamage?.Invoke(-actualDelta, transform.position);
        }

        if (Mathf.Abs(actualDelta) > 0f)
        {
            OnPlayerHealthChanged?.Invoke(currentHealth, MaxHealth);
        }

        if (currentHealth <= 0f) Die();
    }

    public void SetOverTime(float regen, float drain)
    {
        regenPerSecond = Mathf.Max(0f, regen);
        drainPerSecond = Mathf.Max(0f, drain);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        OnPlayerDamage?.Invoke(damage, transform.position);
        OnPlayerHealthChanged?.Invoke(currentHealth, MaxHealth);
        if (currentHealth <= 0f) Die();
    }

    void Die()
    {
        Debug.Log("Player died!");
        AudioManager.Play(SoundType.PlayerDeath);
        currentHealth = MaxHealth * .1f;
        OnPlayerHealthChanged?.Invoke(currentHealth, MaxHealth);
    }
}

