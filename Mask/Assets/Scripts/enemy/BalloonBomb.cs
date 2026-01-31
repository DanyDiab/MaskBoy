using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonBomb : MonoBehaviour
{
    enum BombState {
        Flying,
        Mine
    }
    
    [SerializeField] float flySpeed = 8f;
    [SerializeField] float damage = 20f;
    [SerializeField] float mineLifetime = 3f;
    [SerializeField] float explosionRadius = 1f;
    
    [Header("Sprites")]
    [SerializeField] Sprite flyingSprite;
    [SerializeField] Sprite mineSprite;
    
    SpriteRenderer spriteRenderer;
    BombState currentState = BombState.Flying;
    Vector3 targetPosition;
    Vector3 flyDirection;
    float mineTimer = 0f;
    bool hasExploded = false;
    GameObject shooter;
    
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Set initial flying sprite
        if (spriteRenderer != null && flyingSprite != null) {
            spriteRenderer.sprite = flyingSprite;
        }
    }
    
    public void Initialize(Vector3 target, float bombDamage, GameObject thrower) {
        targetPosition = target;
        damage = bombDamage;
        flyDirection = (targetPosition - transform.position).normalized;
        shooter = thrower;
    }
    
    void Update() {
        if (hasExploded) return;
        
        switch (currentState) {
            case BombState.Flying:
                // Move toward target
                transform.Translate(flyDirection * flySpeed * Time.deltaTime);
                
                // Check if reached target area
                float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
                if (distanceToTarget < 0.5f) {
                    BecomeMineLand();
                }
                break;
                
            case BombState.Mine:
                // Count down mine timer
                mineTimer += Time.deltaTime;
                if (mineTimer >= mineLifetime) {
                    // Disappear without exploding
                    Destroy(gameObject);
                }
                break;
        }
    }
    
    void BecomeMineLand() {
        currentState = BombState.Mine;
        mineTimer = 0f;
        
        // Change to mine sprite
        if (spriteRenderer != null && mineSprite != null) {
            spriteRenderer.sprite = mineSprite;
        }
    }
    
    void Explode() {
        if (hasExploded) return;
        hasExploded = true;
        
        // Check for player in explosion radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits) {
            if (hit.CompareTag("Player")) {
                PlayerMove player = hit.GetComponent<PlayerMove>();
                if (player != null) {
                    player.TakeDamage(damage);
                }
            }
        }
        
        // TODO: Add explosion visual/sound effect here
        Debug.Log("Balloon Bomb Exploded!");
        Destroy(gameObject);
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        if (hasExploded) return;
        
        // Ignore shooter and all enemies
        if (other.gameObject == shooter) return;
        if (other.GetComponent<Enemy>() != null) return;
        
        // Only react to Player
        if (other.CompareTag("Player")) {
            Explode();
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision) {
        if (hasExploded) return;
        
        // Ignore shooter and all enemies
        if (collision.gameObject == shooter) return;
        if (collision.gameObject.GetComponent<Enemy>() != null) return;
        
        // If hit player, explode immediately
        if (collision.gameObject.CompareTag("Player")) {
            Explode();
        }
        // If flying and hit something else (ground, wall), become mine
        else if (currentState == BombState.Flying) {
            BecomeMineLand();
        }
    }
}
