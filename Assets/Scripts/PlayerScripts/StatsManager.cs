using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("Health")]
    public int maxHP;
    public int currentHP;


    [Header("CashMoneyFlow")]
    public int doubloons;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

}
