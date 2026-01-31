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
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    public void init(GameObject shooter, Vector3 shootDir, float speed, float damage){
        Vector3 shootPos = shooter.transform.position;
        Debug.LogFormat("ShootDir | {0}\nshooter | {1}", shootDir,shootPos);

        transform.position = shooter.transform.position;
        this.shooter = shooter;
        this.shootDir = shootDir;
        this.speed = speed;
        this.damage = damage;

        transform.rotation = Quaternion.FromToRotation(Vector3.up, shootDir);
    }

    void OnCollisionEnter2D(Collision2D collision){
        GameObject collidedWith = collision.collider.gameObject;
        Debug.Log(collidedWith);
        Destroy(gameObject);
    }
}
