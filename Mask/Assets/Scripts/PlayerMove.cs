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
    
    void Start(){
        playerTransform = transform;
        playerStats = GetComponent<PlayerStats>();
        playerHealth = GetComponent<PlayerHealth>();
    }
    
    public void TakeDamage(float damage) {
        // Keep this method because enemies call PlayerMove.TakeDamage()
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
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
        float actualMoveSpeed = playerStats != null ? playerStats.CurrentMoveSpeed : 5f;
        transform.Translate(dir * actualMoveSpeed * Time.deltaTime);

    }


    void Update(){
        movePlayer();
    }
}
