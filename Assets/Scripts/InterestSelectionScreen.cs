using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class InterestSelectionScreen : MonoBehaviour
{
    [SerializeField] Button ahorroButton;
    [SerializeField] Button inversionButton;
    [SerializeField] Button creditoDeudasButton;

    GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        ahorroButton.onClick.AddListener(() => OnTopicSelected("ahorro"));
        inversionButton.onClick.AddListener(() => OnTopicSelected("inversion"));
        creditoDeudasButton.onClick.AddListener(() => OnTopicSelected("credito-deudas"));
    }

    void OnTopicSelected(string temaSeleccionado)
    {
        PlayerData.SetSelectedTopic(temaSeleccionado);
        //int nivelAutoevaluado = PlayerData.GetKnowledge(temaSeleccionado);

        StartCoroutine(SendSelectedTopic(temaSeleccionado));
    }

    IEnumerator SendSelectedTopic(string temaSeleccionado)
    {
        if (PlayerData.GetUserId() <= 0)
        {
            Debug.LogError("ID de usuario no válido.");
            yield break;
        }

        if (string.IsNullOrEmpty(temaSeleccionado))
        {
            Debug.LogError("Tema seleccionado no válido.");
            yield break;
        }

        string jsonPayload = $"{{\"id_usuario\": {PlayerData.GetUserId()}, \"tema\": \"{temaSeleccionado}\"}}";

        using (UnityWebRequest request = new UnityWebRequest("http://127.0.0.1:8000/api/v1/select-topic", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Tema seleccionado guardado correctamente: " + request.downloadHandler.text);

                int nivelAutoevaluado = PlayerData.GetKnowledge(temaSeleccionado);
                StartCoroutine(SendSelfAssessmentData(temaSeleccionado, nivelAutoevaluado));
            }
            else
            {
                Debug.LogError("Error al guardar el tema seleccionado: " + request.error);
                Debug.LogError("Respuesta del servidor: " + request.downloadHandler.text);
            }
        }
    }

    IEnumerator SendSelfAssessmentData(string temaSeleccionado, int nivelAutoevaluado)
    {
        var payload = new
        {
            id_usuario = PlayerData.GetUserId(),
            nivel_autoevaluado = nivelAutoevaluado,
            user_answers = new List<object>()
        };

        string jsonPayload = JsonConvert.SerializeObject(payload); //JsonUtility.ToJson(payload); 
        Debug.Log($"Payload enviado: {jsonPayload}");

        using (UnityWebRequest request = new UnityWebRequest("http://127.0.0.1:8000/api/v1/initial-evaluation", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Nivel autoevaluado guardado correctamente: " + request.downloadHandler.text);

                InitialEvaluationResponse response = JsonUtility.FromJson<InitialEvaluationResponse>(request.downloadHandler.text);
                if (response != null)
                {
                    PlayerData.SetEvaluationId(response.id_evaluacion); 
                }

                //JsonConvert.DeserializeObject<InitialEvaluationResponse>(request.downloadHandler.text);

                if (gameManager != null)
                {
                    gameManager.ShowOptionsScreen();
                }
                else
                {
                    Debug.LogError("GameManager no encontrado.");
                }
            }
            else
            {
                Debug.LogError("Error al guardar el nivel autoevaluado: " + request.error);
                Debug.LogError("Respuesta del servidor: " + request.downloadHandler.text); 

            }
        }
    }

    [System.Serializable]
    public class InitialEvaluationResponse
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
    }
}
