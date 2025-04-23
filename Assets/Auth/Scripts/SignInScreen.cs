using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class SignInScreen : MonoBehaviour
{
    [Header("User Information")]
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passwordField;

    [Header("Submit Button")]
    [SerializeField] Button submitButton;

    [Header("Message Text")]
    [SerializeField] TMP_Text messageText;

    private string signInUrl = "http://127.0.0.1:8000/api/v1/auth/sign-in"; 

    private GameManagerAuth gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManagerAuth>();

        submitButton.onClick.AddListener(OnSubmitSignIn);
    }

    private void OnSubmitSignIn()
    {
        if (string.IsNullOrEmpty(emailField.text) || string.IsNullOrEmpty(passwordField.text))
        {
            ShowMessage("Email and password are required.", Color.red);
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
            SignInResponse response = JsonUtility.FromJson<SignInResponse>(request.downloadHandler.text);

            ShowMessage("Sign-in successful!", Color.green);

            PlayerPrefs.SetString("access_token", response.access_token);

            if (gameManager != null)
            {
                gameManager.OnSignInComplete();
            }
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
}