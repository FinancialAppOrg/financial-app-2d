using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Linq;

public class SignInScreen : MonoBehaviour
{
    [Header("User Information")]
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passwordField;

    [Header("Submit Button")]
    [SerializeField] Button submitButton;

    [Header("Message Text")]
    [SerializeField] TMP_Text messageText;

    [Header("Button Forgot Password")]
    [SerializeField] Button buttonForgotPassword;

    [Header("Forgot Password Panel")]
    [SerializeField] GameObject forgotPasswordPanel;

    [Header("Button Sign Up")] 
    [SerializeField] Button buttonSignUp;

    [Header("Sign Up Screen Canvas")] 
    [SerializeField] GameObject signUpScreenCanvas; 

    [Header("Sign In Screen Canvas")] 
    [SerializeField] GameObject signInScreenCanvas; 


    // private string signInUrl = "http://127.0.0.1:8000/api/v1/auth/sign-in";
    private string signInUrl = "https://financeapp-backend-production.up.railway.app/api/v1/auth/sign-in";


    private GameManagerAuth gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManagerAuth>();

        submitButton.onClick.AddListener(OnSubmitSignIn);
   
        buttonForgotPassword.onClick.AddListener(OnForgotPasswordClick);

        if (buttonSignUp != null) 
        {
            buttonSignUp.onClick.AddListener(OnSignUpClick);
        }

        if (forgotPasswordPanel != null)
        {
            forgotPasswordPanel.SetActive(false);
        }
    }

    private void OnForgotPasswordClick()
    {
        if (forgotPasswordPanel != null)
        {
            forgotPasswordPanel.SetActive(true);
        }
    }

    private void OnSignUpClick()
    {
        if (signUpScreenCanvas != null && signInScreenCanvas != null)
        {
            signUpScreenCanvas.SetActive(true);
            signInScreenCanvas.SetActive(false);
        }
    }

    private void OnSubmitSignIn()
    {
        if (string.IsNullOrEmpty(emailField.text) || string.IsNullOrEmpty(passwordField.text))
        {
            ShowMessage("Email and password are required.", Color.red);
            return;
        }
        
        if(!IsValidEmail(emailField.text))
        {
            ShowMessage("Invalid email format.", Color.red);
            return;
        }

        //if (passwordField.text.Length < 6)
        //{
        //    ShowMessage("Password must be at least 6 characters long.", Color.red);
        //    return;
        //}

        if (!IsValidPassword(passwordField.text))
        {
            return;
        }

        SignInData signInData = new SignInData
        {
            email = emailField.text,
            password = passwordField.text
        };

        StartCoroutine(SendSignInRequest(signInData));
    }

    private IEnumerator SendSignInRequest(SignInData signInData)
    {
        string jsonData = JsonUtility.ToJson(signInData);

        UnityWebRequest request = new UnityWebRequest(signInUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.responseCode == 200)
            {
                SignInResponse response = JsonUtility.FromJson<SignInResponse>(request.downloadHandler.text);

                ShowMessage("Sign-in successful!", Color.green);

                PlayerPrefs.SetString("access_token", response.access_token);
                PlayerPrefs.Save();

                PlayerData.SetUserId(response.user_id);

                if (gameManager != null)
                {
                    gameManager.OnSignInComplete();
                }
            }
            else if (request.responseCode == 401)
            {
                string errorResponse = request.downloadHandler.text;
                ShowMessage("Error de autenticación: " + errorResponse, Color.red);
            }
            else
            {
                ShowMessage("Error: " + request.downloadHandler.text, Color.red);
            }
        }
        else
        {
            ShowMessage("Ingrese correctamente sus datos", Color.red);
            //ShowMessage("Error de conexión: " + request.error, Color.red);
        }
    }

    private void ShowMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
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
public class SignInData
{
    public string email;
    public string password;
}

[System.Serializable]
public class SignInResponse
{
    public string access_token;
    public string token_type;
    public int user_id;
}