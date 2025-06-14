using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] Button menuButton;
    //ScoreKeeper scoreKeeper;
    private int monedas;
    private int aciertos;
    private GameManager gameManager;

    //void Awake()
    //{
    //    scoreKeeper = FindObjectOfType<ScoreKeeper>();
    //}

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Configurar el botón si existe
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuButtonClicked);
        }
    }

    public void ShowFinalScore()
    {
        monedas = PlayerPrefs.GetInt("quizz_monedas", 0);
        aciertos = PlayerPrefs.GetInt("quizz_aciertos", 0);
        finalScoreText.text = "Felicidades!\nObtuviste " + aciertos + " preguntas correctas y " + monedas + " moneda coleccionable";

        //finalScoreText.text = "Felicidades!\nTienes " + scoreKeeper.CalculateScoreQuizz() + " Puntos de conocimiento";
     }

    private void OnMenuButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.ShowMenuScreen();
        }
        else
        {
            Debug.LogError("GameManager no encontrado");
        }
    }
}
