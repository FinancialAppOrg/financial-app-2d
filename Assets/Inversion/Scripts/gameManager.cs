using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using System.Text;

public class gameManager : MonoBehaviour
{
    //private GameManager sharedGameManager;

    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI aciertosText;
    public TextMeshProUGUI clasificacionText;
    public TextMeshProUGUI finalSaldoText;
    public TextMeshProUGUI finalAciertosText;
    public TextMeshProUGUI finalMonedasText;
    public TextMeshProUGUI monedasText;
    public TextMeshProUGUI saldoText;
    public GameObject player;   
    public playerController playerController;
    private Animator animator;
    public popManager popManager;
    public UIManager uIManager;
    public Button testButton;
    public Button assistantIcon;
    private string selectedArea;
    private int gameId;
    public int situacionesCompletadas = 0;
    public int totalSituaciones = 5;

    void Start()
    {
        //camera
        float targetAspect = 9f / 16f; // referencia: 1080x1920
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera camera = Camera.main;

        if (scaleHeight < 1.0f)
        {
            camera.orthographicSize /= scaleHeight;
        }
        //
        animator = GetComponent<Animator>();
        if (popManager == null)
            popManager = FindObjectOfType<popManager>();
    }
    //game
    public void StartGame(int userId, string temaSeleccionado, string nivelJugado, float saldoInicial)
    {
        StartCoroutine(PostStartGame(userId, temaSeleccionado, nivelJugado, saldoInicial));
    }

