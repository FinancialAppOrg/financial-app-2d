using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Linq;

public class ResetPasswordScreen : MonoBehaviour
{
    [Header("User Information")]
    [SerializeField] TMP_InputField newPasswordField; 
    [SerializeField] TMP_InputField confirmPasswordField;

    [Header("Submit Button")]
    [SerializeField] Button submitButton; 

    [Header("Message Text")]
    [SerializeField] TMP_Text messageText; 

    [Header("Back Button")]
    [SerializeField] Button backButton;

    [Header("Game Manager")]
    [SerializeField] GameManagerAuth gameManagerAuth; 

    //private string resetPasswordUrl = "http://127.0.0.1:8000/api/v1/auth/change-password/";
    private string resetPasswordUrl = "https://financeapp-backend-production.up.railway.app/api/v1/auth/change-password/";

    private void Start()
    {
        submitButton.onClick.AddListener(OnSubmitResetPassword);

        if (backButton != null) 
        {
            backButton.onClick.AddListener(OnBackToEnterCodeClick);
        }
    }

    public void ShowResetPasswordScreen(bool show)
    {
        gameObject.SetActive(show);

        if (show)
        {
            newPasswordField.text = string.Empty;
            confirmPasswordField.text = string.Empty;
            ShowMessage(string.Empty, Color.black);
        }
    }

    private void OnBackToEnterCodeClick()
    {
        gameObject.SetActive(false);
    }

    private void OnSubmitResetPassword()
    {
        if (!IsValidPassword(newPasswordField.text))
        {
            return;
        }

        if (string.IsNullOrEmpty(newPasswordField.text) || string.IsNullOrEmpty(confirmPasswordField.text))
        {
            ShowMessage("Both password fields are required.", Color.red);
            return;
        }

        if (newPasswordField.text != confirmPasswordField.text)
        {
            ShowMessage("Passwords do not match.", Color.red);
            return;
        }

        string userId = PlayerPrefs.GetString("user_id", null);
        if (string.IsNullOrEmpty(userId))
        {
            ShowMessage("User ID not found. Please try again.", Color.red);
            return;
        }


        ResetPasswordData resetPasswordData = new ResetPasswordData
        {
            password = newPasswordField.text
        };

        StartCoroutine(SendResetPasswordRequest(userId, resetPasswordData));
    }

    private IEnumerator SendResetPasswordRequest(string userId, ResetPasswordData resetPasswordData)
    {
        string jsonData = JsonUtility.ToJson(resetPasswordData);

        UnityWebRequest request = new UnityWebRequest(resetPasswordUrl + userId, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
        {
            ShowMessage("Password reset successfully!", Color.green);
            Debug.Log("Contraseña restablecida: " + request.downloadHandler.text);

            if (gameManagerAuth != null)
            {
                gameManagerAuth.BackToSignInScreen();
            }
        }
        else
        {
            ShowMessage("Error al restablecer la contraseña", Color.red);
            Debug.LogError("Error al restablecer la contraseña: " + request.error);
        }
    }

    private void ShowMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
    }

    private bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            ShowMessage("La contraseña no puede estar vacía.", Color.red);
            return false;
        }

        if (password.Length < 8)
        {
            ShowMessage("La contraseña debe tener al menos 8 caracteres.", Color.red);
            return false;
        }

        if (!password.Any(char.IsUpper))
        {
            ShowMessage("La contraseña debe contener al menos una letra mayúscula.", Color.red);
            return false;
        }

        if (!password.Any(char.IsLower))
        {
            ShowMessage("La contraseña debe contener al menos una letra minúscula.", Color.red);
            return false;
        }

        if (!password.Any(char.IsDigit))
        {
            ShowMessage("La contraseña debe contener al menos un número.", Color.red);
            return false;
        }

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            ShowMessage("La contraseña debe contener al menos un carácter especial (ej. !@#$%).", Color.red);
            return false;
        }

        if (password.Contains(" "))
        {
            ShowMessage("La contraseña no puede contener espacios.", Color.red);
            return false;
        }

        return true;
    }

}

[System.Serializable]
public class ResetPasswordData
{
    public string password;
}
