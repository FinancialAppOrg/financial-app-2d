using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] float timeToCompleteQuestion = 30f;
    [SerializeField] float timeToShowCorrectAnswer = 10f;

    public bool loadNextQuestion;
    public float fillFraction;

    public bool isAnsweringQuestion = false;
    float timerValue;

    void Update()
    {
        UpdateTimer();

    }

    public void StartNewQuestion()
    {
        isAnsweringQuestion = true;
        timerValue = timeToCompleteQuestion;
        loadNextQuestion = false;
        Debug.Log("Timer iniciado para nueva pregunta");
    }

    public void CancelTimer()
    {
        timerValue = 0;
    }

    void UpdateTimer()
    {
        timerValue -= Time.deltaTime;
        //timerValue = timerValue - Timer.deltaTime;

        if (isAnsweringQuestion)
        {
            if (timerValue > 0)
            {
                fillFraction = timerValue / timeToCompleteQuestion; //5/10 = 0.5
            }
            else
            {
                isAnsweringQuestion = false;
                timerValue = timeToShowCorrectAnswer;
            }
        }
        else
        {
            if (timerValue > 0)
            {
                fillFraction = timerValue / timeToShowCorrectAnswer;
            }
            else
            {
                isAnsweringQuestion = true;
                timerValue = timeToCompleteQuestion;
                loadNextQuestion = true;
            }
        }

        //Debug.Log(timerValue);
        //Debug.Log(isAnsweringQuestion + ": " + timerValue + " = " + fillFraction);
    }
}
