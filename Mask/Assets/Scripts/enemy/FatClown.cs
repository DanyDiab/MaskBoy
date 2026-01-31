using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatClown : Enemy
{

    enum FatMove {
        Moving,
        Charging,
        Dashing,
        Tired,
    }

    [SerializeField] float dashSpeed = 15f;   
    [SerializeField] float chargeUpTime = 0.5f;
    [SerializeField] float maxDashDistance = 8f;  // How far to dash before getting tired
    [SerializeField] float tiredDuration = 0.5f;  // How long to rest when tired

    Vector3 dashDirection;
    Vector3 dashStartPosition;
    float chargeTimer = 0f;
    float tiredTimer = 0f;
    FatMove currentMove = FatMove.Moving;



    protected override void moveEnemy(float attackRange, float moveSpeed) {
        if (playerTransform == null) return;
        
        float distanceToTarget = Vector3.Distance(enemyTransform.position, playerTransform.position);
        
        // Check if should start charging
        if (distanceToTarget <= attackRange && currentMove == FatMove.Moving) {
            currentMove = FatMove.Charging;
            dashDirection = (playerTransform.position - enemyTransform.position).normalized;
        }

        switch (currentMove) {
            case FatMove.Moving:
                base.moveEnemy(attackRange, moveSpeed);
                break;
            case FatMove.Charging:
                // Keep tracking player during charge
                dashDirection = (playerTransform.position - enemyTransform.position).normalized;
                chargeTimer += Time.deltaTime;
                if (chargeTimer >= chargeUpTime) {
                    currentMove = FatMove.Dashing;
                    dashStartPosition = enemyTransform.position;
                    chargeTimer = 0f;
                }
                break;
            case FatMove.Dashing:
                transform.Translate(dashDirection * dashSpeed * Time.deltaTime);
                
                // Check if dashed far enough
                float distanceDashed = Vector3.Distance(dashStartPosition, enemyTransform.position);
                if (distanceDashed >= maxDashDistance) {
                    currentMove = FatMove.Tired;
                    tiredTimer = 0f;
                }
                break;
            case FatMove.Tired:
                // Stand still and rest
                tiredTimer += Time.deltaTime;
                if (tiredTimer >= tiredDuration) {
                    currentMove = FatMove.Moving;
                    tiredTimer = 0f;
                }
                break;
        }
    }



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    
    // FatClown has its own Update to handle charge/dash behavior
    void Update()
    {
        // Always call moveEnemy - FatClown handles its own states internally
        moveEnemy(attackRange, moveSpeed);
    }


    void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log($"FatClown collided with: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        
        if (collision.gameObject.CompareTag("Player")) {
            // Deal damage to player
            PlayerMove player = collision.gameObject.GetComponent<PlayerMove>();
            if (player != null) {
                Debug.Log($"Dealing {damage} damage to player");
                player.TakeDamage(damage);
            } else {
                Debug.Log("PlayerMove component not found!");
            }
            
            // Stop dashing and get tired
            currentMove = FatMove.Tired;
            tiredTimer = 0f;
            chargeTimer = 0f;
        }
    }
    
    // In case the player's collider is a trigger
    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log($"FatClown triggered by: {other.gameObject.name}, Tag: {other.gameObject.tag}");
        
        if (other.gameObject.CompareTag("Player")) {
            PlayerMove player = other.gameObject.GetComponent<PlayerMove>();
            if (player != null) {
                Debug.Log($"Dealing {damage} damage to player (trigger)");
                player.TakeDamage(damage);
            }
            
            currentMove = FatMove.Tired;
            tiredTimer = 0f;
            chargeTimer = 0f;
        }
    }
}
