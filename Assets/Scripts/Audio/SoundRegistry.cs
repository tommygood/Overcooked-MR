using UnityEngine;
using System.Collections.Generic;
using System.Linq; // For Linq operations like ToDictionary

// This attribute allows you to create instances of this ScriptableObject
// directly from the Unity Editor's Create menu.
[CreateAssetMenu(fileName = "NewSoundRegistry", menuName = "Audio/Sound Registry")]
public class SoundRegistry : ScriptableObject
{
    [System.Serializable]
    public enum SoundID
    {
        None,
        Chop,
        Frying,
        Pour,
        CarCrash,
        Light, // 點火
        Burning,
        BowlCleaned,
        Boil,
        Ding,
        Bgm,
        GameOver,
    }

    // A simple struct to hold the mapping for the Inspector
    [System.Serializable]
    public struct SoundEntry
    {
        public SoundID ID;
        public SoundEvent Event;
    }

    [Tooltip("List of all sound events in your game, mapped to their IDs.")]
    public List<SoundEntry> SoundEntries = new List<SoundEntry>();

    private Dictionary<SoundID, SoundEvent> _soundMap;

    // Called when the ScriptableObject is loaded or modified in the editor
    void OnEnable()
    {
        InitializeMap();
    }

    public void InitializeMap()
    {
        if (_soundMap == null)
        {
            _soundMap = new Dictionary<SoundID, SoundEvent>();
        }
        else
        {
            _soundMap.Clear(); // Clear existing map if re-initializing
        }

        foreach (var entry in SoundEntries)
        {
            if (entry.Event != null)
            {
                if (!_soundMap.ContainsKey(entry.ID))
                {
                    _soundMap.Add(entry.ID, entry.Event);
                }
                else
                {
                    Debug.LogWarning($"SoundRegistry: Duplicate SoundID '{entry.ID}' found. Skipping the second entry.", this);
                }
            }
            else
            {
                Debug.LogWarning($"SoundRegistry: SoundEntry for ID '{entry.ID}' has no SoundEvent assigned!", this);
            }
        }
        Debug.Log($"SoundRegistry initialized with {_soundMap.Count} entries.");
    }

    // Public method to retrieve a SoundEvent by its ID
    public SoundEvent GetSoundEvent(SoundID id)
    {
        if (_soundMap == null || _soundMap.Count == 0)
        {
            InitializeMap(); // Attempt to initialize if not already
            if (_soundMap == null || _soundMap.Count == 0)
            {
                Debug.LogError("SoundRegistry: Map is not initialized or empty!");
                return null;
            }
        }

        if (_soundMap.TryGetValue(id, out SoundEvent soundEvent))
        {
            return soundEvent;
        }

        Debug.LogWarning($"SoundRegistry: No SoundEvent found for ID '{id}'!", this);
        return null;
    }
}