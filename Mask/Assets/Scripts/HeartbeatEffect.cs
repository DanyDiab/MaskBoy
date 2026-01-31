using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HeartbeatEffect : MonoBehaviour
{
    [Header("References")]
    PlayerHealth playerHealth;

    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] float startHealthPercentage = 0.4f;
    [SerializeField] float minPitch = 1.0f;
    [SerializeField] float maxPitch = 2.0f;
    [SerializeField] float maxVolume = 1.0f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (AudioManager.Instance != null) {
            audioSource.clip = AudioManager.Instance.GetClip(SoundType.Heartbeat);
        }
        
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;

        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth == null) return;

        float current = playerHealth.CurrentHealth;
        float max = playerHealth.MaxHealth;
        
        // Safety check
        if (max <= 0) return;

        float percentage = Mathf.Clamp01(current / max);

        if (percentage < startHealthPercentage && percentage > 0)
        {
            // Normalize percentage from (start...0) to (0...1) where 1 is DEAD/LOWEST
            // start=0.4, current=0.2 => factor = (0.4 - 0.2) / 0.4 = 0.5
            float panicFactor = (startHealthPercentage - percentage) / startHealthPercentage;

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            // Interpolate pitch and volume
            audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, panicFactor);
            audioSource.volume = Mathf.Lerp(0f, maxVolume, panicFactor);
        }
        else
        {
            // Fade out or stop
            if (audioSource.isPlaying)
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime * 5f);
                if (audioSource.volume < 0.01f)
                {
                    audioSource.Stop();
                }
            }
        }
    }
}
