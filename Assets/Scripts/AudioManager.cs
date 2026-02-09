using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public FMODEvents audioData;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayOneShot(audioData.testSFX);
        }
    }

    public static void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound, Vector3.zero);
    }

    public static FMODEvents GetAudioData()
    {
        return instance.audioData;
    }
}
