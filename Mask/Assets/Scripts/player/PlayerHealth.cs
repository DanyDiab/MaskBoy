using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] bool clampToMaxHealth = true;
    float currentHealth;
    PlayerStats playerStats;

    [Header("Over Time (runtime)")]
    [SerializeField] float regenPerSecond = 0f;
    [SerializeField] float drainPerSecond = 0f;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => playerStats != null ? playerStats.CurrentMaxHealth : 100f;

    // Event for damage shower
    public static event Action<float, Vector3> OnPlayerDamage;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        currentHealth = MaxHealth;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        float delta = (regenPerSecond - drainPerSecond) * dt;
        if (Mathf.Abs(delta) > 0f){
            if (clampToMaxHealth) TakeDamage(delta);
            if (currentHealth <= 0f) Die();
        }
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
        if (currentHealth <= 0f) Die();
    }

    void Die()
    {
        Debug.Log("Player died!");
        AudioManager.Play(SoundType.PlayerDeath);
        currentHealth = MaxHealth * .1f;
    }
}

