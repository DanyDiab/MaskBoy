using UnityEngine;
using System.Collections;

public class DamageShower : MonoBehaviour
{
    [SerializeField] GameObject damagePopupPrefab;
    [SerializeField] Canvas canvas;

    void Awake()
    {
        Debug.Log("DamageShower: Awake");
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }
        
        if (damagePopupPrefab == null)
        {
            Debug.LogError("DamageShower: No Damage Popup Prefab assigned!");
        }
    }

    void OnEnable()
    {
        Debug.Log("DamageShower: OnEnable - Subscribing to OnPlayerDamage");
        PlayerHealth.OnPlayerDamage += ShowDamage;
    }

    void OnDisable()
    {
        PlayerHealth.OnPlayerDamage -= ShowDamage;
    }

    void ShowDamage(float damage, Vector3 worldPos)
    {
        Debug.Log($"DamageShower: ShowDamage called. Damage: {damage}, Pos: {worldPos}");
        if (damagePopupPrefab == null || canvas == null) 
        {
            Debug.LogError("DamageShower: Missing Prefab or Canvas!");
            return;
        }

        // Instantiate as child of the canvas so it renders in UI
        GameObject popup = Instantiate(damagePopupPrefab, canvas.transform);
        
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos + Vector3.up * 1.5f);
        popup.transform.position = screenPos;

        DamageText damageText = popup.GetComponent<DamageText>();
        if (damageText != null)
        {
            if (damage < 0)
            {
                damageText.SetColor(Color.green);
            }
            else
            {
                damageText.SetColor(Color.red);
            }

            damageText.Setup(Mathf.RoundToInt(Mathf.Abs(damage)).ToString());
        }
    }
}
