using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultsScoreText;
    [SerializeField] Button resultsButton1;
    [SerializeField] Button resultsButton2;

    GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (resultsButton1 != null)
            resultsButton1.onClick.AddListener(() => gameManager.ShowSelectTopicScreen());
        if (resultsButton2 != null)
            resultsButton2.onClick.AddListener(() => gameManager.ShowSelectTopicScreen());
    }

    public void DisplayResults(int score)
    {
        resultsScoreText.text = score + " Puntos de conocimiento";
    }
}
