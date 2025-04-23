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
    }

    private void OnTermsToggleChanged(bool isOn)
    {
        submitButton.interactable = isOn;
    }

    private void OnSubmit()
    {
        if (string.IsNullOrEmpty(usernameField.text) ||
            string.IsNullOrEmpty(birthdateField.text) ||
            string.IsNullOrEmpty(emailField.text) ||
            string.IsNullOrEmpty(passwordField.text))
        {
            Debug.LogError("Todos los campos son obligatorios.");
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

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowMessage("Sign-up successful!", Color.green);

            Debug.Log("Registro exitoso: " + request.downloadHandler.text);

            if (gameManager != null)
            {
                gameManager.OnSignUpComplete();
            }
        }
        else
        {
            ShowMessage("Error: " + request.error, Color.red);
            Debug.LogError("Error en el registro: " + request.error);
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
