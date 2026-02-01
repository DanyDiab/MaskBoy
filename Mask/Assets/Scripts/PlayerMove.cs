using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    Transform playerTransform;
    [SerializeField] ParticleSystem moveParticles;
    PlayerStats playerStats;
    PlayerHealth playerHealth;
    
    // Direction Blocking Flags
    bool blockUp;
    bool blockDown;
    bool blockLeft;
    bool blockRight;

    void Start(){
        playerTransform = transform;
        playerStats = GetComponent<PlayerStats>();
        playerHealth = GetComponent<PlayerHealth>();
    }
    
    public void TakeDamage(float damage) {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    Vector3 getMoveDir(){
        Vector3 dir = Vector3.zero;
        
        // Check input and blocking flags
        if(Input.GetKey(KeyCode.W) && !blockUp){
            dir.y += 1;
        }
        if(Input.GetKey(KeyCode.S) && !blockDown){
            dir.y -=1;
        }
        if(Input.GetKey(KeyCode.A) && !blockLeft){
            dir.x -= 1;
        }
        if(Input.GetKey(KeyCode.D) && !blockRight){
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
        float actualMoveSpeed = playerStats != null ? playerStats.CurrentMoveSpeed : 5f;
        
        // Using Translate as requested
        transform.Translate(dir * actualMoveSpeed * Time.deltaTime);
    }


    void Update(){
        movePlayer();
    }


    void OnCollisionEnter2D(Collision2D collision) {
        UpdateWallState(collision.gameObject.tag, true);
    }

    void OnCollisionExit2D(Collision2D collision) {
        UpdateWallState(collision.gameObject.tag, false);
    }

    void OnTriggerEnter2D(Collider2D other) {
        UpdateWallState(other.tag, true);
    }

    void OnTriggerExit2D(Collider2D other) {
        UpdateWallState(other.tag, false);
    }

    void UpdateWallState(string tag, bool isBlocked)
    {
        switch (tag)
        {
            case "TopWall":
                blockUp = isBlocked;
                break;
            case "BotWall":
                blockDown = isBlocked;
                break;
            case "LeftWall":
                blockLeft = isBlocked;
                break;
            case "RIghtWall":
                blockRight = isBlocked;
                break;
        }
    }
}