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
    public TextMeshProUGUI investmentAmountText;  // Muestra la cantidad de inversi�n
    public Button optionButton1;  // Bot�n para la primera opci�n
    public Button optionButton2;  // Bot�n para la segunda opci�n
    public Button optionButton3;
    public TextMeshProUGUI resultText;  // Texto para mostrar el resultado de la inversi�n
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
            Debug.LogError("GameManager no est� inicializado.");
            yield break;
        }

        string temaActual = PlayerData.GetTema();
        string nivelActual = PlayerData.GetNivel();
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

            // Buscar la situaci�n basada en tema, nivel y situacion_id
            currentSituacion = situacionesData.situaciones.Find(
                s => s.tema == temaActual && s.nivel == nivelActual && s.situacion_id == areaIndicador // Enfoque en la situaci�n 1
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
                    Debug.LogWarning("No hay suficientes opciones en la situaci�n " + currentSituacion.situacion_id);
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

        int currentBalance = gameManager.GetBalance();
        Opcion selectedOption = currentSituacion.opciones.Find(o => o.opcion_id == optionId);

        if (selectedOption == null)
        {
            Debug.LogWarning("Opci�n no encontrada con ID: " + optionId);
            resultText.text = "Opci�n no v�lida.";
            return;
        }

        // Aplicar impacto al saldo
        int newBalance = currentBalance + selectedOption.impacto_saldo;
        gameManager.UpdateBalance(newBalance);

        string resultMessage = selectedOption.impacto_saldo >= 0 ? "Ganaste $" + selectedOption.impacto_saldo : "Perdiste $" + Mathf.Abs(selectedOption.impacto_saldo);
        resultText.text = "Resultado: " + resultMessage + ". Saldo actual: $" + newBalance;

        Debug.Log("Inversi�n realizada. Nueva situaci�n procesada.");

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


//public TextMeshProUGUI correctasText;
//public TextMeshProUGUI incorrectasText;

/*
// Verificar si la opci�n es correcta
if (selectedOption.correcta == 1)
{
    PlayerData.IncrementarCorrectas();
}
else
{
    PlayerData.IncrementarIncorrectas();
}

// Actualizar los contadores en pantalla
correctasText.text = "Correctas: " + PlayerData.GetCorrectas();
incorrectasText.text = "Incorrectas: " + PlayerData.GetIncorrectas();

Debug.Log("Inversi�n realizada. Nueva situaci�n procesada.");*/


/*
public void Invest(int optionId)
{
    Debug.Log("gameManager: " + gameManager);  // Verificar si es null

    if (gameManager == null)
    {
        Debug.LogError("gameManager es nulo en el m�todo Invest().");
        return;
    }
    int currentBalance = gameManager.GetBalance();
    int impactAmount = GetImpactAmount(optionId);

    if (impactAmount == 0)
    {
        resultText.text = "Opci�n no v�lida.";
        Debug.Log("Opci�n no v�lida seleccionada.");
        return;
    }

    int newBalance = currentBalance + impactAmount;
    gameManager.UpdateBalance(newBalance);

    string resultMessage = impactAmount >= 0 ? "Ganaste $" + impactAmount : "Perdiste $" + Mathf.Abs(impactAmount);
    resultText.text = "Resultado: " + resultMessage + ". Saldo actual: $" + newBalance;

    Debug.Log("Inversi�n realizada. Saldo actualizado: " + newBalance);
}
private int GetImpactAmount(int optionId)
{
    switch (optionId)
    {
        case 1: return -50;
        case 2: return -50;
        case 3: return 100;
        default: return 0;
    }
}
*/