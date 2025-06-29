using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    
    private void OnLevelComplete()
    {
        audioSource.Play();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        
        GridManager.OnLevelFinished += OnLevelComplete;
    }

    private void OnDestroy()
    {
        GridManager.OnLevelFinished -= OnLevelComplete;
    }
}
