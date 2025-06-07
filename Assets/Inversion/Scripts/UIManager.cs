using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class UIManager : MonoBehaviour
{
    public gameManager gameManager;
    public GeminiAPIClient geminiClient;
    public TextMeshProUGUI bienvenidaText;
    public TextMeshProUGUI retroalimentacionFinalText;

    private string backendUrl = "https://financeapp-backend-production.up.railway.app/api/v1"; 

    void Start()
    {
    }

    public void CreateGameButtonClick()
    {
        int userId = PlayerData.GetUserId();
        string temaSeleccionado = PlayerData.GetSelectedTopic();
        string nivelSeleccionado = PlayerData.GetSelectedLevel();
        //int userId = 1;//PlayerPrefs.GetInt("userId", 0);
        gameManager.StartGame(userId, temaSeleccionado, nivelSeleccionado, 1200);
        Debug.Log("Creando juego para el usuario con ID: " + userId);
        Debug.Log("Tema seleccionado: " + temaSeleccionado);

    }

    public void CreateQuizzButtonClick()
    {
        int gameId = PlayerPrefs.GetInt("gameId", 0);
        int userId = PlayerData.GetUserId();
        //int userId = 1;//PlayerPrefs.GetInt("user_id", 0);
        Debug.Log("gameId: " + gameId + ", userId: " + userId);
        gameManager.StartQuizz(gameId, userId, 2);
    }
    
    public void ShowWelcomeAssistant()
    {
        if (geminiClient == null)
        {
            Debug.LogError("No se pudo acceder al asistente");
            return;
        }

        Debug.Log("Solicitando explicación al asistente...");
        geminiClient.AssistantGuide(
            (response) =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    Debug.LogWarning("La respuesta del asistente está vacía o es nula.");
                }
                else
                {
                    bienvenidaText.text = response;
                    Debug.Log("Respuesta del asistente recibida: " + response);
                }
            }
        );
    }

    public void ShowFeedbackAssistant()
    {
        if (geminiClient == null)
        {
            Debug.LogError("No se pudo acceder al asistente");
            return;
        }

        Debug.Log("Solicitando explicación al asistente...");
        geminiClient.AssistantFeedback(
            (response) =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    Debug.LogWarning("La respuesta del asistente está vacía o es nula.");
                }
                else
                {
                    retroalimentacionFinalText.text = response;
                    Debug.Log("Respuesta del asistente recibida: " + response);
                }
            }
        );
    }

    public void ShowFeedbackAssistant2(int idJuego)
    {
        StartCoroutine(GetGameData(idJuego, (gameData) =>
        {
            if (geminiClient == null)
            {
                Debug.LogError("No se pudo acceder al asistente");
                return;
            }

            Debug.Log("Solicitando retroalimentación al asistente...");
            geminiClient.AssistantFeedback3(gameData, (response) =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    Debug.LogWarning("La respuesta del asistente está vacía o es nula.");
                }
                else
                {
                    retroalimentacionFinalText.text = response;
                    Debug.Log("Respuesta del asistente recibida: " + response);
                }
            });
        }));
    }

    private IEnumerator GetGameData(int idJuego, System.Action<string> callback)
    {
        string url = $"{backendUrl}/game/{idJuego}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string gameData = request.downloadHandler.text;
            Debug.Log("Datos del juego obtenidos: " + gameData);
            callback?.Invoke(gameData);
        }
        else
        {
            Debug.LogError("Error al obtener los datos del juego: " + request.error);
        }
    }
}
