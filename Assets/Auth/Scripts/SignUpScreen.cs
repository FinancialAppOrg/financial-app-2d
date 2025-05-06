using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class SignUpScreen : MonoBehaviour
{
    [Header("User Information")]
    [SerializeField] TMP_InputField usernameField;
    [SerializeField] TMP_InputField birthdateField;
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passwordField;

    [Header("Terms and Conditions")]
    [SerializeField] Toggle termsToggle;

    [Header("Submit Button")]
    [SerializeField] Button submitButton;

    [Header("Message Text")]
    [SerializeField] TMP_Text messageText;

    [Header("Button Sign In")]
    [SerializeField] Button buttonSignIn; 

    [Header("Sign In Screen Canvas")]
    [SerializeField] GameObject signInScreenCanvas;

    [Header("Sign Up Screen Canvas")]
    [SerializeField] GameObject signUpScreenCanvas; 

    private string signUpUrl = "http://127.0.0.1:8000/api/v1/auth/sign-up";
    
    private GameManagerAuth gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManagerAuth>();

        if (gameManager == null)
        {
            Debug.LogWarning("GameManagerAuth no encontrado en la escena.");
        }

        submitButton.interactable = false;
        termsToggle.onValueChanged.AddListener(OnTermsToggleChanged);
        submitButton.onClick.AddListener(OnSubmit);

        if (buttonSignIn != null) 
        {
            buttonSignIn.onClick.AddListener(OnSignInClick);
        }
    }

    private void OnTermsToggleChanged(bool isOn)
    {
        if (isOn)
        {
            if (AreFieldsComplete())
            {
                submitButton.interactable = true;
                ShowMessage("", Color.clear); 
            }
            else
            {
                submitButton.interactable = false;
                ShowMessage("Por favor, completa todos los campos antes de registrarte.", Color.red);
            }
        }
        else
        {
            submitButton.interactable = false;
            ShowMessage("Debes aceptar los términos y condiciones para registrarte.", Color.red);
        }
    }

    private bool AreFieldsComplete()
    {
        return !string.IsNullOrEmpty(usernameField.text) &&
               !string.IsNullOrEmpty(birthdateField.text) &&
               !string.IsNullOrEmpty(emailField.text) &&
               !string.IsNullOrEmpty(passwordField.text);
    }

    private void OnSignInClick()
    {
        if (signInScreenCanvas != null && signUpScreenCanvas != null)
        {
            signInScreenCanvas.SetActive(true);
            signUpScreenCanvas.SetActive(false);
        }
    }

    private void OnSubmit()
    {
        if (!AreFieldsComplete())
        {
            ShowMessage("Todos los campos son obligatorios.", Color.red);
            return;
        }

        SignUpData signUpData = new SignUpData
        {
            name = usernameField.text,
            birthdate = birthdateField.text,
            email = emailField.text,
            password = passwordField.text,
            image_url = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_960_720.png" 
        };

        StartCoroutine(SendSignUpRequest(signUpData));
    }

    private IEnumerator SendSignUpRequest(SignUpData signUpData)
    {
        string jsonData = JsonUtility.ToJson(signUpData);

        UnityWebRequest request = new UnityWebRequest(signUpUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && (request.responseCode == 200 || request.responseCode == 201))
        {
            ShowMessage("Sign-up successful!", Color.green);

            Debug.Log("Registro exitoso: " + request.downloadHandler.text);

            if (signInScreenCanvas != null && signUpScreenCanvas != null)
            {
                signInScreenCanvas.SetActive(true);
                signUpScreenCanvas.SetActive(false);
            }

            if (gameManager != null)
            {
                gameManager.OnSignUpComplete();
            }
        }
        else
        {
            string errorMessage = request.responseCode == 400
            ? "Error: Datos inválidos. Por favor, verifica la información ingresada."
            : "Error: " + request.error;

            ShowMessage("Error: " + request.error, Color.red);
            Debug.LogError("Error en el registro: " + request.error + " (Código: " + request.responseCode + ")");
        }
    }

    private void ShowMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
    }
}

[System.Serializable]
public class SignUpData
{
    public string name;
    public string birthdate;
    public string email;
    public string password;
    public string image_url;
}
