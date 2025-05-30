using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluacionSceneController : MonoBehaviour
{
    public GameObject quizPanel;
    public GameObject selfAssessmentScreen;
    public GameObject scoreScreen;
    public GameManager gameManager;

    IEnumerator Start()
    {
        yield return null;

        string destino = PlayerPrefs.GetString("pantallaEvaluacion", "selfAssessmentScreen");
        Debug.Log("Valor de pantallaEvaluacion: " + destino);// Valor predeterminado
        //string destino = PlayerPrefs.GetString("pantallaEvaluacion", "");
        int quizzId = PlayerPrefs.GetInt("quizz_id", 0);
        if (destino == "quizz")
        {
            Debug.Log("Activando pantalla de quiz");
            Debug.Log("Quizz ID almacenado: " + quizzId);

            if (quizPanel != null)
            {
                gameManager.ResetQuizState();
                gameManager.QuizzPanels();
                // Cargar preguntas
                yield return StartCoroutine(gameManager.GetQuestionsFromAPI(quizzId));

                if (gameManager.questions != null && gameManager.questions.Count > 0)
                {
                    var quiz = quizPanel.GetComponent<Quiz>();
                    if (quiz != null)
                    {
                        gameManager.InitializeNewQuiz(gameManager.questions);//quiz.Init(gameManager.questions);
                        quiz.gameObject.SetActive(true); 
                        quiz.OnQuizCompleted += gameManager.HandleQuizCompleted;
                    }
                }
                else
                {
                    Debug.LogError("No se encontraron preguntas para el quiz con ID: " + quizzId);
                }
            }
        }
        else
        {
            Debug.Log("Activando pantalla de autoevaluación");
            selfAssessmentScreen.SetActive(true);
            quizPanel.SetActive(false);
            scoreScreen.SetActive(false);
        }
    }
}
