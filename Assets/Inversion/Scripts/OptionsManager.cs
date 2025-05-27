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
    public popManager popManager;
    public GeminiAPIClient geminiClient;//gemini api
    public TextMeshProUGUI investmentAmountText;  // Muestra la cantidad de inversión
    public Button optionButton1;  // Botón para la primera opción
    public Button optionButton2;  // Botón para la segunda opción
    public TextMeshProUGUI resultText;  // Texto para mostrar el resultado de la inversión
    public TextMeshProUGUI responseText;//respuesta del asistente
    public TextMeshProUGUI descripcionText;  // Texto para mostrar la descripcion de la situacion
    public List<AreaButtonBinding> areaButtonBindings;
    private Dictionary<int, GameObject> areaVisuals = new Dictionary<int, GameObject>();
    //public int areaIndicador;
    private Situacion currentSituacion;
    private string baseUrl = "https://financeapp-backend-production.up.railway.app/api/v1/situaciones";
    private string temaActual = PlayerData.GetSelectedTopic();
    private string nivelActual = PlayerData.GetSelectedLevel();
    private int areaId;

    void Start()
    {
        foreach (var binding in areaButtonBindings)
        {
            if (binding != null && binding.areaObject != null)
            {
                areaVisuals[binding.situacionId] = binding.areaObject;
            }
            else
            {
                Debug.LogWarning("Binding nulo o sin botón asignado");
            }
        }
    }

    public void LoadSituacionDataForSelectedArea()
    {
        string selectedArea = gameManager.GetSelectedArea();
        Debug.Log("Cargando datos para el área seleccionada: " + selectedArea);

        if (!string.IsNullOrEmpty(selectedArea))
        {
            StartCoroutine(LoadSituacionData(selectedArea));
        }
        else
        {
            Debug.LogWarning("selectedArea está vacío o nulo.");
        }
    }

    //Get situations
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

    private IEnumerator LoadSituacionData(string areaName)
    {
        string apiUrl = $"{baseUrl}?tema={temaActual}&nivel={nivelActual}";
        Debug.Log("Solicitando datos de: " + apiUrl);
        areaId = gameManager.GetAreaIndicador(areaName);
        Debug.Log("Indicador recibido: " + areaId);

        if (gameManager == null)
        {
            Debug.LogError("GameManager no está inicializado.");
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

            Debug.Log($"Tema actual: {temaActual}, Nivel actual: {nivelActual}, Area ID: {areaId}");


            // Buscar la situación basada en tema, nivel y situacion_id
            currentSituacion = Array.Find(situacionesData,
                s => s.tema == temaActual && s.nivel == nivelActual //&& s.id_situacion == areaId // situacionid
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
                    // Asignar los id_opcion dinámicamente al hacer clic
                    int opcion1Id = currentSituacion.opciones[0].id_opcion;
                    int opcion2Id = currentSituacion.opciones[1].id_opcion;
                    optionButton1.onClick.AddListener(() => Invest(opcion1Id));
                    optionButton2.onClick.AddListener(() => Invest(opcion2Id));
                }
                else
                {
                    Debug.LogWarning("No hay suficientes opciones en la situación " + currentSituacion.id_situacion);
                }
            }
            else
            {
                Debug.LogError("URL: " + apiUrl + " no contiene una situación válida para el tema: " + temaActual + " y nivel: " + nivelActual + ". Asegúrate de que el tema y nivel sean correctos.");
                Debug.LogError("No se encontró una situación para el tema: " + temaActual + " y nivel: " + nivelActual);
                Debug.Log("Error selectedArea: " + areaName);
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

        Opcion selectedOption = GetOptionById(optionId);

        if (selectedOption != null)
        {
            //Enviar decisión
            int gameId = PlayerPrefs.GetInt("gameId", 0); 
            int situacionId = currentSituacion.id_situacion;
            int opcionElegida = selectedOption.id_opcion;
            bool correcto = selectedOption.es_correcta;

            Debug.Log($"Enviando decisión: juego={gameId}, situacion={situacionId}, opcion={opcionElegida}, correcto={correcto}");
            gameManager.SubmitDecision(gameId, situacionId, opcionElegida);
            
            if (areaVisuals.TryGetValue(areaId, out GameObject areaObj))
            {
                var sprite = areaObj.GetComponent<SpriteRenderer>();
                if (sprite != null)
                {
                    sprite.color = Color.gray;
                }
                var btn = areaObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = false; 
                }

                AreaInteraction handler = areaObj.GetComponent<AreaInteraction>();
                if (handler != null)
                {
                    handler.DesactivarArea(); 
                }
            }
            else
            {
                Debug.LogWarning("No se encontró el área visual para el ID: " + areaId);
            }
            

            ProcessOptionImpact(selectedOption);

            if (!correcto)
            {
                popManager.OpenAssistantPanel(); 
                RequestAssistant(selectedOption);
                StartCoroutine(gameManager.EsperarAsistenteYVerificarEndGame());
                Debug.Log("es incorrecta");
            }
            else
            {
                //gameManager.VerificarEndGame();
                popManager.CloseOptionsPanel();
                gameManager.VerificarEndGame();
                Debug.Log("es correcta");
            }
        }
        else
        {
            Debug.LogWarning("Opción no encontrada con ID: " + optionId);
            resultText.text = "Opción no válida.";
        }
    }

    private Opcion GetOptionById(int optionId)
    {
        return currentSituacion.opciones.Find(o => o.id_opcion == optionId);
    }

    private void ProcessOptionImpact(Opcion selectedOption)
    {
        //int currentBalance = gameManager.GetBalance();
        //int newBalance = currentBalance + selectedOption.impacto_saldo;
        //gameManager.UpdateBalance();

        string resultMessage = selectedOption.impacto_saldo >= 0
            ? $"¡Ganaste! \nS/{selectedOption.impacto_saldo}"
            : $"¡Perdiste! \nS/{Mathf.Abs(selectedOption.impacto_saldo)}";

        resultText.text = resultMessage;
        Debug.Log($"Opción {selectedOption.id_opcion} seleccionada.");
    }

    private void RequestAssistant(Opcion selectedOption)
    {
        if (geminiClient == null)
        {
            Debug.LogError("No se pudo acceder al asistente");
            return;
        }

        Debug.Log("Solicitando explicación al asistente...");
        geminiClient.AskFinancialQuestion(
            currentSituacion.descripcion,
            selectedOption,
            selectedOption.descripcion_opcion,
            (response) =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    Debug.LogWarning("La respuesta del asistente está vacía o es nula.");
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
    public bool es_correcta;  
    public int impacto_saldo;
}

[Serializable]
public class AreaButtonBinding
{
    public int situacionId;
    public GameObject areaObject;
}
