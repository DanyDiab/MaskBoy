using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health = 100f;
    [SerializeField] protected float moveSpeed = 5f;
    protected Transform enemyTransform;


    protected virtual void moveEnemy() {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
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
        
    }
}
