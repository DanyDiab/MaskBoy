using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugglerClown : Enemy
{
    enum JugglerState {
        Moving,
        Stopping,
        Throwing,
        Cooldown
    }
    
    [Header("Throwing")]
    [SerializeField] GameObject jugglingBallPrefab;
    [SerializeField] float throwRange = 6f;
    [SerializeField] float throwCooldown = 1.5f;
    [SerializeField] float stopDuration = 0.25f;
    [SerializeField] float ballDamage = 10f;
    [SerializeField] float burstSpreadAngle = 20f; // Degrees between burst shots
    
    [Header("Cone Movement")]
    [SerializeField] float coneAngle = 90f; // Total cone angle in degrees (wider = more erratic)
    [SerializeField] float minMoveDistance = 0.5f; // Min distance before picking new point
    [SerializeField] float maxMoveDistance = 1.5f; // Max distance before picking new point
    [SerializeField] float reachThreshold = 0.2f; // How close before picking new point
    [SerializeField] float directionChangeInterval = 0.3f; // Randomly change direction this often
    [SerializeField] float speedVariation = 0.5f; // Random speed multiplier variance
    
    JugglerState jugglerState = JugglerState.Moving;
    float stateTimer = 0f;
    int throwCount = 0; // Tracks 1, 2, 3 pattern
    Vector3 currentMoveTarget;
    bool hasValidMoveTarget = false;
    float directionChangeTimer = 0f;
    float currentSpeedMultiplier = 1f;
    
    protected override void Start() {
        base.Start();
        
        // Fallback values if config is null
        if (config == null) {
            moveSpeed = 3f;
            attackRange = throwRange;
        }
        
        PickNewMoveTarget();
    }
    
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
        
        // Handle pause from collisions
        if (isPaused) {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0) isPaused = false;
            return;
        }
        
        float distanceToPlayer = Vector3.Distance(enemyTransform.position, playerTransform.position);
        
        switch (jugglerState) {
            case JugglerState.Moving:
                // Erratic cone movement toward player
                MoveErratically();
                
                // Check if in throw range
                if (distanceToPlayer <= throwRange) {
                    jugglerState = JugglerState.Stopping;
                    stateTimer = 0f;
                }
                break;
                
            case JugglerState.Stopping:
                // Brief pause before throwing
                stateTimer += Time.deltaTime;
                if (stateTimer >= stopDuration) {
                    jugglerState = JugglerState.Throwing;
                    ThrowBalls();
                }
                break;
                
            case JugglerState.Throwing:
                // Throwing happens instantly, move to cooldown
                jugglerState = JugglerState.Cooldown;
                stateTimer = 0f;
                break;
                
            case JugglerState.Cooldown:
                stateTimer += Time.deltaTime;
                if (stateTimer >= throwCooldown) {
                    jugglerState = JugglerState.Moving;
                    PickNewMoveTarget();
                }
                break;
        }
    }
    
    void MoveErratically() {
        if (!hasValidMoveTarget) {
            PickNewMoveTarget();
        }
        
        // Randomly change direction on a timer (adds unpredictability)
        directionChangeTimer += Time.deltaTime;
        if (directionChangeTimer >= directionChangeInterval) {
            directionChangeTimer = 0f;
            // 50% chance to pick a new direction early
            if (Random.value > 0.5f) {
                PickNewMoveTarget();
            }
        }
        
        // Move toward current target point with varying speed
        Vector3 direction = (currentMoveTarget - enemyTransform.position).normalized;
        float actualSpeed = moveSpeed * currentSpeedMultiplier;
        transform.Translate(direction * actualSpeed * Time.deltaTime);
        
        // Check if reached target point
        float distanceToTarget = Vector3.Distance(enemyTransform.position, currentMoveTarget);
        if (distanceToTarget < reachThreshold) {
            PickNewMoveTarget();
        }
    }
    
    void PickNewMoveTarget() {
        if (playerTransform == null) {
            hasValidMoveTarget = false;
            return;
        }
        
        // Direction to player
        Vector3 toPlayer = (playerTransform.position - enemyTransform.position).normalized;
        
        // Get a random angle within the cone (wider cone = more erratic)
        float halfCone = coneAngle / 2f;
        float randomAngle = Random.Range(-halfCone, halfCone);
        
        // Rotate direction by random angle
        Vector3 randomDirection = RotateVector(toPlayer, randomAngle);
        
        // Random distance for this move segment
        float moveDistance = Random.Range(minMoveDistance, maxMoveDistance);
        
        // Random speed multiplier for variety
        currentSpeedMultiplier = 1f + Random.Range(-speedVariation, speedVariation);
        
        // Set target point
        currentMoveTarget = enemyTransform.position + randomDirection * moveDistance;
        hasValidMoveTarget = true;
        
        // Reset direction change timer
        directionChangeTimer = 0f;
    }
    
    Vector3 RotateVector(Vector3 v, float degrees) {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector3(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos,
            0
        );
    }
    
    void ThrowBalls() {
        if (jugglingBallPrefab == null) {
            Debug.LogWarning("JugglerClown: No juggling ball prefab assigned!");
            return;
        }
        
        throwCount++;
        
        if (throwCount <= 2) {
            // Single ball throw
            ThrowSingleBall(playerTransform.position);
            Debug.Log("JugglerClown: Single throw #" + throwCount);
        } else {
            // Burst of 3 balls
            ThrowBurstBalls();
            throwCount = 0; // Reset pattern
            Debug.Log("JugglerClown: Burst throw!");
        }
    }
    
    void ThrowSingleBall(Vector3 targetPos) {
        Vector3 direction = (targetPos - enemyTransform.position).normalized;
        SpawnBall(direction);
    }
    
    void ThrowBurstBalls() {
        Vector3 baseDirection = (playerTransform.position - enemyTransform.position).normalized;
        
        // Center ball
        SpawnBall(baseDirection);
        
        // Left ball
        Vector3 leftDir = RotateVector(baseDirection, -burstSpreadAngle);
        SpawnBall(leftDir);
        
        // Right ball
        Vector3 rightDir = RotateVector(baseDirection, burstSpreadAngle);
        SpawnBall(rightDir);
    }
    
    void SpawnBall(Vector3 direction) {
        GameObject ball = Instantiate(jugglingBallPrefab, enemyTransform.position, Quaternion.identity);
        JugglingBall ballScript = ball.GetComponent<JugglingBall>();
        if (ballScript != null) {
            ballScript.Initialize(direction, ballDamage, gameObject);
        }
    }
}
