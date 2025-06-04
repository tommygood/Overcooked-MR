using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField]
    private SoundRegistry soundRegistry;

    [Header("Volumes")]
    [Range(0f, 1f)] public float MasterVolume = 1f;
    [Range(0f, 1f)] public float MusicVolume = 1f;
    [Range(0f, 1f)] public float SfxVolume = 1f;
    [Range(0f, 1f)] public float UiSfxVolume = 1f;

    void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            this.soundRegistry.InitializeMap();
            DontDestroyOnLoad(gameObject); // Keep SoundManager alive across scenes
        }
    }

    // Call this for one-shot sound effects (e.g., footsteps, impacts)
    public void PlaySFX(SoundRegistry.SoundID soundID, Vector3 position)
    {
        SoundEvent soundEvent = soundRegistry.GetSoundEvent(soundID);
        if (soundEvent == null || soundEvent.AudioClips == null || soundEvent.AudioClips.Count == 0)
        {
            Debug.LogWarning($"SoundManager: Attempted to play null or empty SoundEvent. Skipping.");
            return;
        }
        if (soundEvent.Loop)
        {
            Debug.LogWarning($"SoundManager: SoundEvent '{soundEvent.name}' is marked as looping. Use PlayLoopingSFX instead.", soundEvent);
            return;
        }

        AudioSource audioSource = SFXPooler.Instance.GetPooledAudioSource();
        if (audioSource == null) return;

        BindSoundEvent(soundEvent, audioSource);
        audioSource.loop = false; // Ensure it's not looping

        // Set position for 3D sounds
        audioSource.transform.position = position;

        audioSource.Play();

        // Return to pool after it finishes playing
        SFXPooler.Instance.ReturnPooledAudioSource(audioSource, audioSource.clip.length);
    }

    // Call this for starting a looping sound effect (e.g., engine hum, running water)
    // Returns the AudioSource instance, which you'll need to stop it later.
    public AudioSource PlayLoopingSFX(SoundRegistry.SoundID soundID, Vector3 position)
    {
        SoundEvent soundEvent = soundRegistry.GetSoundEvent(soundID);
        if (soundEvent == null || soundEvent.AudioClips == null || soundEvent.AudioClips.Count == 0)
        {
            Debug.LogWarning($"SoundManager: Attempted to play null or empty SoundEvent. Skipping.");
            return null;
        }
        if (!soundEvent.Loop)
        {
            Debug.LogWarning($"SoundManager: SoundEvent '{soundEvent.name}' is not marked as looping. Use PlaySFX instead.", soundEvent);
            return null;
        }

        AudioSource audioSource = SFXPooler.Instance.GetPooledAudioSource();
        if (audioSource == null) return null;

        BindSoundEvent(soundEvent, audioSource);
        audioSource.loop = true; // Crucial: Set to loop

        // Set position for 3D sounds
        audioSource.transform.position = position;

        audioSource.Play();
        return audioSource; // Return the AudioSource instance so it can be stopped
    }

    private void BindSoundEvent(SoundEvent soundEvent, AudioSource audioSource)
    {
        // Apply properties from SoundEvent
        audioSource.clip = soundEvent.GetRandomClip();
        audioSource.volume = soundEvent.GetRandomVolume() * SfxVolume * MasterVolume; // Apply global SFX volume
        audioSource.pitch = soundEvent.GetRandomPitch();
        audioSource.spatialBlend = soundEvent.SpatialBlend;
        audioSource.minDistance = soundEvent.MinDistance;
        audioSource.maxDistance = soundEvent.MaxDistance;
    }

    // Call this to stop a previously started looping sound.
    public void StopLoopingSFX(AudioSource audioSource)
    {
        SFXPooler.Instance.ReturnPooledAudioSource(audioSource); // SFXPooler handles stopping and returning
    }
}