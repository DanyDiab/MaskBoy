using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonClown : Enemy
{
    enum ClownState {
        Moving,
        Throwing,
        Cooldown
    }
    
    [Header("Throwing")]
    [SerializeField] GameObject balloonBombPrefab;
    [SerializeField] float throwRange = 5f;
    [SerializeField] float throwCooldown = 2f;
    [SerializeField] float throwWindup = 0.5f;
    [SerializeField] float bombDamage = 20f;
    
    ClownState clownState = ClownState.Moving;
    float stateTimer = 0f;
    
    void Update() {
        // Try to find player if not found yet
        if (playerTransform == null) {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) {
                playerTransform = player.transform;
            } else {
                return;
            }
        }
        
        if (enemyTransform == null) {
            enemyTransform = transform;
        }
        
        if (isPaused) {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0) isPaused = false;
            return;
        }
        
        float distanceToPlayer = Vector3.Distance(enemyTransform.position, playerTransform.position);
        
        switch (clownState) {
            case ClownState.Moving:
                // Move toward player
                if (distanceToPlayer > throwRange) {
                    moveEnemy(attackRange, moveSpeed);
                } else {
                    // In range - start throwing
                    clownState = ClownState.Throwing;
                    stateTimer = 0f;
                }
                break;
                
            case ClownState.Throwing:
                // Wind up before throw
                stateTimer += Time.deltaTime;
                if (stateTimer >= throwWindup) {
                    ThrowBalloon();
                    clownState = ClownState.Cooldown;
                    stateTimer = 0f;
                }
                break;
                
            case ClownState.Cooldown:
                // Wait before next action
                stateTimer += Time.deltaTime;
                if (stateTimer >= throwCooldown) {
                    clownState = ClownState.Moving;
                    stateTimer = 0f;
                }
                break;
        }
    }
    
    void ThrowBalloon() {
        if (balloonBombPrefab == null) {
            Debug.LogWarning("BalloonClown: No balloon bomb prefab assigned!");
            return;
        }
        
        // Spawn the balloon bomb
        GameObject bomb = Instantiate(balloonBombPrefab, enemyTransform.position, Quaternion.identity);
        
        // Initialize it with target, damage, and shooter reference
        BalloonBomb bombScript = bomb.GetComponent<BalloonBomb>();
        if (bombScript != null) {
            bombScript.Initialize(playerTransform.position, bombDamage, gameObject);
        }
        
        Debug.Log("BalloonClown threw a balloon!");
    }
    
    protected override void Start() {
        base.Start();
        
        // Fallback if no config assigned
        if (config == null) {
            moveSpeed = 3f;
            attackRange = 5f;
            Debug.LogWarning("BalloonClown: No config assigned, using default moveSpeed");
        }
        
        Debug.Log($"BalloonClown started - moveSpeed: {moveSpeed}, attackRange: {attackRange}");
    }
}
