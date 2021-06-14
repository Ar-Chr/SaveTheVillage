using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndgameScreen : MonoBehaviour
{
    [SerializeField] private Sound popupSound;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button postGameButton;
    [SerializeField] private GameObject postGameScreen;

    private void Start()
    {
        quitButton.onClick.AddListener(() => GameManager.Instance.Quit());
        postGameButton.onClick.AddListener(() => gameObject.SetActive(false));
        postGameButton.onClick.AddListener(() => postGameScreen.SetActive(true));
    }

    private void OnEnable()
    {
        AudioManager.Instance.Play(popupSound);
    }
}
