using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EvaluationScreen : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> evaluationQuestions = new List<QuestionSO>();
    List<QuestionSO> questions = new List<QuestionSO>();
    QuestionSO currentQuestion;

    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;

    int correctAnswerIndex;
    bool hasAnsweredEarly;

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;

    [Header("Scoring")]
    ScoreKeeper scoreKeeperr;

    [Header("Results Screen")]
    [SerializeField] ResultsScreen resultsScreen;

    void Awake()
    {
        scoreKeeperr = FindObjectOfType<ScoreKeeper>();
        if (scoreKeeperr == null)
        {
            Debug.LogError("No se encontró ScoreKeeper.");
        }
        questions = new List<QuestionSO>(evaluationQuestions);
    }

    void Start()
    {
        if (questions.Count == 0)
        {
            Debug.LogError("No hay preguntas disponibles para la evaluación.");
        }
        else
        {
            LoadNextEvaluationQuestion();
        }
    }

    void Update()
    {

    }

    public void OnEvaluationAnswerSelected(int index)
    {
        if (currentQuestion == null)
        {
            Debug.LogError("currentQuestion es null.");
            return;
        }

        hasAnsweredEarly = true;
        ShowEvaluationAnswer(index);
        SetEvaluationButtonState(false);
    }

    void ShowEvaluationAnswer(int index)
    {
        if (currentQuestion == null)
        {
            Debug.LogError("currentQuestion es null.");
            return;
        }

        if (index < 0 || index >= answerButtons.Length)
        {
            Debug.LogError("Índice de respuesta fuera de rango.");
            return;
        }

        Image buttonImage;

        if (index == currentQuestion.GetCorrectAnswerIndex())
        {
            questionText.text = "¡Correcto!";
            buttonImage = answerButtons[index].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = correctAnswerSprite;
            }
            scoreKeeperr.IncrementCorrectAnswers();
        }
        else
        {
            correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();
            string correctAnswer = currentQuestion.GetAnswer(correctAnswerIndex);
            questionText.text = "Respuesta correcta:\n" + correctAnswer;
            buttonImage = answerButtons[correctAnswerIndex].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = correctAnswerSprite;
            }
        }

        StartCoroutine(WaitAndLoadNextQuestion(2f));
    }

    IEnumerator WaitAndLoadNextQuestion(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadNextEvaluationQuestion();
    }

    void LoadNextEvaluationQuestion()
    {
        if (questions.Count > 0)
        {
            SetEvaluationButtonState(true);
            SetDefaultEvaluationButtonSprites();
            GetRandomEvaluationQuestion();
            DisplayEvaluationQuestion();
            scoreKeeperr.IncrementQuestionsSeen();
        }
        else
        {
            Debug.Log("Evaluación completada.");
            ShowEvaluationResultsScreen();
        }
    }

    void GetRandomEvaluationQuestion()
    {
        if (questions.Count == 0)
        {
            Debug.LogError("No hay más preguntas disponibles.");
            return;
        }

        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];
        questions.RemoveAt(index);
    }

    void DisplayEvaluationQuestion()
    {
        if (currentQuestion == null)
        {
            Debug.LogError("currentQuestion es null.");
            return;
        }

        questionText.text = currentQuestion.GetQuestion();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = currentQuestion.GetAnswer(i);
            }
        }
    }

    void SetEvaluationButtonState(bool state)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Button button = answerButtons[i].GetComponent<Button>();
            if (button != null)
            {
                button.interactable = state;
            }
        }
    }

    void SetDefaultEvaluationButtonSprites()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Image buttonImage = answerButtons[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = defaultAnswerSprite;
            }
        }
    }

    void ShowEvaluationResultsScreen()
    {
        resultsScreen.gameObject.SetActive(true);

        resultsScreen.DisplayResults(scoreKeeperr.CalculateScore());

        FindObjectOfType<GameManager>().ShowResultsScreen();
    }
}
