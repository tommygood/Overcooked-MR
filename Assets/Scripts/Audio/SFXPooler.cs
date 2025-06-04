using UnityEngine;
using System.Collections.Generic;
using System.Collections; // For Coroutines

public class SFXPooler : MonoBehaviour
{
    // Singleton instance for easy access from other scripts
    public static SFXPooler Instance { get; private set; }

    [Tooltip("Prefab containing just an AudioSource component. Will be instantiated for the pool.")]
    [SerializeField] private GameObject _audioSourcePrefab;

    [Tooltip("Initial number of AudioSources to create in the pool.")]
    [SerializeField] private int _poolSize = 10;

    private Queue<AudioSource> _availableSources = new Queue<AudioSource>();
    private List<AudioSource> _activeSources = new List<AudioSource>(); // To keep track of currently playing sources

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
            // Ensure this object persists across scene loads if your SoundManager does
            // DontDestroyOnLoad(gameObject);
        }

        PrepopulatePool();
    }

    // Fills the pool with AudioSource GameObjects at startup.
    private void PrepopulatePool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            CreateNewAudioSourceInstance();
        }
    }

    // Creates a new AudioSource GameObject and adds it to the available pool.
    private AudioSource CreateNewAudioSourceInstance()
    {
        if (_audioSourcePrefab == null)
        {
            Debug.LogError("SFXPooler: AudioSource Prefab is not assigned! Cannot create new instances.", this);
            return null;
        }

        // Instantiate as a child of the SFXPooler GameObject for organization
        AudioSource newSource = Instantiate(_audioSourcePrefab, transform).GetComponent<AudioSource>();
        if (newSource == null)
        {
            Debug.LogError("SFXPooler: Prefab does not contain an AudioSource component!", this);
            Destroy(newSource.gameObject); // Clean up if prefab is wrong
            return null;
        }

        newSource.gameObject.SetActive(false); // Deactivate until needed
        _availableSources.Enqueue(newSource);
        return newSource;
    }

    // Retrieves an AudioSource from the pool. Grows the pool if necessary.
    public AudioSource GetPooledAudioSource()
    {
        AudioSource source;
        if (_availableSources.Count > 0)
        {
            source = _availableSources.Dequeue();
        }
        else
        {
            // If pool is exhausted, create a new one (dynamic growth)
            Debug.LogWarning("SFX Pool exhausted. Creating new AudioSource instance dynamically.", this);
            source = CreateNewAudioSourceInstance();
            if (source == null) return null; // Failed to create
        }

        source.gameObject.SetActive(true); // Activate before use
        _activeSources.Add(source); // Add to list of active sources
        return source;
    }

    // Returns a one-shot AudioSource to the pool after its clip has finished playing.
    public void ReturnPooledAudioSource(AudioSource source, float delay)
    {
        if (source == null) return;
        StartCoroutine(ReturnAfterDelay(source, delay));
    }

    // Returns a looping AudioSource to the pool immediately when explicitly told to stop.
    public void ReturnPooledAudioSource(AudioSource source)
    {
        if (source == null) return;

        if (_activeSources.Contains(source))
        {
            _activeSources.Remove(source);
        }
        else
        {
            // This can happen if an AudioSource was manually stopped and returned
            // but wasn't tracked by _activeSources for some reason (e.g., external stop)
            Debug.LogWarning($"SFXPooler: Attempted to return an AudioSource that was not in _activeSources list: {source.gameObject.name}", source);
        }

        source.Stop(); // Ensure it's stopped
        source.gameObject.SetActive(false); // Deactivate
        source.clip = null; // Clear clip reference
        source.loop = false; // Reset loop state
        source.volume = 1f; // Reset volume
        source.pitch = 1f; // Reset pitch
        source.spatialBlend = 0f; // Reset spatial blend (or whatever your default is)
        source.minDistance = 1f; // Reset 3D settings
        source.maxDistance = 500f; // Reset 3D settings

        _availableSources.Enqueue(source); // Add back to available pool
    }

    private IEnumerator ReturnAfterDelay(AudioSource source, float delay)
    {
        // Wait for the sound to finish playing, plus a small buffer
        yield return new WaitForSeconds(delay);

        // Ensure the source is still playing (it might have been stopped externally)
        // and hasn't been returned by other means (e.g., explicit StopLoopingSFX)
        if (source != null && source.gameObject.activeSelf)
        {
            ReturnPooledAudioSource(source); // Use the common return method
        }
    }
}