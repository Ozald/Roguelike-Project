using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBaseController : MonoBehaviour
{

    [SerializeField]
    public DefaultEnemySO defaultEnemySO;
    public AbstractEnemyAttackType attackType;
    public AbstractEnemyMovement movementType;

    
    // Start is called before the first frame update
    void Start()
    {
      //Locate if theres an active target found in the map, if not, send an error in console
        
    }

    // Update is called once per frame
    void Update()
    {
        //check constantly for target location
    }
}
