using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : Singleton<Statistics>
{
    public int WheatCollected { get; private set; }
    public int UnitsLost { get; private set; }
    public int UnitsKilled { get; private set; }

    private void Start()
    {
        GameManager.Instance.OnWheatCollected.AddListener(HandleWheatCollected);
        GameManager.Instance.OnUnitsLost.AddListener(HandleUnitsLost);
        GameManager.Instance.OnUnitsKilled.AddListener(HandleUnitsKilled);
    }

    private void HandleWheatCollected(int wheatAmount)
    {
        WheatCollected += wheatAmount;
    }

    private void HandleUnitsLost(int unitsLost)
    {
        UnitsLost += unitsLost;
    }

    private void HandleUnitsKilled(int unitsKilled)
    {
        UnitsKilled += unitsKilled;
    }
}
