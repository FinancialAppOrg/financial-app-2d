using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    int correctAnswers= 0;
    int questionsSeen = 0;
    private float backendScore = 0f;
    private int correctAnswersB = 0;

    public int GetCorrectAnswers()
    {
        return correctAnswersB;
    }

    public void UpdateCorrectAnswers(int correct)
    {
        correctAnswersB = correct;
    }

    public void IncrementCorrectAnswers()
    {
        correctAnswers++;
    }

    public void UpdateScore(float score)
    {
        backendScore = score;
    }

    public int GetQuestionsSeen()
    {
        return questionsSeen;
    }

    public void IncrementQuestionsSeen()
    {
        questionsSeen++;
    }

    public int CalculateScore()
    {
        return (int)backendScore;
        //int incorrectAnswers = questionsSeen - correctAnswers;
        //return (correctAnswers * 10) - (incorrectAnswers * 5);
    }
    public void Reset()
    {
        correctAnswers = 0;
        questionsSeen = 0;
        backendScore = 0f;
        correctAnswersB = 0;
    }

}
