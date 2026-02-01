using UnityEngine;
using TMPro;
using System.Collections;

public class DamageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] float moveSpeed = 50f;
    [SerializeField] float lifeTime = 1f;
    [SerializeField] float fadeSpeed = 2f;

    public void Setup(string text)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh != null) textMesh.text = text;
        
        StartCoroutine(AnimateRoutine());
    }

    public void SetColor(Color color)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh != null) textMesh.color = color;
    }

    IEnumerator AnimateRoutine()
    {
        float timer = 0f;
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Optional: Add some random horizontal drift like a "shower" or pop
        // But user asked for "same style", so sticking to vertical move for now.
        // If "shower" implies multiple or scattering, I might add randomness in the Manager or here.
        // Sticking to ComboPopup logic as requested.

        while (timer < lifeTime)
        {
            timer += Time.deltaTime;
            
            // Move up
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            // Fade out near end
            if (timer > lifeTime * 0.5f)
            {
                canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
