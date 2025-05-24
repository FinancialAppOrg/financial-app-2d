using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class UpdateProfileScreen : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField birthdateInput;
    [SerializeField] TMP_InputField imageUrlInput;
    [SerializeField] TMP_Text statusMessage; 

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

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(birthdate) || string.IsNullOrEmpty(imageUrl))
        {
            Debug.LogError("Todos los campos deben estar llenos.");
            ShowStatusMessage("Error: Todos los campos deben estar llenos.", Color.red);
            return;
        }

        if (!DateTime.TryParseExact(birthdate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
        {
            Debug.LogError("El formato de la fecha es inválido. Usa el formato DD-MM-YYYY.");
            ShowStatusMessage("Error: El formato de la fecha es inválido. Usa DD-MM-YYYY.", Color.red);
            return;
        }

        string formattedBirthdate = parsedDate.ToString("yyyy-MM-dd");

        StartCoroutine(UpdateUserProfileCoroutine(userId, name, formattedBirthdate, imageUrl));
    }

    private IEnumerator UpdateUserProfileCoroutine(int userId, string name, string birthdate, string imageUrl)
    {
        if (userId == 0)
        {
            Debug.LogError("El userId no está configurado en PlayerPrefs.");
            ShowStatusMessage("Error: El ID de usuario no está configurado.", Color.red);
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
                ShowStatusMessage("Error: El token de acceso no está configurado.", Color.red);
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