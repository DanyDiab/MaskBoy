using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Player Proj Properties")]
    PlayerStats playerStats;

    [Header("Projectile")]
    [SerializeField] GameObject projectile;
    Transform playerTransform;
    float timeBetweenShots;
    float lastShotTime;
    void Start()
    {
        playerTransform = transform;
        playerStats = GetComponent<PlayerStats>();
        lastShotTime = 0;
    }

    void Update(){
        float fireRate = playerStats.CurrentFireRate;
        timeBetweenShots = fireRate > 0 ? 1f / fireRate : 999f;
        
        float currTime = Time.time;
        if(Input.GetMouseButton((int)MouseButton.Left) && currTime - lastShotTime > timeBetweenShots){
            shoot(Input.mousePosition);
            lastShotTime = currTime;
        }
    }

    void shoot(Vector3 mousePos){
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
        mousePosWorld.z = 0f;
        Vector3 playerPos = transform.position;
        playerPos.z = 0f;
        Vector3 dir = (mousePosWorld - playerPos).normalized;
        GameObject projInstance = Instantiate(projectile);
        Projectile projScript = projInstance.GetComponent<Projectile>();
        projScript.init(playerTransform.gameObject,dir,playerStats.CurrentProjectileSpeed,playerStats.CurrentDamage);
        AudioManager.Play(SoundType.Shoot);
    }
}
