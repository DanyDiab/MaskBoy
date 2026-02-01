using UnityEngine;

public class CheckPanelVisibility : MonoBehaviour
{
    public RectTransform targetPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (targetPanel != null)
            {
                Debug.Log($"[Visibility Check] Panel Active: {targetPanel.gameObject.activeInHierarchy}");
                Debug.Log($"[Visibility Check] CanvasGroup Alpha: {targetPanel.GetComponent<CanvasGroup>()?.alpha}");
                Debug.Log($"[Visibility Check] World Position: {targetPanel.position}");
                Debug.Log($"[Visibility Check] Anchored Position: {targetPanel.anchoredPosition}");
                Debug.Log($"[Visibility Check] Local Scale: {targetPanel.localScale}");
                
                // Check if it's within screen bounds
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, targetPanel.position);
                Debug.Log($"[Visibility Check] Screen Position (Center): {screenPos} (Screen Size: {Screen.width}x{Screen.height})");
            }
        }
    }
}
