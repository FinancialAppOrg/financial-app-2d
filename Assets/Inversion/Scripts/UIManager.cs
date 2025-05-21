using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public gameManager gameManager;
    public GeminiAPIClient geminiClient;
    public int userId = 1;
    public TextMeshProUGUI bienvenidaText;
    public TextMeshProUGUI retroalimentacionFinalText;

    void Start()
    {
    }

    public void CreateGameButtonClick()
    {
        gameManager.StartGame(userId, "inversion", "basico", 1200);
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
