using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] float duration = 1.0f;
    [SerializeField] float displayTime = 1.5f;

    [Header("Animation Settings")]
    [SerializeField] float centerFontSize = 120f;
    [SerializeField] float cornerFontSize = 40f;
    [SerializeField] float padding = 50f;

    RectTransform rectTransform;
    RectTransform canvasRect;

    void Awake()
    {
        if (waveText == null) waveText = GetComponent<TextMeshProUGUI>();
        rectTransform = waveText.rectTransform;
        
        // Find the parent canvas rect for dynamic sizing
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }
    }

    void OnEnable()
    {
        EnemySpawner.OnWaveStart += OnWaveStart;
    }

    void OnDisable()
    {
        EnemySpawner.OnWaveStart -= OnWaveStart;
    }

    void OnWaveStart(int wave)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateWaveRoutine(wave));
    }

    private static readonly string[] RomanNumerals = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
    private static readonly int[] Values = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

    string ToRoman(int number)
    {
        if (number <= 0) return "";
        
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        for (int i = 0; i < Values.Length; i++)
        {
            while (number >= Values[i])
            {
                number -= Values[i];
                result.Append(RomanNumerals[i]);
            }
        }
        return result.ToString();
    }

    IEnumerator AnimateWaveRoutine(int wave)
    {
        waveText.text = ToRoman(wave);
        
        // Ensure we have the canvas rect
        if (canvasRect == null)
        {
             Canvas canvas = GetComponentInParent<Canvas>();
             if (canvas != null) canvasRect = canvas.GetComponent<RectTransform>();
        }
        Vector2 cornerPos = new Vector2(-200,-200);
        Vector2 centerPos = Vector2.zero;

        if (canvasRect != null)
        {
            // Calculate bottom-left based on canvas size
            // Assumes anchors are at Center (0.5, 0.5)
            float width = canvasRect.rect.width;
            float height = canvasRect.rect.height;
            
            // Bottom-left relative to center is (-width/2, -height/2)
            // Add padding to keep it on screen
            // Also consider the text's own size if needed, but padding usually covers it
            cornerPos = new Vector2(-width / 2f + padding, -height / 2f + padding);
        }

        rectTransform.anchoredPosition = centerPos;
        rectTransform.localScale = Vector3.one; // Reset scale
        waveText.fontSize = centerFontSize;
        
        yield return new WaitForSeconds(displayTime);

        float elapsed = 0f;
        Vector3 startPos = rectTransform.anchoredPosition;
        Vector3 endPos = cornerPos;
        float startSize = centerFontSize;
        float endSize = cornerFontSize;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Smooth step for nicer ease
            t = t * t * (3f - 2f * t);

            rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
            waveText.fontSize = Mathf.Lerp(startSize, endSize, t);

            yield return null;
        }

        rectTransform.anchoredPosition = endPos;
        waveText.fontSize = endSize;
    }
}
