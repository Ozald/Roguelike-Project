using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyBaseController : MonoBehaviour
{

    [SerializeField]
    public DefaultEnemySO defaultEnemySO;
    public AbstractEnemyMovement movementType;
    
    //public AbstractEnemyAttackType attackType;
    
    // ^ I think instead of an attack type, the weapon will take in the Attacks rather than the enemy
    //Then we give a weapon type to the Enemy script here
    
    
    
    
    void Start()
    { 
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D coll = GetComponent<BoxCollider2D>(); //for the base enemy's sprite
        coll.isTrigger = true;
        coll.size =  spriteRenderer.bounds.size;
        


    }

    // Update is called once per frame
    void Update()
    {
        //check constantly for target location
    }
}
