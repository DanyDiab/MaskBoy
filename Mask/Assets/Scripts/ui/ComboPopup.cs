using UnityEngine;
using TMPro;
using System.Collections;

public class ComboPopup : MonoBehaviour
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

    IEnumerator AnimateRoutine()
    {
        float timer = 0f;
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Vector3 startPos = transform.position;

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
