using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class ForgotPasswordScreen : MonoBehaviour
{
    [Header("User Information")]
    [SerializeField] TMP_InputField emailField;

    [Header("Submit Button")]
    [SerializeField] Button submitButton;

    [Header("Message Text")]
    [SerializeField] TMP_Text messageText;


    private string forgotPasswordUrl = "http://127.0.0.1:8000/api/v1/auth/forgot-password";

    private void Start()
    {
        submitButton.onClick.AddListener(OnSubmitForgotPassword);
    }

    public void ShowForgotPasswordScreen(bool show)
    {
        gameObject.SetActive(show);

        if (show)
        {
            // Limpia el campo de correo y el mensaje
            emailField.text = string.Empty;
            ShowMessage(string.Empty, Color.black);
        }
    }

    public void OnBackToLoginClick()
    {
        gameObject.SetActive(false);
        // Aquí puedes activar el panel de inicio de sesión si es necesario
    }

    private void OnSubmitForgotPassword()
    {
        if (string.IsNullOrEmpty(emailField.text))
        {
            ShowMessage("Email is required.", Color.red);
            return;
        }

        ForgotPasswordData forgotPasswordData = new ForgotPasswordData
        {
            email = emailField.text
        };

        StartCoroutine(SendForgotPasswordRequest(forgotPasswordData));
    }

    private IEnumerator SendForgotPasswordRequest(ForgotPasswordData forgotPasswordData)
    {
        string jsonData = JsonUtility.ToJson(forgotPasswordData);

        UnityWebRequest request = new UnityWebRequest(forgotPasswordUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowMessage("Verification code sent to your email.", Color.green);
        }
        else
        {
            ShowMessage("Error: " + request.error, Color.red);
        }
    }

    private void ShowMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
    }
}

[System.Serializable]
public class ForgotPasswordData
{
    public string email;
}