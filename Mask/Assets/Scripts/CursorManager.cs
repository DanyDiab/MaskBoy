using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Sprite cursorSprite;
    [SerializeField] private int sortingOrder = 1000;
    
    private GameObject cursorObj;
    private SpriteRenderer cursorRenderer;

    void Start()
    {
        // Hide system cursor
        Cursor.visible = false;

        // Create the cursor visual
        CreateCursorObject();
    }

    void CreateCursorObject()
    {
        if (cursorObj == null)
        {
            cursorObj = new GameObject("CustomCursor");
            cursorRenderer = cursorObj.AddComponent<SpriteRenderer>();
            cursorRenderer.sprite = cursorSprite;
            cursorRenderer.sortingOrder = sortingOrder;
            cursorRenderer.sortingLayerName = "Player";
            
            // Ensure it doesn't get destroyed if you want it to persist, 
            // but this script itself must persist then. 
            // For now, we'll keep it local to the scene or assume the Manager persists.
            
            // Parent it to this manager to keep hierarchy clean
            cursorObj.transform.SetParent(transform);
        }
    }

    void Update()
    {
        // Enforce hidden cursor
        if (Cursor.visible) Cursor.visible = false;

        UpdateCursorPosition();
    }

    void UpdateCursorPosition()
    {
        if (cursorObj != null && Camera.main != null)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0f; // Keep it on the 2D plane
            cursorObj.transform.position = worldPos;
        }
    }

    void OnDisable()
    {
        Cursor.visible = true;
    }
}
