using UnityEngine;
using System.Collections.Generic; // For List

// This attribute allows you to create instances of this ScriptableObject
// directly from the Unity Editor's Create menu.
[CreateAssetMenu(fileName = "NewSoundEvent", menuName = "Audio/Sound Event")]
public class SoundEvent : ScriptableObject
{
    [Tooltip("The main audio clip(s) for this event. One will be chosen randomly.")]
    public List<AudioClip> AudioClips = new List<AudioClip>(); // Initialize to avoid null

    [Range(0f, 1f)]
    [Tooltip("Base volume for the sound event.")]
    public float Volume = 1f;

    [Tooltip("Random volume variation (e.g., 0.1 for +/- 10% from base volume).")]
    [Range(0f, 0.5f)] // Increased range for more noticeable variation if desired
    public float VolumeVariation = 0f;

    [Range(-3f, 3f)]
    [Tooltip("Base pitch for the sound event.")]
    public float Pitch = 1f;

    [Tooltip("Random pitch variation (e.g., 0.1 for +/- 10% from base pitch).")]
    [Range(0f, 0.5f)] // Increased range for more noticeable variation if desired
    public float PitchVariation = 0f;

    [Tooltip("0 = 2D (non-spatialized), 1 = 3D (spatialized).")]
    [Range(0f, 1f)]
    public float SpatialBlend = 1f; // Default to 3D for in-game SFX

    [Tooltip("If true, the sound will loop continuously until explicitly stopped.")]
    public bool Loop = false;

    [Header("Distance Falloff (for 3D sounds)")]
    [Tooltip("Distance at which the sound starts to fade.")]
    public float MinDistance = 1f;
    [Tooltip("Distance at which the sound fully fades out.")]
    public float MaxDistance = 50f;


    // --- Helper Methods (for internal use by SoundManager) ---

    // Returns a randomly selected AudioClip from the list.
    public AudioClip GetRandomClip()
    {
        if (AudioClips == null || AudioClips.Count == 0)
        {
            Debug.LogWarning($"SoundEvent '{name}' has no AudioClips assigned! Returning null.", this);
            return null;
        }
        return AudioClips[Random.Range(0, AudioClips.Count)];
    }

    // Returns the base volume with random variation applied.
    public float GetRandomVolume()
    {
        return Volume + Random.Range(-VolumeVariation, VolumeVariation);
    }

    // Returns the base pitch with random variation applied.
    public float GetRandomPitch()
    {
        return Pitch + Random.Range(-PitchVariation, PitchVariation);
    }
}