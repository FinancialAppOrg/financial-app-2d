using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class EnterCodeScreen : MonoBehaviour
{
    [Header("User Information")]
    [SerializeField] TMP_InputField codeField; 

    [Header("Verify Button")]
    [SerializeField] Button verifyButton; 

    [Header("Message Text")]
    [SerializeField] TMP_Text messageText; 

    [Header("Back Button")]
    [SerializeField] Button backButton;

    [Header("Enter Code Screen")]
    [SerializeField] GameObject resetPasswordScreen;

    private string verifyCodeUrl = "http://127.0.0.1:8000/api/v1/auth/verify_code"; 

    private void Start()
    {
        verifyButton.onClick.AddListener(OnVerifyCode);

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackToForgotPasswordClick);
        }
    }

    public void ShowEnterCodeScreen(bool show)
    {
        gameObject.SetActive(show);

        if (show)
        {
            codeField.text = string.Empty;
            ShowMessage(string.Empty, Color.black);
        }
    }

    private void OnBackToForgotPasswordClick()
    {
        gameObject.SetActive(false);
    }

    private void OnVerifyCode()
    {
        if (string.IsNullOrEmpty(codeField.text))
        {
            ShowMessage("Code is required.", Color.red);
            return;
        }

        string userId = PlayerPrefs.GetString("user_id", null);
        if (string.IsNullOrEmpty(userId))
        {
            ShowMessage("User ID not found. Please try again.", Color.red);
            return;
        }

        VerifyCodeData verifyCodeData = new VerifyCodeData
        {
            user_id = userId,
            code = codeField.text
        };

        StartCoroutine(SendVerifyCodeRequest(verifyCodeData));
    }

    private IEnumerator SendVerifyCodeRequest(VerifyCodeData verifyCodeData)
    {
        string jsonData = JsonUtility.ToJson(verifyCodeData);

        UnityWebRequest request = new UnityWebRequest(verifyCodeUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
        {
            ShowMessage("Code verified successfully!", Color.green);
            Debug.Log("Código verificado: " + request.downloadHandler.text);

            if (resetPasswordScreen != null)
            {
                gameObject.SetActive(false);
                resetPasswordScreen.SetActive(true);
            }
        }
        else
        {
            ShowMessage("Error: " + request.error, Color.red);
            Debug.LogError("Error al verificar el código: " + request.error);
        }
    }

    private void ShowMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
    }
}

[System.Serializable]
public class VerifyCodeData
{
    public string user_id;
    public string code;
}
