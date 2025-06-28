using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Linq;

public class UpdateProfileScreen : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField birthdateInput;
    [SerializeField] TMP_InputField imageUrlInput;
    [SerializeField] TMP_Text statusMessage;

    private string defaultImageUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_960_720.png";

    void Start()
    {
        if (imageUrlInput != null && string.IsNullOrEmpty(imageUrlInput.text))
        {
            imageUrlInput.text = defaultImageUrl;
        }
    }

    public void OnSaveButtonClicked()
    {
        int userId = PlayerData.GetUserId(); 
        string name = nameInput.text;
        string birthdate = birthdateInput.text;
        string imageUrl = imageUrlInput.text;

        if (userId == 0)
        {
            Debug.LogError("El userId no está configurado.");
            ShowStatusMessage("Error: El ID de usuario no está configurado.", Color.red);
            return;
        }

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(birthdate) )//|| string.IsNullOrEmpty(imageUrl)
        {
            Debug.LogError("Todos los campos deben estar llenos.");
            ShowStatusMessage("Todos los campos deben estar llenos.", Color.red);
            return;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(birthdate, @"^\d{4}-\d{2}-\d{2}$"))
        {
            ShowStatusMessage("Formato de fecha inválido. Usa YYYY-MM-DD.", Color.red);
            return;
        }

        if (!DateTime.TryParseExact(birthdate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
        {
            ShowStatusMessage("Fecha de nacimiento inválida.", Color.red);
            return;
        }

        if (parsedDate > DateTime.Today)
        {
            ShowStatusMessage("La fecha de nacimiento no puede ser en el futuro.", Color.red);
            return;
        }

        int age = DateTime.Today.Year - parsedDate.Year;
        if (parsedDate.Date > DateTime.Today.AddYears(-age)) age--;

        if (age < 13)
        {
            ShowStatusMessage("Debes tener al menos 13 años para registrarte.", Color.red);
            return;
        }

        if (age > 100)
        {
            ShowStatusMessage("Por favor ingresa una fecha de nacimiento válida.", Color.red);
            return;
        }

        //if (!DateTime.TryParseExact(birthdate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
        //{
        //    Debug.LogError("El formato de la fecha es inválido. Usa el formato DD-MM-YYYY.");
        //    ShowStatusMessage("El formato de la fecha es inválido. Usa DD-MM-YYYY.", Color.red);
        //    return;
        //}

        string formattedBirthdate = parsedDate.ToString("yyyy-MM-dd");

        StartCoroutine(UpdateUserProfileCoroutine(userId, name, formattedBirthdate, imageUrl));
    }

    private IEnumerator UpdateUserProfileCoroutine(int userId, string name, string birthdate, string imageUrl)
    {
        if (userId == 0)
        {
            Debug.LogError("El userId no está configurado en PlayerPrefs.");
            ShowStatusMessage("El ID de usuario no está configurado.", Color.red);
            yield break;
        }

        string url = $"https://financeapp-backend-production.up.railway.app/api/v1/users/{userId}/profile";
        Debug.Log($"URL generada: {url}");

        UserProfileData updateData = new UserProfileData
        {
            name = name,
            birthdate = birthdate,
            image_url = imageUrl
        };

        string jsonData = JsonUtility.ToJson(updateData);
        Debug.Log($"Datos enviados: {jsonData}");

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            string token = PlayerPrefs.GetString("access_token", "");
            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("El token de acceso no está configurado en PlayerPrefs.");
                ShowStatusMessage("El token de acceso no está configurado.", Color.red);
                yield break;
            }

            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Perfil actualizado exitosamente.");
                ShowStatusMessage("Perfil actualizado exitosamente.", Color.green);
            }
            else
            {
                Debug.LogError($"Error al actualizar el perfil: {request.error}");
                Debug.LogError($"Respuesta del servidor: {request.downloadHandler.text}");
                ShowStatusMessage("Error al actualizar el perfil. Inténtalo de nuevo.", Color.red);
            }
        }


    }

    private void ShowStatusMessage(string message, Color color)
    {
        if (statusMessage != null)
        {
            statusMessage.text = message;
            statusMessage.color = color;
        }
    }

    [System.Serializable]
    public class UserProfileData
    {
        public string name;
        public string birthdate;
        public string image_url;
    }

}