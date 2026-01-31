using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Shoot,
    Hit,
    PlayerDeath,
    EnemyDeath,
    Explosion,
    Heartbeat
}

[System.Serializable]
public class Sound
{
    public SoundType type;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private Sound[] sounds;
    private AudioSource audioSource;
    private Dictionary<SoundType, Sound> soundDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        soundDictionary = new Dictionary<SoundType, Sound>();

        foreach (Sound s in sounds)
        {
            if (!soundDictionary.ContainsKey(s.type))
            {
                soundDictionary.Add(s.type, s);
            }
        }
    }

    public static void Play(SoundType type)
    {
        if (Instance == null) return;
        Instance.PlaySound(type);
    }

    public AudioClip GetClip(SoundType type)
    {
        if (soundDictionary.TryGetValue(type, out Sound s))
        {
            return s.clip;
        }
        return null;
    }

    private void PlaySound(SoundType type)
    {
        if (soundDictionary.TryGetValue(type, out Sound s))
        {
            // Random pitch variation between 0.9 and 1.1
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(s.clip, s.volume);
        }
    }
}
