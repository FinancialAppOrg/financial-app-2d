using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultsScoreText;
    [SerializeField] TextMeshProUGUI correctAnswersText;
    [SerializeField] Button resultsButton1;
    [SerializeField] Button resultsButton2;
    [SerializeField] GameControllerBridge gameControllerBridge;

    GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (resultsButton1 != null)
            resultsButton1.onClick.AddListener(() => gameManager.ShowSelectLevelScreen());
        if (resultsButton2 != null)
            resultsButton2.onClick.AddListener(CargarJuegoInversion);
    }

    public void DisplayResults(int score)
    {
        resultsScoreText.text = score + " Puntos de conocimiento";
    }

    public void DisplayCorrectAnswers(int correctAnswers)
    {
        correctAnswersText.text = correctAnswers + " Preguntas correctas";
    }

    public void CargarJuegoInversion()
    {
        if (gameControllerBridge != null)
        {
            gameControllerBridge.CargarJuegoInversion();
        }
        else
        {
            Debug.LogWarning("No se encontró el GameControllerBridge.");
        }
    }
}
