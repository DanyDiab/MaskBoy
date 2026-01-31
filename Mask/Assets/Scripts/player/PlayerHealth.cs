using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] bool clampToMaxHealth = true;
    float currentHealth;

    [Header("Over Time (runtime)")]
    [SerializeField] float regenPerSecond = 0f;
    [SerializeField] float drainPerSecond = 0f;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        float delta = (regenPerSecond - drainPerSecond) * dt;
        if (Mathf.Abs(delta) > 0f)
        {
            currentHealth += delta;
            if (clampToMaxHealth) currentHealth = Mathf.Min(currentHealth, maxHealth);
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
        Debug.Log($"Player took {damage} damage! Health: {currentHealth}/{maxHealth}");
        if (currentHealth <= 0f) Die();
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Add death logic here (respawn, game over, etc.)
    }
}

