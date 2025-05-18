using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    public gameManager gameManager;  // Referencia al GameManager
    public GeminiAPIClient geminiClient;//gemini api
    public TextMeshProUGUI investmentAmountText;  // Muestra la cantidad de inversi�n
    public Button optionButton1;  // Bot�n para la primera opci�n
    public Button optionButton2;  // Bot�n para la segunda opci�n
    public TextMeshProUGUI resultText;  // Texto para mostrar el resultado de la inversi�n
    public TextMeshProUGUI responseText;//respuesta del asistente
    public TextMeshProUGUI descripcionText;  // Texto para mostrar la descripcion de la situacion
    public int areaIndicador;
    private Situacion currentSituacion;
    private string baseUrl = "https://financeapp-backend-production.up.railway.app/api/v1/situaciones";
    private string temaActual = "inversion";//PlayerData.GetTema();
    private string nivelActual = "basico";//PlayerData.GetNivel();


    void Start()
    {
        // Cargar datos desde la API
        StartCoroutine(LoadSituacionData());
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{ \"items\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.items;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] items;
        }
    }

    private IEnumerator LoadSituacionData()
    {
        string apiUrl = $"{baseUrl}?tema={temaActual}&nivel={nivelActual}";
        Debug.Log("Solicitando datos de: " + apiUrl);

        if (gameManager == null)
        {
            Debug.LogError("GameManager no est� inicializado.");
            yield break;
        }

        Debug.Log("Tema actual: " + temaActual + ", Nivel actual: " + nivelActual);

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al cargar los datos: " + request.error);
                yield break;
            }

            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Datos recibidos: " + jsonResponse);

            Situacion[] situacionesData = JsonHelper.FromJson<Situacion>(jsonResponse);

            // Buscar la situaci�n basada en tema, nivel y situacion_id
            currentSituacion = Array.Find(situacionesData,
                s => s.tema == temaActual && s.nivel == nivelActual && s.id_situacion == areaIndicador // situacionid
            );

            if (currentSituacion != null)
            {
                descripcionText.text = currentSituacion.descripcion;

                if (currentSituacion.opciones.Count >= 2)
                {
                    optionButton1.GetComponentInChildren<TextMeshProUGUI>().text = currentSituacion.opciones[0].descripcion_opcion;
                    optionButton2.GetComponentInChildren<TextMeshProUGUI>().text = currentSituacion.opciones[1].descripcion_opcion;

                    // Remover listeners previos para evitar duplicados
                    optionButton1.onClick.RemoveAllListeners();
                    optionButton2.onClick.RemoveAllListeners();

                    // Asignar los `id_opcion` din�micamente al hacer clic
                    int opcion1Id = currentSituacion.opciones[0].id_opcion;
                    int opcion2Id = currentSituacion.opciones[1].id_opcion;

                    optionButton1.onClick.AddListener(() => Invest(opcion1Id));
                    optionButton2.onClick.AddListener(() => Invest(opcion2Id));
                }
                else
                {
                    Debug.LogWarning("No hay suficientes opciones en la situaci�n " + currentSituacion.id_situacion);
                }
            }
            else
            {
                Debug.LogError("No se encontr� una situaci�n para el tema: " + temaActual + " y nivel: " + nivelActual);
            }
        }
    }
    public void Invest(int optionId)
    {
        if (gameManager == null || currentSituacion == null)
        {
            Debug.LogError("GameManager o situaci�n actual no est� configurado.");
            return;
        }

        Opcion selectedOption = GetOptionById(optionId);

        if (selectedOption != null)
        {
            ProcessOptionImpact(selectedOption);
            RequestAssistant(selectedOption);
        }
        else
        {
            Debug.LogWarning("Opci�n no encontrada con ID: " + optionId);
            resultText.text = "Opci�n no v�lida.";
        }
    }

    private Opcion GetOptionById(int optionId)
    {
        return currentSituacion.opciones.Find(o => o.id_opcion == optionId);
    }

    private void ProcessOptionImpact(Opcion selectedOption)
    {
        int currentBalance = gameManager.GetBalance();
        int newBalance = currentBalance + selectedOption.impacto_saldo;
        gameManager.UpdateBalance(newBalance);

        string resultMessage = selectedOption.impacto_saldo >= 0
            ? $"�Ganaste! \nS/{selectedOption.impacto_saldo}"
            : $"�Perdiste! \nS/{Mathf.Abs(selectedOption.impacto_saldo)}";

        resultText.text = resultMessage;
        Debug.Log($"Opci�n {selectedOption.id_opcion} seleccionada. Nuevo saldo: S/{newBalance}");
    }

    private void RequestAssistant(Opcion selectedOption)
    {
        if (geminiClient == null)
        {
            Debug.LogError("No se pudo acceder al asistente");
            return;
        }

        Debug.Log("Solicitando explicaci�n al asistente...");
        geminiClient.AskFinancialQuestion(
            currentSituacion.descripcion,
            selectedOption,
            selectedOption.descripcion_opcion,
            (response) =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    Debug.LogWarning("La respuesta del asistente est� vac�a o es nula.");
                }
                else
                {
                    responseText.text = "\n" + response;
                    Debug.Log("Respuesta del asistente recibida: " + response);
                }
            }
        );
    }


}

[Serializable]
public class SituacionesData
{
    public List<Situacion> situaciones;
}

[Serializable]
public class Situacion
{
    public int id_situacion;
    public string tema;
    public string nivel;
    public string descripcion;
    public string saldo_inicial;
    public List<Opcion> opciones;
}

[Serializable]
public class Opcion
{
    public int id_opcion;
    public string descripcion_opcion;
    public int es_correcta;  // 1 = correcta, 0 = incorrecta
    public int impacto_saldo;
}

