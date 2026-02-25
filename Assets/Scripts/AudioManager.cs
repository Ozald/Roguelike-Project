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
    public EventInstance currentBGMusic;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        PlayBGMusic(instance.audioData.floor1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayOneShot(audioData.testSFX);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("DEBUG: Changed music to \"Battle\" intensity.");
            currentBGMusic.setParameterByName("Intensity", 1);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("DEBUG: Changed music to \"Calm\" intensity.");
            currentBGMusic.setParameterByName("Intensity", 0);
        }
    }

    public static void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound, Vector3.zero);
    }

    public static void PlayBGMusic(EventReference music)
    {
        instance.currentBGMusic = RuntimeManager.CreateInstance(music);
        instance.currentBGMusic.start();
    }

    public static void StopBGMusic(EventReference music)
    {
        instance.currentBGMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public static FMODEvents GetAudioData()
    {
        return instance.audioData;
    }
}
