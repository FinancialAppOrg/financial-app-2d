using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class EvaluationScreen : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] TextMeshProUGUI questionText;
    //[SerializeField] List<QuestionSO> evaluationQuestions = new List<QuestionSO>();
    List<QuestionData> questions = new List<QuestionData>();
    QuestionData currentQuestion;

    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;

    [Header("Scoring")]
    ScoreKeeper scoreKeeperr;

    [Header("Results Screen")]
    [SerializeField] ResultsScreen resultsScreen;

    void Awake()
    {
        scoreKeeperr = FindObjectOfType<ScoreKeeper>();
        if (scoreKeeperr == null)
        {
            Debug.LogError("No se encontr� ScoreKeeper.");
        }
    }

    void Start()
    {
        if (questions == null || questions.Count == 0)
        {
            Debug.Log("Esperando configuraci�n de preguntas...");
            return;
        }

        //LoadNextEvaluationQuestion();
    }

    void Update() {}

    public void ConfigureEvaluation(OptionsScreen.InitialEvaluationData evaluationData)
    {
        if (evaluationData == null || evaluationData.preguntas == null || evaluationData.preguntas.Count == 0)
        {
            Debug.LogError("No se recibieron preguntas para la evaluaci�n.");
            return;
        }

        List<QuestionData> preguntasValidas = new List<QuestionData>();

        foreach (var pregunta in evaluationData.preguntas)
        {
            if (!string.IsNullOrEmpty(pregunta.opcion_1) &&
                !string.IsNullOrEmpty(pregunta.opcion_2) &&
                !string.IsNullOrEmpty(pregunta.opcion_3) &&
                !string.IsNullOrEmpty(pregunta.opcion_4))
            {
                preguntasValidas.Add(pregunta);
            }
            else
            {
                Debug.LogError($"Pregunta con opciones incompletas: {pregunta.texto_pregunta}");
            }
        }

        if (preguntasValidas.Count == 0)
        {
            Debug.LogError("No hay preguntas v�lidas para la evaluaci�n.");
            return;
        }

        questions = preguntasValidas;
        Debug.Log($"Preguntas v�lidas cargadas: {questions.Count}");
        LoadNextEvaluationQuestion(); 
    }


    public void OnEvaluationAnswerSelected(int index)
    {
        //Debug.Log($"Respuesta seleccionada - �ndice (Unity): {index}");
        Debug.Log($"Opci�n seleccionada: {(currentQuestion.opciones != null && index < currentQuestion.opciones.Count ? currentQuestion.opciones[index] : "INV�LIDA")}");

        int backendIndex = index + 1;
        //Debug.Log($"Respuesta convertida - �ndice (Backend): {backendIndex}");

        Debug.Log($"Opci�n correcta index: {currentQuestion.opcion_correcta}");
       //Debug.Log($"Opci�n correcta texto: {(currentQuestion.opciones != null && currentQuestion.opcion_correcta < currentQuestion.opciones.Count ? currentQuestion.opciones[currentQuestion.opcion_correcta] : "INV�LIDA")}");

        if (currentQuestion == null)
        {
            Debug.LogError("currentQuestion es null.");
            return;
        }

        StartCoroutine(SendAnswerToBackend(backendIndex));

        ShowEvaluationAnswer(index);
        SetEvaluationButtonState(false);
    }

    void ShowEvaluationAnswer(int index)
    {
        if (currentQuestion == null)
        {
            Debug.LogError("Pregunta actual no v�lida");
            return;
        }

        //int indiceCorrectoBackend = currentQuestion.opcion_correcta; 
        int indiceCorrectoUnity = currentQuestion.opcion_correcta - 1;
        bool isCorrect = (index == indiceCorrectoUnity);

        if (isCorrect)
        {
            questionText.text = "�Correcto!";
            scoreKeeperr.IncrementCorrectAnswers();
        }
        else
        {
            if (indiceCorrectoUnity >= 0 && indiceCorrectoUnity < currentQuestion.opciones.Count)
            {
                questionText.text = $"Respuesta correcta:\n{currentQuestion.opciones[indiceCorrectoUnity]}";
            }
            else
            {
                questionText.text = "Respuesta incorrecta (no se pudo determinar la correcta)";
            }
        }

        if (indiceCorrectoUnity >= 0 && indiceCorrectoUnity < answerButtons.Length)
        {
            Image buttonImage = answerButtons[indiceCorrectoUnity].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = correctAnswerSprite;
            }
        }

        StartCoroutine(WaitAndLoadNextQuestion(2f));
    }

    IEnumerator WaitAndLoadNextQuestion(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadNextEvaluationQuestion();
    }

    void LoadNextEvaluationQuestion()
    {
        if (questions.Count > 0)
        {
            SetEvaluationButtonState(true);
            SetDefaultEvaluationButtonSprites();
            GetNextEvaluationQuestion();
            DisplayEvaluationQuestion();
            scoreKeeperr.IncrementQuestionsSeen();
        }
        else
        {
            Debug.Log("Evaluaci�n completada. Preguntas contestadas: " + scoreKeeperr.GetQuestionsSeen());
            //ShowEvaluationResultsScreen();
            StartCoroutine(FinishEvaluation());
            Debug.Log("FinishEvaluation");
        }
    }

    void GetNextEvaluationQuestion()
    {
        if (questions.Count == 0)
        {
            Debug.LogError("No hay m�s preguntas disponibles.");
            return;
        }

        Debug.Log($"Obteniendo pregunta. Total antes: {questions.Count}");
        currentQuestion = questions[0];
        questions.RemoveAt(0);
        Debug.Log($"Pregunta actual: {currentQuestion.texto_pregunta}. Total despu�s: {questions.Count}");

        if (currentQuestion.opciones == null || currentQuestion.opciones.Count < 4)
        {
            //Debug.LogError($"Pregunta con opciones inv�lidas: {currentQuestion.texto_pregunta}");
            Debug.Log($"Opciones: {string.Join(", ", currentQuestion.opciones)}");
  
            LoadNextEvaluationQuestion();
            return;
        }

        //Debug.Log("Opciones de la pregunta actual:");
        for (int i = 0; i < currentQuestion.opciones.Count; i++)
        {
            Debug.Log($"{i + 1}. {currentQuestion.opciones[i]}");
        }
    }

    void GetRandomEvaluationQuestion()
    {
        if (questions.Count == 0)
        {
            Debug.LogError("No hay m�s preguntas disponibles.");
            return;
        }

        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];
        questions.RemoveAt(index);
    }

    void DisplayEvaluationQuestion()
    {
        if (currentQuestion == null)
        {
            Debug.LogError("Pregunta actual no v�lida");
            return;
        }

        Debug.Log($"Mostrando pregunta: {currentQuestion.texto_pregunta}");
        questionText.text = currentQuestion.texto_pregunta;

        foreach (var button in answerButtons)
        {
            button.SetActive(false);
        }

        var opciones = currentQuestion.opciones;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].SetActive(false);

            if (i < opciones.Count)
            {
                TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = opciones[i];
                    answerButtons[i].SetActive(true);
                }
            }
        }
    }

    void SetEvaluationButtonState(bool state)
    {
        foreach (var button in answerButtons)
        {
            Button btn = button.GetComponent<Button>();
            if (btn != null)
            {
                btn.interactable = state;
            }
        }
    }

    void SetDefaultEvaluationButtonSprites()
    {
        foreach (var button in answerButtons)
        {
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = defaultAnswerSprite;
            }
        }
    }

    IEnumerator SendAnswerToBackend(int userAnswer)
    {
        int evaluationId = PlayerData.GetEvaluationId();
        if (evaluationId <= 0)
        {
            Debug.LogError("ID de evaluaci�n no v�lido.");
            yield break;
        }

        //Debug.Log($"Enviando respuesta para evaluationId: {evaluationId}, id_pregunta: {currentQuestion.id_pregunta}, respuesta: {userAnswer}");
        Debug.Log($"Enviando respuesta para evaluationId: {evaluationId}, id_pregunta: {currentQuestion.id_pregunta}, respuesta (backend index): {userAnswer}");

        string url = $"https://financeapp-backend-production.up.railway.app/api/v1/initial-evaluation/{evaluationId}/answer";

        Dictionary<string, object> payload = new Dictionary<string, object>
        {
            {"id_pregunta", currentQuestion.id_pregunta},
            {"respuesta", userAnswer}
        };

        string jsonPayload = JsonConvert.SerializeObject(payload);
        Debug.Log($"Payload enviado: {jsonPayload}");

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

        
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Respuesta registrada correctamente: " + request.downloadHandler.text);
                EvaluationResults results = JsonUtility.FromJson<EvaluationResults>(request.downloadHandler.text);
                Debug.Log($"Resultados actualizados: Correctas: {results.total_preguntas_correctas}, Incorrectas: {results.total_preguntas_incorrectas}, Calificaci�n: {results.calificacion_final}, Nivel: {results.nivel_determinado}");
                
                scoreKeeperr.UpdateScore(results.calificacion_final);
                scoreKeeperr.UpdateCorrectAnswers(results.total_preguntas_correctas);

                if (currentQuestion == null || currentQuestion.id_pregunta <= 0)
                {
                    Debug.LogError("Pregunta actual no v�lida o ID de pregunta no definido.");
                    yield break;
                }

                if (scoreKeeperr.GetQuestionsSeen() >= 5)
                {
                    Debug.Log($"Nivel actualizado a: {results.nivel_determinado}");
                    PlayerData.SetSelectedLevel(results.nivel_determinado); // Aseg�rate de que PlayerData tenga un m�todo SetSelectedLevel
                    //PlayerData.SetLevel(results.nivel_determinado); // Aseg�rate de que PlayerData tenga un m�todo SetLevel
                }

                Debug.Log($"Enviando respuesta - EvaluationID: {evaluationId}, PreguntaID: {currentQuestion.id_pregunta}, Respuesta: {userAnswer}");
                // Debug.Log($"URL completa: {url}");
            }
            else
            {
                //Debug.LogError($"Error al registrar la respuesta: {request.error}");
                //Debug.LogError($"C�digo de estado: {request.responseCode}");
                Debug.LogError("Respuesta del servidor: " + request.downloadHandler.text);
            }
        }
    }

    //End

    private IEnumerator FinishEvaluation()
    {
        int evaluationId = PlayerData.GetEvaluationId();
        if (evaluationId <= 0)
        {
            Debug.LogError("ID de evaluaci�n no v�lido para finalizar.");
            yield break;
        }
        string url = $"https://financeapp-backend-production.up.railway.app/api/v1/initial-evaluation/{evaluationId}/finish";

        var jsonData = JsonConvert.SerializeObject(new { evaluation_id = evaluationId });

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Enviando solicitud de End Evaluacion: " + jsonData);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                PlayerData.SetEvaluationCompleted(response["tema"].ToString(), true);
                PlayerData.SetUserLevel(response["tema"].ToString(), response["nivel_determinado"].ToString());

                bool verificacion = PlayerData.GetEvaluationCompleted(response["tema"].ToString());
                string nivelVerificacion = PlayerData.GetUserLevel(response["tema"].ToString());
                Debug.Log($"Evaluaci�n completada para tema: {response["tema"].ToString()},VERIFICACI�N: Tema '{response["tema"].ToString()}' - Completado: {verificacion}, Nivel: '{nivelVerificacion}'");

                ShowEvaluationResultsScreen();
            }
            else
            {
                Debug.LogError("Error al finalizar el juego: " + request.error);
            }
        }
    }



    [System.Serializable]
    public class QuestionList
    {
        public List<QuestionData> preguntas;
    }

    [System.Serializable]
    public class EvaluationFinishResponse
    {
        public int id_evaluacion;
        public int id_usuario;
        public string tema;
        public float calificacion_final;
        public string nivel_determinado;
    }
    /*
    void ShowEvaluationResultsScreen()
    {
        // Cambiar el orden: primero finalizar, luego mostrar resultados
        StartCoroutine(FinishEvaluationAndShowResults());
    }

    IEnumerator FinishEvaluationAndShowResults()
    {
        // Finalizar la evaluaci�n en el backend PRIMERO
        yield return StartCoroutine(FinishEvaluation());

        // DESPU�S mostrar los resultados
        resultsScreen.gameObject.SetActive(true);
        resultsScreen.DisplayResults(scoreKeeperr.CalculateScore());
        resultsScreen.DisplayCorrectAnswers(scoreKeeperr.GetCorrectAnswers());
        FindObjectOfType<GameManager>().ShowResultsScreen();
    }*/
    /*
    void ShowEvaluationResultsScreen()
    {
        // Finalizar la evaluaci�n en el backend
        StartCoroutine(FinishEvaluation());

        resultsScreen.gameObject.SetActive(true);
        resultsScreen.DisplayResults(scoreKeeperr.CalculateScore());
        resultsScreen.DisplayCorrectAnswers(scoreKeeperr.GetCorrectAnswers());
        FindObjectOfType<GameManager>().ShowResultsScreen();
    }*/

    
    void ShowEvaluationResultsScreen()
    {
        resultsScreen.gameObject.SetActive(true);

        resultsScreen.DisplayResults(scoreKeeperr.CalculateScore());

        resultsScreen.DisplayCorrectAnswers(scoreKeeperr.GetCorrectAnswers());

        FindObjectOfType<GameManager>().ShowResultsScreen();
    }

    [System.Serializable]
    public class EvaluationResults
    {
        public int total_preguntas_correctas;
        public int total_preguntas_incorrectas;
        public float calificacion_final;
        public string nivel_determinado;
    }
}
