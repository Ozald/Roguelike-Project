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
      
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
