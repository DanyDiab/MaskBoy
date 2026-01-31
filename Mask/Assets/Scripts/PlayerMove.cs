using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
    
{
    [Header("move speed")]
    [SerializeField] float moveSpeed = 5f;
    Transform playerTransform;
    void Start(){
        playerTransform = transform;
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

        return dir;
    }

    void movePlayer(){
        Vector3 dir = getMoveDir();
        transform.Translate(dir * moveSpeed * Time.deltaTime);

    }


    void Update(){
        movePlayer();
    }
}
