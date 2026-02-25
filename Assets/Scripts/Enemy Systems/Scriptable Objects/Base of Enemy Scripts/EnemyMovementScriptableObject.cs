using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "New Enemy Movement Type /Enemy -NAME TBA- Movement")]

public class EnemyMovementScriptableObject : AbstractEnemyMovement
{

    [SerializeField]
    protected int movementSpeed = 20;

    void Movement()
    {
        Debug.Log("Please pretend I'm changing the movements here! This is from the EnemyMovementScriptableObject");
    }
 
}
