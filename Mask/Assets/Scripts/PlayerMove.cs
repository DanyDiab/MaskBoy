using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
    
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    Transform playerTransform;
    [SerializeField] ParticleSystem moveParticles;
    
    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    float currentHealth;
    
    void Start(){
        playerTransform = transform;
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damage) {
        currentHealth -= damage;        
        if (currentHealth <= 0) {
            Die();
        }
    }
    
    void Die() {
        Debug.Log("Player died!");
        // Add death logic here (respawn, game over, etc.)
    }

    Vector3 getMoveDir(){
        Vector3 dir = Vector3.zero;
        if(Input.GetKey(KeyCode.W)){
            dir.y += 1;
        }
        if(Input.GetKey(KeyCode.S)){
            dir.y -=1;
        }
        if(Input.GetKey(KeyCode.A)){
            dir.x -= 1;
        }
        if(Input.GetKey(KeyCode.D)){
            dir.x += 1;
        }

        return dir.normalized;
    }

    void enableParticles(Vector3 dir){
        bool isMoving = dir != Vector3.zero;
        if (isMoving) {
            if (!moveParticles.isPlaying) {
                moveParticles.Play();
            }
        } else {
            moveParticles.Stop();
        }
    }

    void movePlayer(){
        Vector3 dir = getMoveDir();

        enableParticles(dir);
        transform.Translate(dir * moveSpeed * Time.deltaTime);

    }


    void Update(){
        movePlayer();
    }
}
