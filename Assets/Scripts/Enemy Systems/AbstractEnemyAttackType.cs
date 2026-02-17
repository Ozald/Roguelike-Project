using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbstractEnemyAttackType: ScriptableObject
{
    float closenessToPlayer; //used to judge when to attack the player in EnemyBaseController
    void Attack(){} //All attack S.O.s will inherit this function, making EnemyBaseController inherit any type of attack given.
}
