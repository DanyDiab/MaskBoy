using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float comboWindow = 1.0f; // Time window to chain kills
    [SerializeField] int minComboToShow = 3;
    [SerializeField] float maxRotationAngle = 20f;
    [SerializeField] float rotationSmoothTime = 0.2f;

    [Header("UI Reference")]
    [SerializeField] TextMeshProUGUI comboText;
    [SerializeField] GameObject popupPrefab;
    [SerializeField] float padding = 50f;
    [SerializeField] bool debugMode = true;
    
    // State
    int currentCombo = 0;
    float lastKillTime;
    float rotationVelocity; // For SmoothDamp
    RectTransform rectTransform;
    RectTransform canvasRect;

    void Awake()
    {
        if (comboText == null) comboText = GetComponent<TextMeshProUGUI>();

        if (comboText != null)
        {
            rectTransform = comboText.rectTransform;
            comboText.gameObject.SetActive(false);
            
            // Find the parent canvas rect for dynamic sizing
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null) canvasRect = canvas.GetComponent<RectTransform>();
        }
        if(popupPrefab != null){

            popupPrefab.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("ComboManager: No Combo Text assigned or found on GameObject!");
        }
    }

    void Start()
    {
        
        if (rectTransform != null && canvasRect != null)
        {
            // Position in Top Left dynamically
            // Assumes anchors are at Center (0.5, 0.5)
            float width = canvasRect.rect.width;
            float height = canvasRect.rect.height;
            
            Vector2 topLeftPos = new Vector2(-width / 2f + padding, height / 2f - padding);
            rectTransform.anchoredPosition = topLeftPos;
            rectTransform.localScale = Vector3.one;
        }
    }

    void OnEnable()
    {
        Enemy.OnEnemyDeath += OnEnemyDeath;
    }

    void OnDisable()
    {
        Enemy.OnEnemyDeath -= OnEnemyDeath;
    }

    void OnEnemyDeath(Vector3 pos)
    {
        float time = Time.time;
        if (time - lastKillTime <= comboWindow)
        {
            currentCombo++;
        }
        else
        {
            currentCombo = 1;
        }
        lastKillTime = time;

        Debug.Log($"Combo: {currentCombo}");

        if (currentCombo >= minComboToShow)
        {
            ShowCombo(currentCombo, pos);
        }
        else
        {
            if (comboText != null) comboText.gameObject.SetActive(false);
        }
    }

    void ShowCombo(int combo, Vector3 worldPos)
    {
        if (comboText == null)
        {
            return;
        }
        

        // 1. Update Main Static Text (Top Left)
        comboText.gameObject.SetActive(true);
        comboText.text = combo + "X";

        // Random rotation punch for main text
        float randomAngle = Random.Range(-maxRotationAngle, maxRotationAngle);
        comboText.transform.localRotation = Quaternion.Euler(0, 0, randomAngle);
        rotationVelocity = 0f;

        // 2. Spawn Floating Popup (Per Enemy)
        if (popupPrefab != null)
        {
            // Use the canvas of the main text as parent
            GameObject popup = Instantiate(popupPrefab, comboText.transform.parent);
            popup.SetActive(true); // Ensure it's visible even if the source was hidden

            if (Camera.main != null)
            {
                popup.transform.position = Camera.main.WorldToScreenPoint(worldPos);
            }
            
            ComboPopup popupScript = popup.GetComponent<ComboPopup>();
            if (popupScript == null) popupScript = popup.AddComponent<ComboPopup>();
            
            popupScript.Setup(combo + "X");
        }
    }

    void Update()
    {
        // Check for combo expiry to hide text
        if (Time.time - lastKillTime > comboWindow)
        {
            currentCombo = 0;
            if (comboText != null && comboText.gameObject.activeSelf)
            {
                comboText.gameObject.SetActive(false);
            }
        }

        // Animate rotation back to 0 if active
        if (comboText != null && comboText.gameObject.activeSelf)
        {
            float currentZ = comboText.transform.localEulerAngles.z;
            // Unity Euler angles are 0-360, need to map to -180 to 180 for correct smoothing
            if (currentZ > 180) currentZ -= 360;

            float newZ = Mathf.SmoothDamp(currentZ, 0f, ref rotationVelocity, rotationSmoothTime);
            comboText.transform.localRotation = Quaternion.Euler(0, 0, newZ);
        }
    }
}