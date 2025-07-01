using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button progressButton;
    [SerializeField] private Button coinsButton;

    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        playButton.onClick.AddListener(OnPlayButtonClicked);
        progressButton.onClick.AddListener(OnProgressButtonClicked);
        coinsButton.onClick.AddListener(OnCoinsButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        gameManager.OnPlayButtonClicked();
    }

    private void OnProgressButtonClicked()
    {
        gameManager.OnProgressButtonClicked();
    }

    private void OnCoinsButtonClicked()
    {
        gameManager.OnCoinsButtonClicked();
    }
}