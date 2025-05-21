using Fusion.XR.Shared.Locomotion;
using Photon.Voice.Fusion;
using UnityEngine;
using UnityEngine.Events;

/**
 * 
 * Enable the fade of the XR camera during voice connection (the Unity mic setup induce a slight unpleasant freeze)
 * 
 **/
public class VoiceConnectionFadeManager : MonoBehaviour
{
    public FusionVoiceClient fusionVoiceClient;
    [Header("Voice Callbacks")]
    public UnityEvent onVoiceConnectionJoined;

    bool didVoiceConnectionJoined = false;

    public bool autoRegisterFadeOutOnVoiceConnection = true;

    protected virtual void Awake()
    {
        // Find the VoiceConnection, if not defined
        if (fusionVoiceClient == null) fusionVoiceClient = GetComponent<FusionVoiceClient>();
        if (fusionVoiceClient == null)
        {
            Debug.LogError("Should be stored next to a FusionVoiceClient (or fusionVoiceClient should be set)");
            return;
        }

        if (autoRegisterFadeOutOnVoiceConnection)
        {
            if (fusionVoiceClient)
            {
                foreach (var fader in FindObjectsOfType<Fader>())
                {
                    fader.startFadeLevel = 1;
                }
            }
        }
    }

    void FadeOutOnVoiceConnection()
    {
        if (!autoRegisterFadeOutOnVoiceConnection) return;
        foreach (var fader in FindObjectsOfType<Fader>())
        {
            fader.AnimateFadeOut(1);
        }
    }

    protected virtual void Update()
    {
        if (!didVoiceConnectionJoined && fusionVoiceClient && fusionVoiceClient.ClientState == Photon.Realtime.ClientState.Joined)
        {
            didVoiceConnectionJoined = true;
            OnVoiceConnectionJoined();
        }
    }
    
    protected virtual void OnVoiceConnectionJoined()
    {
        FadeOutOnVoiceConnection();
        if (onVoiceConnectionJoined != null) onVoiceConnectionJoined.Invoke();
    }
}
