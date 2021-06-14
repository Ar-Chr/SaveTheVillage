using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Unit Info")]
public class Unit_SO : ScriptableObject
{
    public enum UnitType
    {
        Melee,
        Ranged,
        NonCombat
    }

    public Unit_SO(int cost, float trainTime)
    {
        this.cost = cost;
        this.trainTime = trainTime;
    }

    public int startingCount;
    public int cost;
    public float trainTime;
    public Sound trainedSound;
    public int upkeep;
    [Space]
    public int killsEnemies;
    public bool isKilled;
    public UnitType unitType;
}
