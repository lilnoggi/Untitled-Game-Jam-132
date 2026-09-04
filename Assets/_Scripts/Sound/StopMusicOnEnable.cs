using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class StopMusicOnEnable : MonoBehaviour
{   
    [Header("FMOD Settings")]
    [SerializeField] private StudioEventEmitter eventEmitter;
    private void OnEnable()
    {
        if (eventEmitter != null && eventEmitter.IsPlaying ())
        {
            eventEmitter.Stop();
        }
    }
}