using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "Enemy/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    
    [Header("Combat")]
    public float health = 100f;
    public float damage = 10f;
    public float attackRange = 1.0f;
}
