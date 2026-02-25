using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Enemy Attack/Enemy Heavy Attack")]
public class HeavyAttackSO : AbstractEnemyAttackType
{

    [SerializeField]
    protected int damage;
     void Attack()
    {
        Debug.Log("Please pretend im attacking from the AttackScriptableObject script!");
    }
}
