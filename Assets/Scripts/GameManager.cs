using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using TMPro;
using System.Text;

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
    public List<Question> questions = new List<Question>();

    string selectedTopic;
    private bool isQuizEnding = false;

    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private GameObject updateProfileScreen; 

    [SerializeField] private GameObject progressPanel;
    [SerializeField] private GameObject notificationsPanel;
    [SerializeField] private GameObject logoutConfirmationPanel;

    private GameObject currentPopup;
    private GameObject previousScreen;
    
    //[SerializeField] private GameObject closeSettingsButton;

    private const string LogoutUrl = "https://financeapp-backend-production.up.railway.app/api/v1/auth/logout";

    private int aciertos_totales;
    private int monedas_ganadas;

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
        if (updateProfileScreen != null) updateProfileScreen.gameObject.SetActive(false); 
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
            return; 
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
            //if (closeSettingsButton != null) 
            //{
            //    closeSettingsButton.SetActive(true); 
            //}
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
            //if (closeSettingsButton != null) closeSettingsButton.SetActive(false); 
        }
    }

    public void ShowLogoutConfirmationPanel()
    {
        CloseCurrentPopup();
        if (logoutConfirmationPanel != null)
        {
            logoutConfirmationPanel.SetActive(true);
            currentPopup = logoutConfirmationPanel;
            //if (closeSettingsButton != null) closeSettingsButton.SetActive(false);
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

    public void QuizzPanels()
    {
        selfAssessmentScreen.gameObject.SetActive(false);
        quiz.gameObject.SetActive(false);
        scoreScreen.gameObject.SetActive(false);
    }

    public void OnReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    //quizz
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{\"array\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }

    public void LoadQuestionsFromAPI(int quizzId)
    {
        StartCoroutine(GetQuestionsFromAPI(quizzId));
    }

    public IEnumerator GetQuestionsFromAPI(int quizzId)
    {
        string url = $"https://financeapp-backend-production.up.railway.app/api/v1/quizz/questions/{quizzId}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener preguntas: " + request.error);
            questions = new List<Question>(); // para evitar null
        }
        else
        {
            string json = request.downloadHandler.text;
            questions = new List<Question>(JsonHelper.FromJson<Question>(json));

            Debug.Log("Preguntas cargadas: " + questions.Count);
            //quiz.SetQuestions(questions);
            //quiz.GetNextQuestion();
        }
    }

    public void SetQuestions(List<Question> loadedQuestions)
    {
        questions = new List<Question>(loadedQuestions);
    }

    public void PostAnswerQuizz(int quizId, int questionId, int selectedOption)
    {
        StartCoroutine(PostAnswer(quizId, questionId, selectedOption));
    }

    private IEnumerator PostAnswer(int quizId, int questionId, int selectedOption)
    {
        string url = "https://financeapp-backend-production.up.railway.app/api/v1/quizz/answer";
        Debug.Log("Enviando POST a: " + url);

        AnswerRequest answer = new AnswerRequest
        {
            id_quizz = quizId,
            id_pregunta = questionId,
            opcion_elegida = selectedOption
        };
        string jsonData = JsonUtility.ToJson(answer);
        Debug.Log("Payload enviado: " + jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("accept", "application/json");

            Debug.Log("Enviando answer: " + jsonData);

            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Answer enviada con éxito. Respuesta: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error al enviar Answer: " + request.responseCode + " - " + request.error);
            }

        }
    }

    public void HandleQuizCompleted()
    {
        // Evitar múltiples ejecuciones
        if (isQuizEnding)
        {
            Debug.Log("HandleQuizCompleted ya está en proceso, ignorando llamada adicional");
            return;
        }

        isQuizEnding = true;
        Debug.Log("Iniciando finalización del quiz");

        int quizId = PlayerPrefs.GetInt("quizz_id", 0);

        if (quiz != null) quiz.gameObject.SetActive(false);

        if (quizId > 0)
        {
            Debug.Log($"Finalizando quiz con ID: {quizId}");
            EndQuizz(quizId);
        }
        else
        {
            Debug.LogWarning("No se encontró ID de quiz válido para finalizar");
        }

        if (scoreScreen != null)
        {
            scoreScreen.gameObject.SetActive(true);
            scoreScreen.ShowFinalScore();
        }
    }

    public void ResetQuizState()
    {
        isQuizEnding = false;
        Debug.Log("Estado del quiz reseteado");
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

    // Notificaciones

    public void ToggleDailyNotifications(bool isEnabled)
    {
        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.EnableDailyNotifications(isEnabled);
        }
    }

    public void ToggleInactivityNotifications(bool isEnabled)
    {
        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.EnableInactivityNotifications(isEnabled);
        }
    }

    // Cerrar sesión

    public void Logout()
    {
        StartCoroutine(LogoutCoroutine());
    }

    

    private IEnumerator LogoutCoroutine()
    {
        //string accessToken = PlayerPrefs.GetString("access_token", "");
        //if (string.IsNullOrEmpty(accessToken))
        //{
        //    Debug.LogError("No se encontró el token de acceso.");
        //    yield break;
        //}
        //Debug.Log("Token de acceso: " + accessToken);
        //
        //using (UnityWebRequest request = UnityWebRequest.Post(LogoutUrl, ""))
        //{
        //    request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        //    request.SetRequestHeader("Content-Type", "application/json");
        //
        //    yield return request.SendWebRequest();
        //
        //    if (request.result == UnityWebRequest.Result.Success)
        //    {
        //        Debug.Log("Cierre de sesión exitoso.");
        //        PlayerPrefs.DeleteKey("access_token");
        //        PlayerPrefs.Save();
        //PlayerData.ClearUserData();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("AuthScene");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameObject signInScreen = GameObject.Find("SignInScreen");
        if (signInScreen != null)
        {
            signInScreen.SetActive(true);
        }
        else
        {
            Debug.LogError("No se encontró el Canvas de SignInScreen en la escena AuthScene.");
        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Error al cerrar sesión: " + request.error);
        //        Debug.LogError("Respuesta del servidor: " + request.downloadHandler.text);
        //    }
        //}
    }

    public void ConfirmLogout()
    {
        Logout(); 
    }


    public void InitializeNewQuiz(List<Question> questions)
    {
        ResetQuizState(); // Resetear estado antes de inicializar

        if (quiz != null)
        {
            quiz.Init(questions);
        }
    }

    private IEnumerator HandleQuizCompletedAsync(int quizId)
    {
        if (quizId > 0)
        {
            Debug.Log($"Finalizando quiz con ID: {quizId}");
            yield return StartCoroutine(PostEndQuizz(quizId));
        }
        else
        {
            Debug.LogWarning("No se encontró ID de quiz válido para finalizar");
        }

        // Mostrar pantalla de resultados después de finalizar el quiz
        if (scoreScreen != null)
        {
            scoreScreen.gameObject.SetActive(true);
            scoreScreen.ShowFinalScore();
        }
    }

    // Modificar el método EndQuizz para que no inicie una nueva corrutina
    public IEnumerator EndQuizzCoroutine(int quizId)
    {
        return PostEndQuizz(quizId);
    }
    public void EndQuizz(int quizId)
    {
        StartCoroutine(PostEndQuizz(quizId));
    }
    private IEnumerator PostEndQuizz(int quizId)
    {
        string url = "https://financeapp-backend-production.up.railway.app/api/v1/quizz/end";

        var jsonData = JsonConvert.SerializeObject(new { id_quizz = quizId });

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Enviando solicitud de End Quizz: " + jsonData);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                //Debug.Log("Quizz finalizado exitosamente.");
                aciertos_totales = int.Parse(responseData["preguntas_correctas"].ToString());
                PlayerPrefs.SetInt("quizz_aciertos", aciertos_totales);
                PlayerPrefs.Save();
                monedas_ganadas = int.Parse(responseData["monedas_ganadas"].ToString());
                PlayerPrefs.SetInt("quizz_monedas", monedas_ganadas);
                PlayerPrefs.Save();
                Debug.Log("Quizz finalizado exitosamente." + " " + aciertos_totales + " " + monedas_ganadas);

            }
            else
            {
                Debug.LogError("Error al finalizar el quizz: " + request.responseCode + " - " + request.error);
                Debug.LogError("Respuesta del servidor: " + request.downloadHandler.text); 
            }
        }
    }
}