    private IEnumerator PostStartGame(int userId, string temaSeleccionado, string nivelJugado, float saldoInicial)
    {
        string baseUrl = "https://financeapp-backend-production.up.railway.app/api/v1/start-game";

        string jsonData = JsonConvert.SerializeObject(new
        {
            id_usuario = userId,
            tema_seleccionado = temaSeleccionado,
            nivel_jugado = nivelJugado,
            saldo_inicial = saldoInicial
        });

        using (UnityWebRequest request = new UnityWebRequest(baseUrl, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Enviando solicitud para crear juego: " + jsonData);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Juego creado con éxito. Respuesta: " + request.downloadHandler.text);

                try
                {
                    var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                    if (responseData.ContainsKey("id_juego"))
                    {
                        gameId = int.Parse(responseData["id_juego"].ToString());
                        PlayerPrefs.SetInt("gameId", gameId);
                        PlayerPrefs.Save();
                        Debug.Log("Game ID almacenado: " + gameId);
                    }
                    if (responseData.ContainsKey("saldo_inicial"))
                        balanceText.text = responseData["saldo_inicial"].ToString();

                    if (responseData.ContainsKey("clasificacion_resultante"))
                        clasificacionText.text = responseData["clasificacion_resultante"].ToString();

                    if (responseData.ContainsKey("monedas_ganadas"))
                        monedasText.text = responseData["monedas_ganadas"].ToString();

                    Debug.Log($": {balanceText.text}|{monedasText.text} | Aciertos Totales: {aciertosText.text} | {clasificacionText.text}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Error al procesar la respuesta del backend: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError("Error al crear juego: " + request.error);
            }
        }
    }

    public int GetGameId()
    {
        return gameId;
    }

    //decision
    public void SubmitDecision(int gameId, int situacionId, int opcionElegida)
    {
        StartCoroutine(PostSubmitDecision(gameId, situacionId, opcionElegida));
        situacionesCompletadas++;
        Debug.Log("situacionesCompletadas" + situacionesCompletadas);
    }

    private IEnumerator PostSubmitDecision(int gameId, int situacionId, int opcionElegida)
    {
        string endpoint = "https://financeapp-backend-production.up.railway.app/api/v1/submit-decision";
        Debug.Log("Enviando POST a: " + endpoint);
        Decision data = new Decision
        {
            id_juego = gameId,
            situacion_id = situacionId,
            opcion_elegida = opcionElegida
        };
        string jsonData = JsonUtility.ToJson(data);
        Debug.Log("Payload enviado: " + jsonData);


        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("accept", "application/json");

            Debug.Log("Enviando decisión: " + jsonData);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                saldoText.text = responseData["saldo_final"].ToString();
                Debug.Log("Decisión enviada con éxito. Respuesta: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error al enviar decisión: " + request.responseCode + " - " + request.error);
            }
        }
    }

    public IEnumerator EsperarAsistenteYVerificarEndGame()
    {
        while (!popManager.resultsScreen.activeSelf)
        {
            yield return null;
        }
        Debug.Log("Panel del asistente activado");
        while (popManager.resultsScreen.activeSelf)
        {
            yield return null;
        }
        Debug.Log("Panel del asistente cerrado");
        VerificarEndGame();
    }

    public void VerificarEndGame()
    {
        if (situacionesCompletadas >= totalSituaciones)
        {
            uIManager.ShowFeedbackAssistant();
            StartCoroutine(PostEndGame());
        }
    }

    //end game
    private IEnumerator PostEndGame()
    {
        string url = "https://financeapp-backend-production.up.railway.app/api/v1/end-game";

        var jsonData = JsonConvert.SerializeObject(new { id_juego = gameId });

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Enviando solicitud de End Game: " + jsonData);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                Debug.Log("Juego finalizado exitosamente.");
                popManager.OpenResult(); 
                finalSaldoText.text = responseData["saldo_final"].ToString() + "\nSoles";
                finalAciertosText.text = responseData["aciertos_totales"].ToString() + "\nPreguntas correctas";
                finalMonedasText.text = "Ganaste "+responseData["monedas_ganadas"].ToString() + " moneda coleccionable";
                Debug.Log("Decisión enviada con éxito. Respuesta: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error al finalizar el juego: " + request.error);
            }
        }
    }

    //Quizz
    public void StartQuizz(int gameId, int userId, int escenaDestino)
    {
        StartCoroutine(PostQuizzYTransicion(gameId, userId, escenaDestino));
    }

    private IEnumerator PostQuizzYTransicion(int gameId, int userId, int escenaDestino)
    {
        string endpoint = "https://financeapp-backend-production.up.railway.app/api/v1/quizz";

        var jsonData = JsonConvert.SerializeObject(new 
        {
            id_juego = gameId,
            id_usuario = userId
        });

        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("accept", "application/json");

            Debug.Log("Enviando POST de Quizz: " + jsonData);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("POST enviado con éxito: " + request.downloadHandler.text);
                var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                if (responseData.ContainsKey("quizz_id"))
                {
                    int quizz_id = int.Parse(responseData["quizz_id"].ToString());
                    PlayerPrefs.SetInt("quizz_id", quizz_id);
                    PlayerPrefs.Save();
                    Debug.Log("Quizz ID almacenado: " + quizz_id);
                }
                PlayerPrefs.SetString("pantallaEvaluacion", "quizz");
                SceneManager.LoadScene(escenaDestino);
            }
            else
            {
                Debug.LogError("Error al enviar Quizz: " + request.responseCode + " - " + request.error);
            }
        }
    }

    public void ShowWelcomeScreen()
    {
        Debug.Log("Mostrando pantalla de bienvenida...");
        popManager.ShowWelcomeScreen(); 
    }
    public void ShowInvestmentOptions(string areaName)
    {
        // Mostrar las opciones específicas según el área seleccionada
        popManager.ShowAreaPanel(areaName);
    }
    
    public int GetAreaIndicador(string areaName)
    {
        return popManager.GetAreaIndicador(areaName);
    }

    public void SetSelectedArea(string areaName)
    {
        selectedArea = areaName;
        Debug.Log("Área establecida en GameManager: " + selectedArea);
    }

    public string GetSelectedArea()
    {
        return selectedArea;
    }
}

[System.Serializable]
public class Decision
{
    public int id_juego;
    public int situacion_id;
    public int opcion_elegida;
}