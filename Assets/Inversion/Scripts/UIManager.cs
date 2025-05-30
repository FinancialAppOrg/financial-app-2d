using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public gameManager gameManager;
    public GeminiAPIClient geminiClient;
    public TextMeshProUGUI bienvenidaText;
    public TextMeshProUGUI retroalimentacionFinalText;

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
}
