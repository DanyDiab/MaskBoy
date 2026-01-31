using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health = 100f;
    [SerializeField] protected float moveSpeed = 5f;
    

    protected static Vector3 targetPosition;
    protected static bool hasTarget = false;
    
    protected Transform enemyTransform;


    protected virtual void moveEnemy() {
        if (!hasTarget) return;

        float distanceToTarget = Vector3.Distance(enemyTransform.position, targetPosition);
        if (distanceToTarget < 1.0f) return;

        Vector3 direction = (targetPosition - enemyTransform.position).normalized;
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



    // Start is called before the first frame update
    void Start()
    {
        enemyTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            hasTarget = true;
            targetPosition = mousePosition;
        }

        moveEnemy();
    }
}
