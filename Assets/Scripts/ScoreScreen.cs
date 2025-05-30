using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI finalScoreText;
    //ScoreKeeper scoreKeeper;
    private int monedas;
    private int aciertos;

    //void Awake()
    //{
    //    scoreKeeper = FindObjectOfType<ScoreKeeper>();
    //}

    public void ShowFinalScore()
    {
        monedas = PlayerPrefs.GetInt("quizz_monedas", 0);
        aciertos = PlayerPrefs.GetInt("quizz_aciertos", 0);
        finalScoreText.text = "Felicidades!\nObtuviste " + aciertos + " preguntas correctas y " + monedas + " moneda coleccionable";

        //finalScoreText.text = "Felicidades!\nTienes " + scoreKeeper.CalculateScoreQuizz() + " Puntos de conocimiento";
     }
}
