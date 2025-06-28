using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;

public class OptionsScreen : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button evaluationButton;
    [SerializeField] GameControllerBridge gameControllerBridge;
    [SerializeField] GameManager gameManager;
    [SerializeField] EvaluationScreen evaluationScreen;

    [Header("Topic Text")]
    [SerializeField] TMP_Text topicText;


    void Start()
    {
        //topicText.text = PlayerData.GetSelectedTopic();

        //ShowMessage(PlayerData.GetSelectedTopic());
        playButton.onClick.AddListener(CargarJuegoInversion);
        evaluationButton.onClick.AddListener(OnEvaluationClicked);
    }

    void OnEnable()
    {
        // Esto sí se ejecuta cada vez que se activa la pantalla
        string selectedTopic = PlayerData.GetSelectedTopic();
        topicText.text = selectedTopic;
        Debug.Log("Tema actualizado al activar la pantalla: " + selectedTopic);
    }

    void OnPlayClicked()
    {
        FindObjectOfType<GameManager>().ShowSelectTopicScreen();
    }

    void OnEvaluationClicked()
    {
        StartCoroutine(IniciarEvaluacionInicial());
        //FindObjectOfType<GameManager>().ShowEvaluationScreen();
    }

    private void ShowMessage(string message)
    {
        topicText.text = message;
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

    IEnumerator IniciarEvaluacionInicial()
    {
        string temaSeleccionado = PlayerData.GetSelectedTopic();
        int evaluationId = PlayerData.GetEvaluationId();
        Debug.Log("temaSeleccionado"+ temaSeleccionado);
        Debug.Log("evaluationId" + evaluationId);
        if (evaluationId <= 0)
        {
            Debug.LogError("ID de evaluación no válido.");
            yield break;
        }

        string url = "https://financeapp-backend-production.up.railway.app/api/v1/initial-evaluation/with-questions/" + evaluationId;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Datos de evaluación inicial obtenidos: " + request.downloadHandler.text);

                InitialEvaluationData evaluationData = JsonConvert.DeserializeObject<InitialEvaluationData>(request.downloadHandler.text);

                if (evaluationData?.preguntas == null || evaluationData.preguntas.Count == 0)
                {
                    Debug.LogError("No se recibieron preguntas válidas para la evaluación.");
                    yield break;
                }

                foreach (var pregunta in evaluationData.preguntas)
                {
                    Debug.Log($"Pregunta: {pregunta.texto_pregunta}");
                    Debug.Log($"Opciones: {string.Join(", ", pregunta.opciones)}");
                }

                if (evaluationScreen == null)
                    evaluationScreen = FindObjectOfType<EvaluationScreen>();

                if (gameManager == null)
                    gameManager = FindObjectOfType<GameManager>();

                if (evaluationScreen == null || gameManager == null)
                {
                    Debug.LogError("Componentes esenciales no encontrados.");
                    yield break;
                }

                evaluationScreen.ConfigureEvaluation(evaluationData);
                gameManager.ShowEvaluationScreen(evaluationData);      
            }
            else
            {
                Debug.LogError($"Error al obtener evaluación: {request.error}\nRespuesta: {request.downloadHandler.text}");
                Debug.LogError("Respuesta del servidor: " + request.downloadHandler.text);
            }
        }
    }

    [System.Serializable]
    public class InitialEvaluationData
    {
        public int id_evaluacion;
        public int id_usuario;
        public string tema;
        public int nivel_autoevaluado;
        public int total_preguntas_correctas;
        public int total_preguntas_incorrectas;
        public float calificacion_final;
        public string nivel_determinado;
        public string fecha_evaluacion;
        public List<QuestionData> preguntas;
    }
}