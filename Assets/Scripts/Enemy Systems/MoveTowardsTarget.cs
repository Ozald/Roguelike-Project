using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsTarget : MonoBehaviour
{
    public GameObject weapon;
    public GameObject targetPostion;
    
    [Range(0f,10f)]
    public float speed = 1f;
    
    void Update()
    {
        weapon.transform.position = Vector3.MoveTowards(transform.position, 
            targetPostion.transform.position, speed * Time.deltaTime);
        
    }
}
