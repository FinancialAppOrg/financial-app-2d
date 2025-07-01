using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System;
using System.Linq; 

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

    private string signUpUrl = "https://financeapp-backend-production.up.railway.app/api/v1/auth/sign-up";
    //private string signUpUrl = "http://127.0.0.1:8000/api/v1/auth/sign-up";
    
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

        //if (!IsValidEmail(emailField.text))
        //{
        //    ShowMessage("Invalid email format.", Color.red);
        //    return;
        //}

        if (!IsValidEmail(emailField.text))
        {
            if (string.IsNullOrWhiteSpace(emailField.text))
                ShowMessage("El email no puede estar vacío.", Color.red);
            else if (!emailField.text.Contains("@"))
                ShowMessage("El email debe contener un @.", Color.red);
            else if (!emailField.text.Contains("."))
                ShowMessage("El email debe contener un dominio válido (ejemplo.com).", Color.red);
            else if (emailField.text.Contains(" "))
                ShowMessage("El email no puede contener espacios.", Color.red);
            else
                ShowMessage("Formato de email inválido. Por favor ingresa un email válido.", Color.red);
            return;
        }

        //if (!IsValidBirthdate(birthdateField.text))
        //{
        //    ShowMessage("Fecha de nacimiento inválida. Debe estar en formato YYYY-MM-DD y tener al menos 13 años.", Color.red);
        //    return;
        //}

        if (!IsValidBirthdate(birthdateField.text))
        {
            if (string.IsNullOrWhiteSpace(birthdateField.text))
                ShowMessage("La fecha de nacimiento no puede estar vacía.", Color.red);
            else if (!System.Text.RegularExpressions.Regex.IsMatch(birthdateField.text, @"^\d{4}-\d{2}-\d{2}$"))
                ShowMessage("Formato de fecha inválido. Usa YYYY-MM-DD.", Color.red);
            else
            {
                try
                {
                    DateTime parsedDate = DateTime.ParseExact(birthdateField.text, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    if (parsedDate > DateTime.Today)
                        ShowMessage("La fecha de nacimiento no puede ser en el futuro.", Color.red);
                    else if (DateTime.Today.Year - parsedDate.Year < 13 ||
                            (DateTime.Today.Year - parsedDate.Year == 13 &&
                             DateTime.Today.DayOfYear < parsedDate.DayOfYear))
                        ShowMessage("Debes tener al menos 13 años para registrarte.", Color.red);
                    else if (DateTime.Today.Year - parsedDate.Year > 100)
                        ShowMessage("Por favor ingresa una fecha de nacimiento válida.", Color.red);
                }
                catch
                {
                    ShowMessage("Fecha de nacimiento inválida.", Color.red);
                }
            }
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
            ? "Datos inválidos. Por favor, verifica la información ingresada."
            : "Error: " + request.error;

            //ShowMessage("Error: " + request.error, Color.red);
            ShowMessage("Ingrese correctamente sus datos", Color.red);
            Debug.LogError("Error en el registro: " + request.error + " (Código: " + request.responseCode + ")");
        }
    }

    private void ShowMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
    }

    //private bool IsValidEmail(string email)
    //{
    //    try
    //    {
    //        var addr = new System.Net.Mail.MailAddress(email);
    //        return addr.Address == email;
    //    }
    //    catch
    //    {
    //        return false;
    //    }
    //}


    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, pattern))
                return false;

            var atCount = email.Count(c => c == '@');
            if (atCount != 1) return false;

            var parts = email.Split('@');
            if (parts.Length != 2) return false;

            var domainParts = parts[1].Split('.');
            if (domainParts.Length < 2) return false;

            if (email.Contains(" ")) return false;

            if (email.StartsWith(".") || email.EndsWith(".") ||
                email.StartsWith("-") || email.EndsWith("-"))
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidBirthdate(string birthdate)
    {
        if (string.IsNullOrWhiteSpace(birthdate))
            return false;

        if (!System.Text.RegularExpressions.Regex.IsMatch(birthdate, @"^\d{4}-\d{2}-\d{2}$"))
            return false;

        try
        {
            DateTime parsedDate = DateTime.ParseExact(birthdate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            if (parsedDate > DateTime.Today)
                return false;

            if (DateTime.Today.Year - parsedDate.Year < 13 ||
                (DateTime.Today.Year - parsedDate.Year == 13 &&
                 DateTime.Today.DayOfYear < parsedDate.DayOfYear))
            {
                return false;
            }

            if (DateTime.Today.Year - parsedDate.Year > 100)
                return false;

            return true;
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
public class SignUpData
{
    public string name;
    public string birthdate;
    public string email;
    public string password;
    public string image_url;
}
