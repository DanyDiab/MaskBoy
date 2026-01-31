using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    GameObject shooter;
    Vector3 shootDir;
    float speed;
    float damage;
    void Start(){

    }


    void Update(){
        Vector3 move = Vector3.up;
        move *= speed;
        move.z = 0;
        transform.Translate(move * Time.deltaTime);
    }

    public void init(GameObject shooter, Vector3 shootDir, float speed, float damage){
        Vector3 shootPos = shooter.transform.position;

        transform.position = shooter.transform.position;
        this.shooter = shooter;
        this.shootDir = shootDir;
        this.speed = speed;
        this.damage = damage;

        float rot = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    void OnCollisionEnter2D(Collision2D collision){
        GameObject collidedWith = collision.collider.gameObject;
        Debug.Log(collidedWith);
        Destroy(gameObject);
    }
}
