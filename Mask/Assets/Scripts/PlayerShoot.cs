using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Player Proj Properties")]

    [SerializeField] float fireRate;
    [SerializeField] float damage;
    [SerializeField] float projSpeed;

    [Header("Projectile")]
    [SerializeField] GameObject projectile;
    Transform playerTransform;
    float timeBetweenShots;
    float lastShotTime;
    void Start()
    {
        playerTransform = transform;
        lastShotTime = 0;
    }

    void Update(){
        timeBetweenShots = fireRate / 60;
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
        projScript.init(playerTransform.gameObject,dir,projSpeed,damage);
    }
}
