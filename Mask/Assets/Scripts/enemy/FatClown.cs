using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatClown : Enemy
{

    enum FatMove {
        Moving,
        Charging,
        Dashing
    }

    [SerializeField] float dashSpeed = 15f;   
    [SerializeField] float chargeUpTime = 0.5f;   

    Vector3 dashDirection;
    float chargeTimer = 0f;
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
                }
                break;
            case FatMove.Dashing:
                transform.Translate(dashDirection * dashSpeed * Time.deltaTime);
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
        if (collision.gameObject.CompareTag("Player")) {
            // collision.gameObject.GetComponent<>().TakeDamage(damage);
            currentMove = FatMove.Moving;
            chargeTimer = 0f;
        }
    }
}
