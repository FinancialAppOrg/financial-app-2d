using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> ahorroQuestionsBasic = new List<QuestionSO>();
    [SerializeField] List<QuestionSO> ahorroQuestionsIntermediate = new List<QuestionSO>();
    [SerializeField] List<QuestionSO> ahorroQuestionsAdvanced = new List<QuestionSO>();
    [SerializeField] List<QuestionSO> inversionQuestionsBasic = new List<QuestionSO>();
    [SerializeField] List<QuestionSO> inversionQuestionsIntermediate = new List<QuestionSO>();
    [SerializeField] List<QuestionSO> inversionQuestionsAdvanced = new List<QuestionSO>();
    List<QuestionSO> questions= new List<QuestionSO>();
    QuestionSO currentQuestion;


    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;

    int correctAnswerIndex;
    bool hasAnsweredEarly ;

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    Timer timer;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    public delegate void QuizCompleted();
    public event QuizCompleted OnQuizCompleted;

    void Awake()
    {
        timer = FindObjectOfType<Timer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();

        //GetNextQuestion();
        //DisplayQuestion();
    }

    void Start()
    {
        // Inicializar el quiz con preguntas
        if (questions.Count == 0)
        {
            Debug.LogError("No hay preguntas disponibles para el quiz.");
        }
    }

    void Update()
    {
        timerImage.fillAmount = timer.fillFraction;

        if (timer.loadNextQuestion)
        {
            hasAnsweredEarly = false;
            GetNextQuestion();
            timer.loadNextQuestion = false;
        }
        else if(!hasAnsweredEarly && !timer.isAnsweringQuestion)
        {
            DisplayAnswer(-1);
            SetButtonState(false);
            //OnAnswerSelected(-1);
            //hasAnsweredEarly = true;
        }
    }

    public void OnAnswerSelected(int index)
    {
        hasAnsweredEarly = true;
        DisplayAnswer(index);
        SetButtonState(false);
        timer.CancelTimer();
        scoreText.text = scoreKeeper.CalculateScore() + " PC";
    }

    void DisplayAnswer(int index)
    {
        if (currentQuestion == null)
        {
            Debug.LogError("currentQuestion es null.");
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
            scoreKeeper.IncrementCorrectAnswers();
        }
        else
        {
            correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();
            string correctAnswer = currentQuestion.GetAnswer(correctAnswerIndex);
            questionText.text = "La respuesta correcta es:\n" + correctAnswer;
            buttonImage = answerButtons[correctAnswerIndex].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = correctAnswerSprite;
            }
        }
    }

    void GetNextQuestion()
    {
        if (questions.Count > 0)
        {
            SetButtonState(true);
            SetDefaultButtonSprites();
            GetRandomQuestion();
            DisplayQuestion();
            scoreKeeper.IncrementQuestionsSeen();

        }
        else
        {
            //FindObjectOfType<GameManager>().HandleQuizCompleted();
           OnQuizCompleted?.Invoke();
        }
        
    }

    void GetRandomQuestion()
    {
        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];

        //if (questions.Contains(currentQuestion))
        //{
            questions.Remove(currentQuestion);

        //}
        
    }

    void DisplayQuestion()
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

    void SetButtonState(bool state)
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

    void SetDefaultButtonSprites()
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

    public void SetTopicAndLevel(string topic, string level)
    {
        switch (topic)
        {
            case "Ahorro":
                switch (level)
                {
                    case "Basic":
                        questions = new List<QuestionSO>(ahorroQuestionsBasic);
                        break;
                    case "Intermediate":
                        questions = new List<QuestionSO>(ahorroQuestionsIntermediate);
                        break;
                    case "Advanced":
                        questions = new List<QuestionSO>(ahorroQuestionsAdvanced);
                        break;
                }
                break;
            case "Inversion":
                switch (level)
                {
                    case "Basic":
                        questions = new List<QuestionSO>(inversionQuestionsBasic);
                        break;
                    case "Intermediate":
                        questions = new List<QuestionSO>(inversionQuestionsIntermediate);
                        break;
                    case "Advanced":
                        questions = new List<QuestionSO>(inversionQuestionsAdvanced);
                        break;
                }
                break;
            default:
                questions = new List<QuestionSO>(ahorroQuestionsBasic);
                break;
        }

        Debug.Log("Preguntas disponibles: " + questions.Count);

        if (questions.Count == 0)
        {
            Debug.LogError("No hay preguntas disponibles para el tema y nivel seleccionados: " + topic + " - " + level);
        }
    }
}
