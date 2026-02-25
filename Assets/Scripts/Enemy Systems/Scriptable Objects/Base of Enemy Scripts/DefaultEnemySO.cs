using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(menuName = "New Enemy Type/Enemy Basic Data")]
public class DefaultEnemySO : ScriptableObject
{
    [Header("Basic Fields")]
    public string enemyName;
    public int health;

}


