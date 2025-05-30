using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    public GameManager gameManager;
    List<Question> questions = new List<Question>();
    Question currentQuestion;


    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;

    int correctAnswerIndex;
    bool hasAnsweredEarly ;
    int selectedAnswerIndex = -1;
    bool hasProcessedAnswer = false;
    bool quizCompleted = false;
    bool isInitialized = false;

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;
    [SerializeField] Sprite selectedAnswerSprite;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    Timer timer;


    [SerializeField] TextMeshProUGUI questionText;

    public delegate void QuizCompleted();
    public event QuizCompleted OnQuizCompleted;

    void Awake()
    {
        timer = FindObjectOfType<Timer>();

    }
    public void Init(List<Question> loadedQuestions)
    {
        Debug.Log($"Preguntas recibidas: {loadedQuestions?.Count ?? 0}");
        questions = loadedQuestions;
        quizCompleted = false;
        isInitialized = false;

        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("No hay preguntas disponibles para el quiz.");
            return;
        }

        Debug.Log($"Preguntas cargadas: {questions.Count}");
        isInitialized = true;
        Debug.Log("Llamando GetNextQuestion desde Init");
        GetNextQuestion();
    }

    void Update()
    {
        if (quizCompleted || !isInitialized) return;

        timerImage.fillAmount = timer.fillFraction;

        if (timer.loadNextQuestion)
        {
            hasAnsweredEarly = false;
            hasProcessedAnswer = false;
            GetNextQuestion();
            timer.loadNextQuestion = false;
        }
        else if(!hasAnsweredEarly && !timer.isAnsweringQuestion && !hasProcessedAnswer)
        {
            hasProcessedAnswer = true;
            DisplayAnswer(-1);
            SetButtonState(false);
        }
    }

    public void OnAnswerSelected(int index)
    {
        if (hasAnsweredEarly || hasProcessedAnswer) return;
        Debug.Log($"Respuesta seleccionada: {index}");

        hasAnsweredEarly = true;
        hasProcessedAnswer = true;
        selectedAnswerIndex = index;
        //
        SetSelectedButtonSprite(index);
        DisplayAnswer(index);
        SetButtonState(false);
        timer.CancelTimer();
    }
    void SetSelectedButtonSprite(int selectedIndex)
    {
        Debug.Log($"Cambiando sprite del botón {selectedIndex}");
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Image buttonImage = answerButtons[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                if (i == selectedIndex)
                {
                    buttonImage.sprite = selectedAnswerSprite;
                    Debug.Log($"Sprite cambiado a selectedAnswerSprite para botón {i}");
                }
                else
                {
                    buttonImage.sprite = defaultAnswerSprite;
                }
            }
            else
            {
                Debug.LogError($"No se encontró Image component en answerButtons[{i}]");
            }
        }
    }

    void DisplayAnswer(int index)
    {
        if (currentQuestion == null)
        {
            Debug.LogError("currentQuestion es null.");
            return;
        }

        Image buttonImage;
        int correctIndex = currentQuestion.opcion_correcta - 1; // del 1-4 al 0-3
        string[] answers = currentQuestion.GetAnswers();

        if (index == correctIndex && index != -1)// Verificar que no sea timeout
        {
            questionText.text = "¡Correcto!";
            buttonImage = answerButtons[index].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = correctAnswerSprite;
            }
        }
        else
        {
            if (index == -1)
            {
                questionText.text = "¡Tiempo agotado!\nLa respuesta correcta es:\n" + answers[correctIndex];

            } else {
                questionText.text = "La respuesta correcta es:\n" + answers[correctIndex];
            }
            buttonImage = answerButtons[correctIndex].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = correctAnswerSprite;
            }
        }

        // enviar respuesta al servidor SOLO UNA VEZ
        int quizId = currentQuestion.id_quizz;
        int questionId = currentQuestion.id_pregunta;
        int selectedOption = index == -1 ? 0 : index + 1; // 0 para timeout, 1-4 para respuestas
        Debug.Log($"Enviando respuesta: quizId={quizId}, questionId={questionId}, selectedOption={selectedOption}");
        gameManager.PostAnswerQuizz(quizId, questionId, selectedOption);
        
    }

    public void GetNextQuestion()
    {
        Debug.Log($"GetNextQuestion llamado - Preguntas restantes: {questions.Count}");
        if (quizCompleted) return;

        selectedAnswerIndex = -1;
        hasProcessedAnswer = false; // Reset para nueva pregunta

        if (questions.Count > 0)
        {
            SetButtonState(true);
            SetDefaultButtonSprites();
            GetRandomQuestion();
            DisplayQuestion();
            Debug.Log($"Pregunta mostrada - Preguntas restantes: {questions.Count}");
        }
        else
        {
            if (!quizCompleted)
            {
                quizCompleted = true;
                Debug.Log("Quiz completado - invocando evento");
                OnQuizCompleted?.Invoke();
            }
        }
        
    }

    void GetRandomQuestion()
    {
        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];
        questions.Remove(currentQuestion);
    }

    void DisplayQuestion()
    {
        if (currentQuestion == null)
        {
            Debug.LogError("currentQuestion es null.");
            return;
        }

        questionText.text = currentQuestion.texto_pregunta;
        Debug.Log($"Pregunta: {currentQuestion.texto_pregunta}");
        string[] answers = currentQuestion.GetAnswers();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = answers[i];
                Debug.Log($"Opción {i + 1}: {answers[i]}");
            }
        }

        // Iniciar el timer para esta pregunta
        if (timer != null)
        {
            timer.StartNewQuestion();
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
    public void SetQuestions(List<Question> loadedQuestions)
    {
        questions = new List<Question>(loadedQuestions);
    }


}

