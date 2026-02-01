using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Sprite cursorSprite;
    [SerializeField] private Vector2 hotSpot = new Vector2(0.5f, 0.5f); // Pivot: (0.5, 0.5) is center, (0, 1) is top-left
    [SerializeField] private int sortingOrder = 30000; // Very high number to sit on top of all UI
    
    private GameObject cursorCanvasObj;
    private RectTransform cursorRect;
    private Image cursorImage;

    void Start()
    {
        // Hide system cursor
        Cursor.visible = false;
        CreateCursorCanvas();
    }

    void CreateCursorCanvas()
    {
        if (cursorCanvasObj == null)
        {
            // Create a dedicated Canvas for the cursor
            cursorCanvasObj = new GameObject("CursorCanvas");
            
            // Make it a child of this manager to keep hierarchy clean
            cursorCanvasObj.transform.SetParent(transform);

            // Configure Canvas for Overlay (draws on top of everything)
            Canvas canvas = cursorCanvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder; 

            // Add CanvasScaler to ensure consistent sizing if needed, 
            // but for a raw cursor, pixel-perfect is often preferred. 
            // We'll leave it raw for now to match input 1:1.

            // Create the Cursor Image
            GameObject imageObj = new GameObject("CursorImage");
            imageObj.transform.SetParent(cursorCanvasObj.transform);
            
            cursorImage = imageObj.AddComponent<Image>();
            cursorImage.sprite = cursorSprite;
            cursorImage.raycastTarget = false; // Important: Don't block clicks!
            cursorImage.SetNativeSize(); // Size it to the sprite's actual pixel size
            
            cursorRect = imageObj.GetComponent<RectTransform>();
            cursorRect.pivot = hotSpot; 
            cursorRect.localScale = new Vector3(0.25f, 0.25f, 1f); // Scale down to 25% size
        }
    }

    void Update()
    {
        // Enforce hidden system cursor
        if (Cursor.visible) Cursor.visible = false;
        UpdateCursorPosition();
    }

    void UpdateCursorPosition()
    {
        if (cursorRect != null)
        {
            // In Overlay mode, setting position to Input.mousePosition works perfectly
            cursorRect.position = Input.mousePosition;
        }
    }

    void OnDisable()
    {
        Cursor.visible = true;
    }
}