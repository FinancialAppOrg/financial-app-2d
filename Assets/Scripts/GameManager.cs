using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Quiz quiz;
    ScoreScreen scoreScreen;
    SelectTopicScreen selectTopicScreen;
    SelectLevelScreen selectLevelScreen;

    InterestSelectionScreen interestSelectionScreen;
    SelfAssessmentScreen selfAssessmentScreen;
    OptionsScreen optionsScreen;
    EvaluationScreen evaluationScreen;
    ResultsScreen resultsScreen;

    string selectedTopic;

    void Awake()
    {
        // Asegúrate de que estos objetos existan en la escena y estén correctamente asignados
        quiz = FindObjectOfType<Quiz>();
        scoreScreen = FindObjectOfType<ScoreScreen>();
        selectTopicScreen = FindObjectOfType<SelectTopicScreen>();
        selectLevelScreen = FindObjectOfType<SelectLevelScreen>();
        interestSelectionScreen = FindObjectOfType<InterestSelectionScreen>();
        selfAssessmentScreen = FindObjectOfType<SelfAssessmentScreen>();
        optionsScreen = FindObjectOfType<OptionsScreen>();
        evaluationScreen = FindObjectOfType<EvaluationScreen>();
        resultsScreen = FindObjectOfType<ResultsScreen>();

        // Verifica que los objetos no sean null
        if (quiz == null) Debug.LogError("Quiz no encontrado en la escena.");
        if (scoreScreen == null) Debug.LogError("ScoreScreen no encontrado en la escena.");
        if (selectTopicScreen == null) Debug.LogError("SelectTopicScreen no encontrado en la escena.");
        if (selectLevelScreen == null) Debug.LogError("SelectLevelScreen no encontrado en la escena.");
        if (interestSelectionScreen == null) Debug.LogError("InterestSelectionScreen no encontrado en la escena.");
        if (selfAssessmentScreen == null) Debug.LogError("SelfAssessmentScreen no encontrado en la escena.");
        if (optionsScreen == null) Debug.LogError("OptionsScreen no encontrado en la escena.");
        if (evaluationScreen == null) Debug.LogError("EvaluationScreen no encontrado en la escena.");
        if (resultsScreen == null) Debug.LogError("ResultsScreen no encontrado en la escena.");
    }

    void Start()
    {
        // Asegúrate de que los objetos no sean null antes de usarlos
        if (quiz != null) quiz.gameObject.SetActive(false);
        if (scoreScreen != null) scoreScreen.gameObject.SetActive(false);
        if (selectTopicScreen != null) selectTopicScreen.gameObject.SetActive(false);
        if (selectLevelScreen != null) selectLevelScreen.gameObject.SetActive(false);
        if (interestSelectionScreen != null) interestSelectionScreen.gameObject.SetActive(false);
        if (selfAssessmentScreen != null) selfAssessmentScreen.gameObject.SetActive(true);
        if (optionsScreen != null) optionsScreen.gameObject.SetActive(false);
        if (evaluationScreen != null) evaluationScreen.gameObject.SetActive(false);
        if (resultsScreen != null) resultsScreen.gameObject.SetActive(false);
    }

    public void ShowSelfAssessmentScreen()
    {
        if (interestSelectionScreen != null) interestSelectionScreen.gameObject.SetActive(false);
        if (selfAssessmentScreen != null) selfAssessmentScreen.gameObject.SetActive(true);
    }

    public void ShowInterestSelectionScreen()
    {
        if (interestSelectionScreen != null) interestSelectionScreen.gameObject.SetActive(true);
        if (selfAssessmentScreen != null) selfAssessmentScreen.gameObject.SetActive(false);
    }

    public void ShowOptionsScreen()
    {
        if (interestSelectionScreen != null) interestSelectionScreen.gameObject.SetActive(false);
        if (optionsScreen != null) optionsScreen.gameObject.SetActive(true);
    }

    public void ShowSelectTopicScreen()
    {
        if (optionsScreen != null) optionsScreen.gameObject.SetActive(false);
        if (selectTopicScreen != null) selectTopicScreen.gameObject.SetActive(true);
        if (resultsScreen != null) resultsScreen.gameObject.SetActive(false);
    }

    public void ShowEvaluationScreen()
    {
        if (optionsScreen != null) optionsScreen.gameObject.SetActive(false);
        if (evaluationScreen != null) evaluationScreen.gameObject.SetActive(true);
    }

    public void ShowResultsScreen()
    {
        if (evaluationScreen != null) evaluationScreen.gameObject.SetActive(false);
        if (resultsScreen != null) resultsScreen.gameObject.SetActive(true);
    }
    public void ShowSelectLevelScreen()
    {
        if (resultsScreen != null) resultsScreen.gameObject.SetActive(false);
        if (selectLevelScreen != null) selectLevelScreen.gameObject.SetActive(true);

    }
    public void StartQuiz(string topic)
    {
        selectedTopic = topic;
        if (selectTopicScreen != null) selectTopicScreen.gameObject.SetActive(true);
        if (selectLevelScreen != null) selectLevelScreen.gameObject.SetActive(false);
        
    }

    public void StartQuizWithLevel(string level)
    {
        if (selectLevelScreen != null) selectLevelScreen.gameObject.SetActive(false);
        if (quiz != null)
        {
            quiz.SetTopicAndLevel(selectedTopic, level);
            //quiz.SetTopic(topic);
            quiz.gameObject.SetActive(true);
            scoreScreen.gameObject.SetActive(false);
            quiz.OnQuizCompleted += HandleQuizCompleted;
        }
    }

    public void HandleQuizCompleted()
    {
        if (quiz != null) quiz.gameObject.SetActive(false);
        if (scoreScreen != null)
        {
            scoreScreen.gameObject.SetActive(true);
            scoreScreen.ShowFinalScore();
        }
    }

    /*
    void OnDestroy()
    {
        if (quiz != null) quiz.OnQuizCompleted -= HandleQuizCompleted;
    }
    */

    public void OnReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
