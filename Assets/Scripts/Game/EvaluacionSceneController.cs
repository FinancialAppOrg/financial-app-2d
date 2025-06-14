using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluacionSceneController : MonoBehaviour
{
    public GameObject quizPanel;
    public GameObject selfAssessmentScreen;
    public GameObject scoreScreen;
    public GameObject menuScreen;
    public GameManager gameManager;

    IEnumerator Start()
    {
        yield return null;

        bool hasCompletedEvaluation = gameManager.CheckIfAnyEvaluationCompleted();
        string destino = PlayerPrefs.GetString("pantallaEvaluacion", "selfAssessmentScreen");

        Debug.Log($"Valor de pantallaEvaluacion: {destino}, Evaluación completada: {hasCompletedEvaluation}");
        //string destino = PlayerPrefs.GetString("pantallaEvaluacion", "");

        int quizzId = PlayerPrefs.GetInt("quizzId", 0);

        if (hasCompletedEvaluation && destino != "quizz")
        {
            Debug.Log("Mostrando MenuScreen - Usuario con evaluación previa");
            ShowMenuScreen();
            yield break;
        }

        if (destino == "quizz")
        {
            Debug.Log("Activando pantalla de quiz");
            Debug.Log("Quizz ID almacenado: " + quizzId);
            yield return StartCoroutine(HandleQuizFlow(quizzId));
        }
        else
        {
            Debug.Log("Activando pantalla de autoevaluación");
            ShowSelfAssessmentScreen();
        }
    }

    private IEnumerator HandleQuizFlow(int quizzId)
    {
        gameManager.ResetQuizState();
        gameManager.QuizzPanels();

        yield return StartCoroutine(gameManager.GetQuestionsFromAPI(quizzId));

        if (gameManager.questions != null && gameManager.questions.Count > 0)
        {
            var quiz = quizPanel.GetComponent<Quiz>();
            if (quiz != null)
            {
                gameManager.InitializeNewQuiz(gameManager.questions);
                quiz.gameObject.SetActive(true);
                quiz.OnQuizCompleted += gameManager.HandleQuizCompleted;
            }
        }
        else
        {
            Debug.LogError("No se encontraron preguntas para el quiz con ID: " + quizzId);
 
            if (gameManager.CheckIfAnyEvaluationCompleted())
            {
                ShowMenuScreen();
            }
            else
            {
                ShowSelfAssessmentScreen();
            }
        }
    }
    private void ShowMenuScreen()
    {
        menuScreen.SetActive(true);
        quizPanel.SetActive(false);
        selfAssessmentScreen.SetActive(false);
        scoreScreen.SetActive(false);
    }

    private void ShowSelfAssessmentScreen()
    {
        menuScreen.SetActive(false);
        quizPanel.SetActive(false);
        selfAssessmentScreen.SetActive(true);
        scoreScreen.SetActive(false);
    }
}
