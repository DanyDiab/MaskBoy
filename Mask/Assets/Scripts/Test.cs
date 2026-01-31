using UnityEngine;

public class Test : MonoBehaviour
{
    Transform transform;
    int x;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        transform = GetComponent<Transform>();
        x = 0;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
