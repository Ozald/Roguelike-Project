using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Audio Data")]
public class FMODEvents : ScriptableObject
{
    [Header("Debug SFX")]
    public EventReference testSFX;
    
    [Header("Music")]
    public EventReference floor1;

    [Header("SFX")]
    public EventReference damageTaken;
}
