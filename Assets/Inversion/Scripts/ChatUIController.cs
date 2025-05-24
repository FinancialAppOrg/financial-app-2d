using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatUIController : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button sendButton;
    public GameObject buttonPrefab; // Prefab del bot�n a instanciar
    public Transform contentParent;
    //public TMP_Text chatDisplay;

    private GeminiAPIClient geminiClient;

    private void Start()
    {
        geminiClient = gameObject.AddComponent<GeminiAPIClient>();
        sendButton.onClick.AddListener(AskFinancialQuestion);

        //AddMessageToChat("Asistente Financiero: �Hola! Soy tu asistente de educaci�n financiera. " +
        //                "Preg�ntame sobre ahorro, inversi�n, cr�ditos o manejo de deudas.");
    }

    private void AskFinancialQuestion()
    {
        string message = inputField.text;
        if (string.IsNullOrEmpty(message)) return;

        AddMessageToChat("T�: " + message);
        inputField.text = "";

        geminiClient.AskFinancialQuestionChat(message, (response) => {
            AddMessageToChat("Asistente Financiero: " + response);
        });
    }

    private void AddMessageToChat(string message)
    {
        if (buttonPrefab != null && contentParent != null)
        {
            // Instanciar el nuevo bot�n
            GameObject newButton = Instantiate(buttonPrefab, contentParent);
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                buttonText.text = message;
            }
            else
            {
                Debug.LogError("El prefab no contiene un TMP_Text como hijo.");
            }

            // Opcional: Agregar un listener al bot�n para alguna acci�n espec�fica
            newButton.GetComponent<Button>().onClick.AddListener(() => {
                Debug.Log("Mensaje seleccionado: " + message);
            });

            // Auto-scroll al fondo
            Canvas.ForceUpdateCanvases();
            ScrollToBottom();
        }
        else
        {
            Debug.LogError("Prefab del bot�n o el contenedor no est�n asignados en el Inspector.");
        }

    }
    private void ScrollToBottom()
    {
        ScrollRect scrollRect = contentParent.GetComponentInParent<ScrollRect>(); //chatDisplay.GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            // Forzar actualizaci�n antes de ajustar la posici�n del scroll
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
