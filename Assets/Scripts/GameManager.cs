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

    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private GameObject updateProfileScreen; 

    [SerializeField] private GameObject progressPanel;
    [SerializeField] private GameObject notificationsPanel;
    [SerializeField] private GameObject logoutConfirmationPanel;

    private GameObject currentPopup;
    private GameObject previousScreen;

    [SerializeField] private GameObject closeSettingsButton;

    void Awake()
    {
        quiz = FindObjectOfType<Quiz>();
        scoreScreen = FindObjectOfType<ScoreScreen>();
        selectTopicScreen = FindObjectOfType<SelectTopicScreen>();
        selectLevelScreen = FindObjectOfType<SelectLevelScreen>();
        interestSelectionScreen = FindObjectOfType<InterestSelectionScreen>();
        selfAssessmentScreen = FindObjectOfType<SelfAssessmentScreen>();
        optionsScreen = FindObjectOfType<OptionsScreen>();
        evaluationScreen = FindObjectOfType<EvaluationScreen>();
        resultsScreen = FindObjectOfType<ResultsScreen>();
        updateProfileScreen = FindObjectOfType<UpdateProfileScreen>()?.gameObject; 

        if (quiz == null) Debug.LogError("Quiz no encontrado en la escena.");
        if (scoreScreen == null) Debug.LogError("ScoreScreen no encontrado en la escena.");
        if (selectTopicScreen == null) Debug.LogError("SelectTopicScreen no encontrado en la escena.");
        if (selectLevelScreen == null) Debug.LogError("SelectLevelScreen no encontrado en la escena.");
        if (interestSelectionScreen == null) Debug.LogError("InterestSelectionScreen no encontrado en la escena.");
        if (selfAssessmentScreen == null) Debug.LogError("SelfAssessmentScreen no encontrado en la escena.");
        if (optionsScreen == null) Debug.LogError("OptionsScreen no encontrado en la escena.");
        if (evaluationScreen == null) Debug.LogError("EvaluationScreen no encontrado en la escena.");
        if (resultsScreen == null) Debug.LogError("ResultsScreen no encontrado en la escena.");
        if (updateProfileScreen == null) Debug.LogError("UpdateProfileScreen no encontrado en la escena.");
    }

    void Start()
    {
        if (quiz != null) quiz.gameObject.SetActive(false);
        if (scoreScreen != null) scoreScreen.gameObject.SetActive(false);
        if (selectTopicScreen != null) selectTopicScreen.gameObject.SetActive(false);
        if (selectLevelScreen != null) selectLevelScreen.gameObject.SetActive(false);
        if (interestSelectionScreen != null) interestSelectionScreen.gameObject.SetActive(false);
        if (selfAssessmentScreen != null) selfAssessmentScreen.gameObject.SetActive(true);
        if (optionsScreen != null) optionsScreen.gameObject.SetActive(false);
        if (evaluationScreen != null) evaluationScreen.gameObject.SetActive(false);
        if (resultsScreen != null) resultsScreen.gameObject.SetActive(false);
        if (updateProfileScreen != null) updateProfileScreen.gameObject.SetActive(false); // Asegúrate de que esté oculto al inicio
    }

    public void ShowSettingsPanel()
    {
        previousScreen = GetActiveScreen();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void HideSettingsPanel()
    {
        if (currentPopup != null)
        {
            Debug.Log("Primero cierra el panel secundario antes de cerrar el panel de configuración.");
            return; // No permite cerrar el settingsPanel si hay un popup activo
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (previousScreen != null)
        {
            previousScreen.SetActive(true);
        }
    }


    public void CloseCurrentPopup()
    {
        if (currentPopup != null)
        {
            currentPopup.SetActive(false);
            currentPopup = null;
            if (closeSettingsButton != null) 
            {
                closeSettingsButton.SetActive(true); 
            }
        }
    }

    public void ShowProgressPanel()
    {
        CloseCurrentPopup();
        if (progressPanel != null)
        {
            progressPanel.SetActive(true);
            currentPopup = progressPanel;
        }
    }

    public void ShowNotificationsPanel()
    {
        CloseCurrentPopup();
        if (notificationsPanel != null)
        {
            notificationsPanel.SetActive(true);
            currentPopup = notificationsPanel;
            if (closeSettingsButton != null) closeSettingsButton.SetActive(false); 
        }
    }

    public void ShowLogoutConfirmationPanel()
    {
        CloseCurrentPopup();
        if (logoutConfirmationPanel != null)
        {
            logoutConfirmationPanel.SetActive(true);
            currentPopup = logoutConfirmationPanel;
            if (closeSettingsButton != null) closeSettingsButton.SetActive(false);
        }
    }

    public void ShowUpdateProfileScreen()
    {
        previousScreen = GetActiveScreen();
        if (settingsPanel != null) settingsPanel.SetActive(false);
        DeactivateAllScreens();
        if (updateProfileScreen != null) updateProfileScreen.SetActive(true); 
    }

    public void BackToSettingsPanel()
    {
        if (updateProfileScreen != null) updateProfileScreen.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true); 
        if (previousScreen != null) previousScreen.SetActive(true);
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

    public void ShowEvaluationScreen(OptionsScreen.InitialEvaluationData evaluationData)
    {
        if (evaluationScreen == null)
        {
            Debug.LogError("EvaluationScreen no encontrado. Asegúrate de que esté en la escena y activo.");
            return;
        }

        if (optionsScreen != null) optionsScreen.gameObject.SetActive(false);
        //if (evaluationScreen != null) evaluationScreen.gameObject.SetActive(true);
        if (evaluationData != null)
        {
            evaluationScreen.ConfigureEvaluation(evaluationData);
            evaluationScreen.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("No se recibieron preguntas para la evaluación.");
        }
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

    public void OnReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void DeactivateAllScreens()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (updateProfileScreen != null) updateProfileScreen.SetActive(false);
        if (progressPanel != null) progressPanel.SetActive(false);
        if (notificationsPanel != null) notificationsPanel.SetActive(false);
        if (logoutConfirmationPanel != null) logoutConfirmationPanel.SetActive(false);
        if (quiz != null) quiz.gameObject.SetActive(false);
        if (scoreScreen != null) scoreScreen.gameObject.SetActive(false);
        if (selectTopicScreen != null) selectTopicScreen.gameObject.SetActive(false);
        if (selectLevelScreen != null) selectLevelScreen.gameObject.SetActive(false);
        if (interestSelectionScreen != null) interestSelectionScreen.gameObject.SetActive(false);
        if (selfAssessmentScreen != null) selfAssessmentScreen.gameObject.SetActive(false);
        if (optionsScreen != null) optionsScreen.gameObject.SetActive(false);
        if (evaluationScreen != null) evaluationScreen.gameObject.SetActive(false);
        if (resultsScreen != null) resultsScreen.gameObject.SetActive(false);
    }

    private GameObject GetActiveScreen()
    {
        if (quiz != null && quiz.gameObject.activeSelf) return quiz.gameObject;
        if (scoreScreen != null && scoreScreen.gameObject.activeSelf) return scoreScreen.gameObject;
        if (selectTopicScreen != null && selectTopicScreen.gameObject.activeSelf) return selectTopicScreen.gameObject;
        if (selectLevelScreen != null && selectLevelScreen.gameObject.activeSelf) return selectLevelScreen.gameObject;
        if (interestSelectionScreen != null && interestSelectionScreen.gameObject.activeSelf) return interestSelectionScreen.gameObject;
        if (selfAssessmentScreen != null && selfAssessmentScreen.gameObject.activeSelf) return selfAssessmentScreen.gameObject;
        if (optionsScreen != null && optionsScreen.gameObject.activeSelf) return optionsScreen.gameObject;
        if (evaluationScreen != null && evaluationScreen.gameObject.activeSelf) return evaluationScreen.gameObject;
        if (resultsScreen != null && resultsScreen.gameObject.activeSelf) return resultsScreen.gameObject;
        if (progressPanel != null && progressPanel.activeSelf) return progressPanel;
        if (notificationsPanel != null && notificationsPanel.activeSelf) return notificationsPanel;
        if (logoutConfirmationPanel != null && logoutConfirmationPanel.activeSelf) return logoutConfirmationPanel;
        return null; 
    }


}


