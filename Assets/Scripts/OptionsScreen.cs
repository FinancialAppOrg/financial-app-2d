using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button evaluationButton;

    void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        evaluationButton.onClick.AddListener(OnEvaluationClicked);
    }

    void OnPlayClicked()
    {
        FindObjectOfType<GameManager>().ShowSelectTopicScreen();
    }

    void OnEvaluationClicked()
    {
        FindObjectOfType<GameManager>().ShowEvaluationScreen();
    }
}