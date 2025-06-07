using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;

public class GeminiAPIClient : MonoBehaviour
{
    private string apiKey = "AIzaSyAlyBaF4by_caLmL_njql4YAHBY4Tsw6Eo";//"AIzaSyCdWwuX7uPQ_Ihmpo1ybZL0XZjQYmNYZHw"; // Reemplaza con tu clave real
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";


    [System.Serializable]
    public class Content
    {
        public string role;
        public Part[] parts;
    }

    [System.Serializable]
    public class Part
    {
        public string text;
    }

    [System.Serializable]
    public class RequestBody
    {
        public Content[] contents;
    }

    [System.Serializable]
    public class ResponseBody
    {
        public Candidate[] candidates;
    }

    [System.Serializable]
    public class Candidate
    {
        public Content content;
        public string finishReason;
        public int index;
    }

    // Contexto financiero para el asistente
    private string financialContext =
        "Eres un asistente virtual especializado en educación financiera. " +
        "Tu objetivo es explicar conceptos financieros de manera clara y sencilla " +
        "para usuarios sin conocimientos avanzados. " +
        "Responde en español y usa ejemplos prácticos. " +
        "Temas principales: ahorro, inversión, créditos, deudas, presupuesto. " +
        "Mantén respuestas breves (máximo 1 párrafo) y no uses emojis o símbolo Unicode. " +
        "Si la pregunta no es financiera, indica amablemente tu especialización. " +
        "Usuario: ";

    public void AskFinancialQuestion(string situacionDescripcion,Opcion opcion, string opcionDescripcion, System.Action<string> callback)
    {
        string question = $"Contexto: {situacionDescripcion}\n" +
                          $"El usuario ha seleccionado la opción: '{opcionDescripcion}'. " +
                          $"Esta opción es {(opcion.es_correcta == true ? "correcta" : "incorrecta")}. " +
                          "Explícale brevemente por qué es una buena o mala decisión (máximo 560 caracteres.";
        StartCoroutine(PostRequest(question, callback));
    }

    public void AssistantGuide(System.Action<string> callback)
    {
        string question = "Dale la bienvendia al usuario, preséntate y tu propósito en el juego para que inicie el juego." +
                          "Mantén respuestas breves (máximo 280 caracteres). ";
        StartCoroutine(PostRequest(question, callback));
    }

    public void AssistantFeedback(System.Action<string> callback)
    {
        string question = "Contexto: Se acabó el juego, dale una corta despedida al usuario (máximo 200 caracteres)";
        StartCoroutine(PostRequest(question, callback));
    }

    public void AssistantFeedback2(string gameData, System.Action<string> callback)
    {
        string question = $"Contexto: Se acabó el juego. Datos del juego: {gameData}. " +
                          "Dale una corta despedida al usuario basada en su desempeño (máximo 200 caracteres).";
        StartCoroutine(PostRequest(question, callback));
    }

    public void AssistantFeedback3(string gameData, System.Action<string> callback)
    {
        string question = $"Contexto: Se acabó el juego. Aquí están los datos del juego:\n" +
                          $"{gameData}\n" +
                          "Analiza el desempeño del usuario considerando los siguientes puntos:\n" +
                          "- ¿Cómo afectaron sus decisiones al saldo final?\n" +
                          "- ¿Qué podría mejorar en sus estrategias?\n" +
                          "- ¿Qué hizo bien y qué debería repetir en el futuro?\n" +
                          "Proporciona una retroalimentación breve y clara basada en estos datos." +
                          "Incluye un consejo financiero breve relacionado con el tema del juego. " +
                          "Considera un máximo 300 caracteres y no dejes saltos de línea.";
        ;
        StartCoroutine(PostRequest(question, callback));
    }


    public void AskFinancialQuestionChat(string question, System.Action<string> callback)//chat
    {
        StartCoroutine(PostRequest(question, callback));
    }

    private IEnumerator PostRequest(string question, System.Action<string> callback)
    {
        string fullPrompt = financialContext + question;

        RequestBody requestBody = new RequestBody
        {
            contents = new Content[] {
                new Content {
                    role = "user",
                    parts = new Part[] {
                        new Part { text = fullPrompt }
                    }
                }
            }
        };

        string jsonBody = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        string requestUrl = $"{apiUrl}?key={apiKey}";

        using (UnityWebRequest request = new UnityWebRequest(requestUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ResponseBody response = JsonUtility.FromJson<ResponseBody>(request.downloadHandler.text);
                if (response.candidates != null && response.candidates.Length > 0)
                {
                    string responseText = response.candidates[0].content.parts[0].text;
                    Debug.Log("Texto recibido del asistente: " + responseText);
                    callback(responseText);
                }
                else
                {
                    Debug.LogWarning("No se recibieron candidatos de respuesta del asistente.");
                    callback("Disculpa, no pude procesar tu consulta financiera. ¿Podrías reformularla?");
                }
            }
            else
            {
                Debug.LogError("Error al conectar con el asistente. Código: " + request.result);
                callback("Error de conexión con el servicio financiero. Intenta nuevamente más tarde.");
            }
        }
    }

}
