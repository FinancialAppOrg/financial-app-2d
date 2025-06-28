using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ScoreScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] Button menuButton;
    //ScoreKeeper scoreKeeper;
    //private int monedas;
    //private int aciertos;
    private GameManager gameManager;

    private const string api_url = "https://financeapp-backend-production.up.railway.app/api/v1/quizz/summary/";

    //void Awake()
    //{
    //    scoreKeeper = FindObjectOfType<ScoreKeeper>();
    //}

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuButtonClicked);
        }
    }

    //public void ShowFinalScore()
    //{
    //    monedas = PlayerPrefs.GetInt("quizz_monedas", 0);
    //    aciertos = PlayerPrefs.GetInt("quizz_aciertos", 0);
    //    finalScoreText.text = "Felicidades!\nObtuviste " + aciertos + " preguntas correctas y " + monedas + " moneda coleccionable";
    //
    //    //finalScoreText.text = "Felicidades!\nTienes " + scoreKeeper.CalculateScoreQuizz() + " Puntos de conocimiento";
    // }

    public void ShowFinalScore(int quizzId)
    {
        StartCoroutine(FetchQuizzSummary(quizzId));
    }

    private IEnumerator FetchQuizzSummary(int quizzId)
    {
        string url = api_url + quizzId;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error al obtener el resumen del quizz: {request.error}");
                finalScoreText.text = "Error al cargar los resultados. Intenta nuevamente.";
            }
            else
            {
                QuizzSummaryResponse response = JsonUtility.FromJson<QuizzSummaryResponse>(request.downloadHandler.text);

                finalScoreText.text = $"Felicidades!\nObtuviste {response.preguntas_correctas} preguntas correctas y {response.monedas_ganadas} moneda coleccionable.";
            }
        }
    }

    private void OnMenuButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.ShowMenuScreen();
        }
        else
        {
            Debug.LogError("GameManager no encontrado");
        }
    }

    [System.Serializable]
    private class QuizzSummaryResponse
    {
        public int preguntas_correctas;
        public int monedas_ganadas;
    }
}
