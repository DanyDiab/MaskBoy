using UnityEngine;
using System.Collections;

public class DamageShower : MonoBehaviour
{
    [SerializeField] GameObject damagePopupPrefab;
    [SerializeField] Canvas canvas;
    [SerializeField] bool showCurrentHpInsteadOfDelta = true;
    PlayerHealth playerHealth;

    void Awake()
    {
        Debug.Log("DamageShower: Awake");
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        if (playerHealth == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerHealth = player.GetComponent<PlayerHealth>();
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

            if (showCurrentHpInsteadOfDelta && playerHealth != null)
            {
                string hpText = $"{Mathf.CeilToInt(playerHealth.CurrentHealth)}/{Mathf.CeilToInt(playerHealth.MaxHealth)}";
                damageText.Setup(hpText);
            }
            else
            {
                damageText.Setup(Mathf.RoundToInt(Mathf.Abs(damage)).ToString());
            }
        }
    }
}
