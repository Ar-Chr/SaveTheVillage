using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostGameScreen : MonoBehaviour
{
    [SerializeField] private Text wheatCollectedText;
    [SerializeField] private Text unitsLostText;
    [SerializeField] private Text unitsKilledText;
    [Space]
    [SerializeField] private Button quitButton;

    private void Start()
    {
        quitButton.onClick.AddListener(() => GameManager.Instance.Quit());
    }

    private void OnEnable()
    {
        wheatCollectedText.text = $"Wheat collected: {Statistics.Instance.WheatCollected}";
        unitsLostText.text = $"Units lost: {Statistics.Instance.UnitsLost}";
        unitsKilledText.text = $"Units killed: {Statistics.Instance.UnitsKilled}";
    }
}
