using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region Fields
    [SerializeField] private HashSet<UnitUI> test;
    [SerializeField] private UnitUI[] unitUis;
    [Space]
    [SerializeField] private Text wheatCount;
    [Space]
    [SerializeField] private Image harvestIcon;
    [SerializeField] private Text wheatDifference;
    [Space]
    [SerializeField] private Image raidIcon;
    [SerializeField] private Text enemiesCount;
    [Space]
    [SerializeField] private Text timer;
    [Space]
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject defeatScreen;
    [SerializeField] private GameObject overthrownScreen;
    [SerializeField] private GameObject pauseMenu;
    [Space]
    [SerializeField] private Sound buttonClickSound;
    #endregion

    private void Start()
    {
        foreach (UnitUI unitUi in unitUis)
        {
            AddTrainButtonOnClick(unitUi);
            unitUi.trainCostText.text = unitUi.unit.cost.ToString();
        }

        AddClickSoundToAllButtons();
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void Update()
    {
        int hours = (int)Time.time / 3600;
        int minutes = (int)Time.time % 3600 / 60;
        int seconds = (int)Time.time % 60;
        timer.text = $"{hours}:{minutes:00}:{seconds:00}";
    }

    private void AddTrainButtonOnClick(UnitUI unitUi)
    {
        float trainTime = unitUi.unit.trainTime;
        Button button = unitUi.trainButton;

        button.onClick.AddListener(() =>
        {
            GameManager.Instance.TrainUnit(unitUi.unit);
            StartCoroutine(IconFillUpdate(unitUi.progressIcon, trainTime));
            unitUi.IsTraining = true;
            StartCoroutine(GameManager.Instance.DelayAction(trainTime, () => unitUi.IsTraining = false));
        });
    }

    private void AddClickSoundToAllButtons()
    {
        foreach (Button button in Resources.FindObjectsOfTypeAll(typeof(Button)))
        {
            button.onClick.AddListener(() => AudioManager.Instance.Play(buttonClickSound));
        }
    }

    #region Update methods
    public void UpdateWheatCount(int wheat)
    {
        wheatCount.text = $"{wheat}/{GameManager.Instance.TargetWheatCount}";

        foreach (UnitUI unitUi in unitUis)
        {
            if (wheat >= unitUi.unit.cost)
                unitUi.IsEnoughWheat = true;
            else
                unitUi.IsEnoughWheat = false;
        }
    }

    public void UpdateUnitCount(Unit_SO unit, int count)
    {
        Text countText = unitUis.First(ui => ui.unit == unit).countText;
        countText.text = count.ToString();
    }

    public void UpdateEnemiesCount(int count)
    {
        enemiesCount.text = count.ToString();
    }

    public void UpdateWheatDifference(int difference)
    {
        wheatDifference.text = difference.ToString();
    }

    private IEnumerator IconFillUpdate(Image icon, float duration)
    {
        float maxFillTime = Time.time + duration;
        while (Time.time < maxFillTime)
        {
            icon.fillAmount = 1 - (maxFillTime - Time.time) / duration;
            yield return new WaitForSeconds(0);
        }
    }

    public void UpdateHarvestIcon(float fill)
    {
        harvestIcon.fillAmount = fill;
    }

    public void UpdateRaidIcon(float fill)
    {
        raidIcon.fillAmount = fill;
    }
    #endregion

    #region Event handlers
    public void HandleVictory()
    {
        victoryScreen.SetActive(true);
    }

    public void HandleDefeat()
    {
        defeatScreen.SetActive(true);
    }

    public void HandleOverthrow()
    {
        overthrownScreen.SetActive(true);
    }

    private void HandleGameStateChanged(GameManager.GameState previous, GameManager.GameState current)
    {
        pauseMenu.SetActive(current == GameManager.GameState.PAUSED);
    }
    #endregion
}

[Serializable]
public class UnitUI
{
    public Unit_SO unit;
    [Space]
    public Text countText;
    [Space]
    public Button trainButton;
    public Image progressIcon;
    public Text trainCostText;

    private bool isTraining;
    public bool IsTraining
    {
        get => isTraining;
        set
        {
            isTraining = value;
            trainButton.interactable = interactable;
        }
    }

    private bool isEnoughWheat;
    public bool IsEnoughWheat
    {
        get => isEnoughWheat;
        set
        {
            isEnoughWheat = value;
            trainButton.interactable = interactable;
        }
    }

    private bool interactable => isEnoughWheat && !isTraining;
}
