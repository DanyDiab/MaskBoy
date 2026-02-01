using UnityEngine;

public class UpgradeShower : MonoBehaviour
{
    [SerializeField] GameObject popupPrefab; // can reuse your DamageText prefab
    [SerializeField] Canvas canvas;
    [SerializeField] Vector3 worldOffset = new Vector3(0f, 1.5f, 0f);

    void Awake()
    {
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        if (popupPrefab == null)
        {
            Debug.LogError("UpgradeShower: No popupPrefab assigned (use your DamageText prefab).");
        }
    }

    void OnEnable()
    {
        UpgradeApplier.OnUpgradePopup += ShowUpgrade;
    }

    void OnDisable()
    {
        UpgradeApplier.OnUpgradePopup -= ShowUpgrade;
    }

    void ShowUpgrade(string text, Color color, Vector3 worldPos)
    {
        if (popupPrefab == null || canvas == null) return;

        GameObject popup = Instantiate(popupPrefab, canvas.transform);
        if (Camera.main != null)
        {
            popup.transform.position = Camera.main.WorldToScreenPoint(worldPos + worldOffset);
        }

        DamageText damageText = popup.GetComponent<DamageText>();
        if (damageText != null)
        {
            damageText.SetColor(color);
            damageText.Setup(text);
        }
    }
}

