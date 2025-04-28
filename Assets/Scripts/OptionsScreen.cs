using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button evaluationButton;
    [SerializeField] GameControllerBridge gameControllerBridge;//Transicion de escenas
    void Start()
    {
        playButton.onClick.AddListener(CargarJuegoInversion);
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