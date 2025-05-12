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
    public TextMeshProUGUI investmentAmountText;  // Muestra la cantidad de inversión
    public Button optionButton1;  // Botón para la primera opción
    public Button optionButton2;  // Botón para la segunda opción
    public Button optionButton3;
    public TextMeshProUGUI resultText;  // Texto para mostrar el resultado de la inversión
    public TextMeshProUGUI responseText;
    public TextMeshProUGUI descripcionText;  // Texto para mostrar la descripcion de la situacion
    public int areaIndicador;
    private Situacion currentSituacion;
    private string apiUrl = "https://angie-rc.github.io/api-fake/data.json";

    void Start()
    {
        optionButton1.onClick.AddListener(() => Invest(1));
        optionButton2.onClick.AddListener(() => Invest(2));
        optionButton3.onClick.AddListener(() => Invest(3));
        // Cargar datos desde la API
        StartCoroutine(LoadSituacionData());
    }

    private IEnumerator LoadSituacionData()
    {
        if (gameManager == null)
        {
            Debug.LogError("GameManager no está inicializado.");
            yield break;
        }

        string temaActual = "inversion";//PlayerData.GetTema();
        string nivelActual = "basico";//PlayerData.GetNivel();
        //int areaIndicador = gameManager.GetAreaIndicador();
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

            SituacionesData situacionesData = JsonUtility.FromJson<SituacionesData>(jsonResponse);

            // Buscar la situación basada en tema, nivel y situacion_id
            currentSituacion = situacionesData.situaciones.Find(
                s => s.tema == temaActual && s.nivel == nivelActual && s.situacion_id == areaIndicador // situacionid
            );

            if (currentSituacion != null)
            {
                descripcionText.text = currentSituacion.descripcion;

                if (currentSituacion.opciones.Count >= 3)
                {
                    optionButton1.GetComponentInChildren<TextMeshProUGUI>().text = currentSituacion.opciones[0].descripcion;
                    optionButton2.GetComponentInChildren<TextMeshProUGUI>().text = currentSituacion.opciones[1].descripcion;
                    optionButton3.GetComponentInChildren<TextMeshProUGUI>().text = currentSituacion.opciones[2].descripcion;
                }
                else
                {
                    Debug.LogWarning("No hay suficientes opciones en la situación " + currentSituacion.situacion_id);
                }
            }
            else
            {
                Debug.LogError("No se encontró una situación para el tema: " + temaActual + " y nivel: " + nivelActual);
            }
        }
    }

    public void Invest(int optionId)
    {
        if (gameManager == null || currentSituacion == null)
        {
            Debug.LogError("GameManager o situación actual no está configurado.");
            return;
        }

        int currentBalance = gameManager.GetBalance();
        Opcion selectedOption = currentSituacion.opciones.Find(o => o.opcion_id == optionId);

        if (selectedOption == null)
        {
            Debug.LogWarning("Opción no encontrada con ID: " + optionId);
            resultText.text = "Opción no válida.";
            return;
        }

        // Aplicar impacto al saldo
        int newBalance = currentBalance + selectedOption.impacto_saldo;
        gameManager.UpdateBalance(newBalance);

        string resultMessage = selectedOption.impacto_saldo >= 0 ? "¡Ganaste! \n$" + selectedOption.impacto_saldo : "¡Perdiste! \n$" + Mathf.Abs(selectedOption.impacto_saldo);
        resultText.text = resultMessage;//"Resultado: " + resultMessage + ". Saldo actual: $" + newBalance;

        Debug.Log("Decision seleccionada. Nuevo saldo procesado.");

        // Solicitar explicación del asistente
        if (geminiClient != null)
        {
            Debug.Log("Solicitando explicación al asistente...");
            geminiClient.AskFinancialQuestion(currentSituacion.descripcion,selectedOption, selectedOption.descripcion, (response) =>
            {
                Debug.Log("Respuesta del asistente recibida: " + response);

                if (string.IsNullOrEmpty(response))
                {
                    Debug.LogWarning("La respuesta del asistente está vacía o es nula.");
                }
                else
                {
                    responseText.text = "\n" + response;
                }
                //responseText.text += "\nAsistente: " + response;
            });
        }
        else
        {
            Debug.LogError("No se pudo acceder al asistente");
        }

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
    public int situacion_id;
    public string tema;
    public string nivel;
    public string descripcion;
    public List<Opcion> opciones;
}

[Serializable]
public class Opcion
{
    public int opcion_id;
    public string descripcion;
    public int correcta;  // 1 = correcta, 0 = incorrecta
    public int impacto_saldo;
}

