using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Lightning : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float minInterval = 5f;
    [SerializeField] float maxInterval = 15f;
    [SerializeField] float flashDuration = 0.2f;
    [SerializeField] float maxAlpha = 0.8f;

    [Header("References")]
    [SerializeField] Image flashPanel;

    void Start()
    {
        if (flashPanel != null)
        {
            // Ensure color starts at 0 alpha
            Color c = flashPanel.color;
            c.a = 0f;
            flashPanel.color = c;
            
            StartCoroutine(LightningRoutine());
        }
        else
        {
            Debug.LogError("Lightning: Flash Panel (Image) is not assigned!");
        }
    }

    IEnumerator LightningRoutine()
    {
        while (true)
        {
            // Wait for random interval
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // Trigger Lightning
            yield return StartCoroutine(FlashEffect());
        }
    }

    IEnumerator FlashEffect()
    {
        // Play Sound
        AudioManager.Play(SoundType.Lightning);

        Color c = flashPanel.color;
        float halfDuration = flashDuration * 0.5f;
        float timer = 0f;

        // Flash In
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0f, maxAlpha, timer / halfDuration);
            flashPanel.color = c;
            yield return null;
        }

        // Flash Out
        timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(maxAlpha, 0f, timer / halfDuration);
            flashPanel.color = c;
            yield return null;
        }

        c.a = 0f;
        flashPanel.color = c;
    }
}
