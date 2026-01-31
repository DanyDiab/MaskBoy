using UnityEngine;

public class MoveBitch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int moveSpeed = 5;
    Transform transform;
    void Start()
    {
        transform = GetComponent<Transform>();
    }


    Vector3 getMoveDir()
    {
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) {
            dir.y += 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            dir.y -= 1;
        }
        if (Input.GetKey(KeyCode.A)) {
            dir.x -= 1;
        }
        if (Input.GetKey(KeyCode.D)) {
            dir.x += 1;
        }
        return dir;
    }

    void move()
    {
        Vector3 moveDir = getMoveDir();
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }
}
