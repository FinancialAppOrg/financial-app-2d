using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlossaryScreenManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button searchButton; 
    public TMP_Text resultText; 
    public Button backButton; 
    public GeminiAPIClient geminiAPIClient; 

    private void Start()
    {
        searchButton.onClick.AddListener(OnSearchButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnSearchButtonClicked()
    {
        string term = inputField.text.Trim();

        if (string.IsNullOrEmpty(term))
        {
            resultText.text = "Por favor, ingresa un término financiero.";
            return;
        }

        string question = $"Explica el concepto de '{term}' y proporciona un ejemplo práctico. Que sea máximo de 400 caracteres";
        geminiAPIClient.AskFinancialQuestionChat(question, OnResponseReceived);
    }

    private void OnResponseReceived(string response)
    {
        resultText.text = response.Trim();
    }

    private void OnBackButtonClicked()
    {
        gameObject.SetActive(false);
    }
}
