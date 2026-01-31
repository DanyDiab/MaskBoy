using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugglingBall : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float damage = 10f;
    [SerializeField] float lifetime = 5f; // Destroy after this time if still alive
    
    Vector3 moveDirection;
    GameObject shooter;
    float aliveTimer = 0f;
    
    public void Initialize(Vector3 direction, float ballDamage, GameObject thrower) {
        moveDirection = direction.normalized;
        damage = ballDamage;
        shooter = thrower;
    }
    
    void Update() {
        // Move in direction
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        
        // Destroy if alive too long (off-screen safety)
        aliveTimer += Time.deltaTime;
        if (aliveTimer >= lifetime) {
            Destroy(gameObject);
        }
        
        // Also check if way off screen
        if (IsOffScreen()) {
            Destroy(gameObject);
        }
    }
    
    bool IsOffScreen() {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        // If outside viewport with some padding
        return viewPos.x < -0.5f || viewPos.x > 1.5f || viewPos.y < -0.5f || viewPos.y > 1.5f;
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        // Ignore shooter and all enemies
        if (other.gameObject == shooter) return;
        if (other.GetComponent<Enemy>() != null) return;
        
        // Damage player if hit
        if (other.CompareTag("Player")) {
            PlayerMove player = other.GetComponent<PlayerMove>();
            if (player != null) {
                player.TakeDamage(damage);
            }
        }
        
        // Destroy on any non-enemy collision
        Destroy(gameObject);
    }
    
    void OnCollisionEnter2D(Collision2D collision) {
        // Ignore shooter and all enemies
        if (collision.gameObject == shooter) return;
        if (collision.gameObject.GetComponent<Enemy>() != null) return;
        
        // Damage player if hit
        if (collision.gameObject.CompareTag("Player")) {
            PlayerMove player = collision.gameObject.GetComponent<PlayerMove>();
            if (player != null) {
                player.TakeDamage(damage);
            }
        }
        
        // Destroy on any non-enemy collision
        Destroy(gameObject);
    }
}
