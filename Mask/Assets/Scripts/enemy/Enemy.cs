using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    protected enum EnemyState 
    {
        Moving,
        Attacking,
    }

    [SerializeField] protected EnemyConfig config;  // Drag your config asset here
    
    // These get their values from config in Start()
    protected float health;
    protected float moveSpeed;
    protected float attackRange;
    protected float damage;
    
    protected EnemyState currentState;
    protected Transform enemyTransform;
    protected static Transform playerTransform;  // Reference to the player
    
    // Collision pause variables
    protected float collisionPauseTime = 0.1f;
    protected float pauseTimer = 0f;
    protected bool isPaused = false;

    protected virtual void moveEnemy(float attackRange, float moveSpeed) {
        if (playerTransform == null) return;

        float distanceToTarget = Vector3.Distance(enemyTransform.position, playerTransform.position);
        if (distanceToTarget < attackRange) return;

        Vector3 direction = (playerTransform.position - enemyTransform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    
    protected virtual void Attack() {
        Debug.Log("Enemy is attacking");
    }


    public virtual void TakeDamage(float damage) {
        health -= damage;
        if (health <= 0) Die();
    }



    protected virtual void Die() {
        Destroy(gameObject);
    }


    void UpdateState () {
        if (playerTransform == null) return;
        
        float distanceToTarget = Vector3.Distance(enemyTransform.position, playerTransform.position);
        if (distanceToTarget < attackRange) {
            currentState = EnemyState.Attacking;
        }
        else {
            currentState = EnemyState.Moving;
        }
    }



    // Start is called before the first frame update
    protected virtual void Start()
    {
        enemyTransform = transform;
        
        // Find the player (only once, since it's static)
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        
        // Load values from config
        if (config != null)
        {
            health = config.health;
            moveSpeed = config.moveSpeed;
            attackRange = config.attackRange;  
            damage = config.damage;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
        switch (currentState) {
            case EnemyState.Moving:
                moveEnemy(attackRange, moveSpeed);
                break;
            case EnemyState.Attacking:
                Attack();
                break;
        }
    }
}
