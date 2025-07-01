using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class ProgresoPanelController : MonoBehaviour
{
    public TextMeshProUGUI juegosCompletadosLabel;
    public TextMeshProUGUI monedasColeccionadasLabel;
    public TextMeshProUGUI recomendacionLabel;
    public GeminiAPIClient geminiClient;

    private string progressEndpoint = "https://financeapp-backend-production.up.railway.app/api/v1/progress/history";
    private string progressEndpoint2 = "https://financeapp-backend-production.up.railway.app/api/v1/progress";

    void Start()
    {
        StartCoroutine(FetchProgressData());
        StartCoroutine(FetchProgressData2());
    }

    void Awake()
    {
        if (geminiClient == null)
        {
            GameObject geminiObject = new GameObject("GeminiAPIClient");
            geminiClient = geminiObject.AddComponent<GeminiAPIClient>();
        }
    }


    private IEnumerator FetchProgressData()
    {
        int userId = PlayerData.GetUserId();
        string url = $"{progressEndpoint}/{userId}";
        Debug.Log("Fetching progress data from: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Datos de progreso recibidos: " + jsonResponse);

                SendDataToAssistant(jsonResponse);
            }
            else
            {
                Debug.LogError("Error al obtener datos de progreso: " + request.error);
            }
        }
    }

    private IEnumerator FetchProgressData2()
    {
        int userId = PlayerData.GetUserId();
        string url = $"{progressEndpoint2}/{userId}";
        Debug.Log("Fetching progress 2 data from: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Datos de progreso recibidos: " + jsonResponse);

                UpdateLabels(jsonResponse);
            }
            else
            {
                Debug.LogError("Error al obtener datos de progreso: " + request.error);
            }
        }
    }

    private void UpdateLabels(string jsonResponse)
    {
        ProgressResponse progressData = JsonUtility.FromJson<ProgressResponse>(jsonResponse);

        juegosCompletadosLabel.text = $"{progressData.juegos_completados} Juegos Completados";
        monedasColeccionadasLabel.text = $"{progressData.monedas_totales} Monedas Coleccionadas";
    }

    private void SendDataToAssistant(string jsonResponse)
    {
        if (geminiClient == null)
        {
            Debug.LogError("GeminiAPIClient no está asignado.");
            return;
        }

        geminiClient.GenerateGeneralRecommendation(jsonResponse, (response) =>
        {
            if (!string.IsNullOrEmpty(response))
            {
                recomendacionLabel.text = response;
                Debug.Log("Recomendación del asistente recibida: " + response);
            }
            else
            {
                recomendacionLabel.text = "No se pudo generar una recomendación.";
            }
        });
    }

    [System.Serializable]
    public class ProgressData
    {
        public string tema;
        public float? evaluacion;
        public float? juego;
        public float? quizz;
    }

    [System.Serializable]
    public class ProgressDataWrapper
    {
        public ProgressData[] progress;
    }

    [System.Serializable]
    public class ProgressResponse
    {
        public int id_usuario;
        public int juegos_completados;
        public int aciertos_totales;
        public int monedas_totales;
        public float mejora_porcentual;
        public string clasificacion;
        public string fecha_actualizacion;
    }
}
