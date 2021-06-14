using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Fields
    private Dictionary<Unit_SO, int> unitCounts;
    private int wheatCount;

    public int TargetWheatCount => targetWheatCount;

    public Unit_SO[] units;
    [Space]
    [SerializeField] private int startingWheatCount;
    [Space]
    [SerializeField] private int enemiesCount;
    [Space]
    [SerializeField] private float timeBetweenHarvests;
    [SerializeField] private float timeBetweenRaids;
    [Space]
    [SerializeField] private int targetWheatCount;
    [Space]
    [Space]
    [SerializeField] private Sound harvestSound;
    [SerializeField] private Sound raidSound;
    [SerializeField] private Sound arrowsSound;
    [SerializeField] private Sound[] deathSounds;
    [SerializeField] private Sound[] swordClashSounds;

    private float nextHarvestTime;
    private float nextRaidTime;

    private bool hornSounded;

    private GameState gameState;
    #endregion

    #region events
    [HideInInspector] public Events.EventGameState OnGameStateChanged;
    [HideInInspector] public Events.EventWheatCollected OnWheatCollected;
    [HideInInspector] public Events.EventUnitsLost OnUnitsLost;
    [HideInInspector] public Events.EventUnitsKilled OnUnitsKilled;
    #endregion

    private void Start()
    {
        gameState = GameState.RUNNING;

        nextHarvestTime = Time.time + timeBetweenHarvests;
        nextRaidTime = Time.time + timeBetweenRaids;

        unitCounts = units.ToDictionary(unit => unit, unit => 0);

        foreach (Unit_SO unit in units)
            ChangeUnitCount(unit, unit.startingCount);

        UIManager.Instance.UpdateEnemiesCount(enemiesCount);
        UIManager.Instance.UpdateWheatDifference(GetWheatDifference());

        ChangeWheatCount(startingWheatCount);

        OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void Update()
    {
        if (Time.time >= nextHarvestTime)
            HandleHarvest();

        if (!hornSounded && Time.time >= nextRaidTime - 3f)
        {
            AudioManager.Instance.Play(raidSound);
            hornSounded = true;
        }

        if (Time.time >= nextRaidTime)
        {
            HandleRaid();
            hornSounded = false;
        }

        UIManager.Instance.UpdateHarvestIcon(1 - (nextHarvestTime - Time.time) / timeBetweenHarvests);
        UIManager.Instance.UpdateRaidIcon(1 - (nextRaidTime - Time.time) / timeBetweenRaids);

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    #region Unit methods
    public void TrainUnit(Unit_SO unit)
    {
        ChangeWheatCount(-unit.cost);
        Action action = () =>
        {
            ChangeUnitCount(unit, 1);
            AudioManager.Instance.Play(unit.trainedSound);
        };
        StartCoroutine(DelayAction(unit.trainTime, action));
    }

    private void ChangeUnitCount(Unit_SO unit, int delta)
    {
        unitCounts[unit] += delta;
        unitCounts[unit] = Math.Max(0, unitCounts[unit]);
        UIManager.Instance.UpdateUnitCount(unit, unitCounts[unit]);
        UIManager.Instance.UpdateWheatDifference(GetWheatDifference());
    }
    #endregion

    #region Wheat methods
    private void ChangeWheatCount(int wheatAmount)
    {
        wheatCount += wheatAmount;
        UIManager.Instance.UpdateWheatCount(wheatCount);

        if (wheatCount < 0)
            HandleOverthrow();

        if (wheatCount >= targetWheatCount) 
            HandleVictory();
    }

    private int GetWheatDifference()
    {
        int difference = 0;
        foreach (var pair in unitCounts)
            difference -= pair.Value * pair.Key.upkeep;

        return difference;
    }

    private void HandleHarvest()
    {
        AudioManager.Instance.Play(harvestSound);
        nextHarvestTime = Time.time + timeBetweenHarvests;

        int wheatHarvested = GetWheatDifference();
        OnWheatCollected.Invoke(wheatHarvested);
        ChangeWheatCount(wheatHarvested);
    }
    #endregion

    #region Raid methods
    private void HandleRaid()
    {
        int enemiesLeft = enemiesCount;

        foreach (Unit_SO unit in units)
            FightWithUnit(unit, ref enemiesLeft);

        if (enemiesLeft > 0)
            HandleDefeat();

        nextRaidTime = Time.time + timeBetweenRaids;
        enemiesCount++;
        UIManager.Instance.UpdateEnemiesCount(enemiesCount);
    }

    private void FightWithUnit(Unit_SO unit, ref int enemiesLeft)
    {
        lock (unitCounts)
        {
            if (enemiesLeft <= 0)
                return;
            if (unitCounts[unit] == 0)
                return;

            int enemyCasualties = Math.Min(enemiesLeft, unit.killsEnemies * unitCounts[unit]);
            int playerCasualties = unit.isKilled ? enemyCasualties / unit.killsEnemies : 0;

            if (enemyCasualties > 0)
                switch (unit.unitType)
                {
                    case Unit_SO.UnitType.Melee:
                        PlaySwordClashes();
                        break;
                    case Unit_SO.UnitType.Ranged:
                        AudioManager.Instance.Play(arrowsSound);
                        break;
                }

            if (playerCasualties > 0)
                PlayDeathSound();

            OnUnitsKilled.Invoke(enemyCasualties);
            OnUnitsLost.Invoke(playerCasualties);

            enemiesLeft -= enemyCasualties;
            ChangeUnitCount(unit, -playerCasualties);
        }
    }

    private void PlayDeathSound()
    {
        AudioManager.Instance.Play(deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)]);
    }

    private void PlaySwordClashes()
    {
        float delay = 0;
        for (int i = 0; i < 3; i++)
        {
            Sound sound = swordClashSounds[UnityEngine.Random.Range(0, swordClashSounds.Length)];
            StartCoroutine(DelayAction(delay, () => AudioManager.Instance.Play(sound)));
            delay += sound.clip.length;
        }
    }
    #endregion

    #region Game end methods
    private void HandleVictory()
    {
        UIManager.Instance.HandleVictory();
        Time.timeScale = 0;
    }

    private void HandleDefeat()
    {
        UIManager.Instance.HandleDefeat();
        Time.timeScale = 0;
    }

    private void HandleOverthrow()
    {
        UIManager.Instance.HandleOverthrow();
        Time.timeScale = 0;
    }
    #endregion

    public void TogglePause()
    {
        ChangeGameState(gameState == GameState.PAUSED ? GameState.RUNNING : GameState.PAUSED);
    }

    private void ChangeGameState(GameState newState)
    {
        var previousState = gameState;
        gameState = newState;
        OnGameStateChanged.Invoke(previousState, newState);
    }

    private void HandleGameStateChanged(GameState previous, GameState current)
    {
        Time.timeScale = current == GameState.PAUSED ? 0f : 1f;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public IEnumerator DelayAction(float delayInSeconds, Action action)
    {
        yield return new WaitForSeconds(delayInSeconds);
        action();
    }

    public enum GameState
    {
        PAUSED,
        RUNNING
    }
}