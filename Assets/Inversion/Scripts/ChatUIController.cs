using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatUIController : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button sendButton;
    public GameObject buttonPrefab; 
    public Transform contentParent;
    //public TMP_Text chatDisplay;

    private GeminiAPIClient geminiClient;

    private void Start()
    {
        geminiClient = gameObject.AddComponent<GeminiAPIClient>();
        sendButton.onClick.AddListener(AskFinancialQuestion);

        //AddMessageToChat("Asistente Financiero: ¡Hola! Soy tu asistente de educación financiera. " +
        //                "Pregúntame sobre ahorro, inversión, créditos o manejo de deudas.");
    }

    private void AskFinancialQuestion()
    {
        string message = inputField.text;
        if (string.IsNullOrEmpty(message)) return;

        AddMessageToChat("Tú: " + message);
        inputField.text = "";

        geminiClient.AskFinancialQuestionChat(message, (response) => {
            AddMessageToChat("Asistente Financiero: " + response);
        });
    }

    private void AddMessageToChat(string message)
    {
        if (buttonPrefab != null && contentParent != null)
        {
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

            newButton.GetComponent<Button>().onClick.AddListener(() => {
                Debug.Log("Mensaje seleccionado: " + message);
            });

            Canvas.ForceUpdateCanvases();
            ScrollToBottom();
        }
        else
        {
            Debug.LogError("Prefab del botón o el contenedor no están asignados en el Inspector.");
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
        StartCoroutine(ScrollNextFrame());

    }
    private void ScrollToBottom()
    {
        ScrollRect scrollRect = contentParent.GetComponentInParent<ScrollRect>(); //chatDisplay.GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
    private IEnumerator ScrollNextFrame()
    {
        ScrollRect scrollRect = contentParent.GetComponentInParent<ScrollRect>();

        yield return null;
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
