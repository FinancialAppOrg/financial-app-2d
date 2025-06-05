using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTopicScreen : MonoBehaviour
{
    [SerializeField] Button[] topicButtons;

    void Start()
    {
        foreach(Button button in topicButtons)
        {
            button.onClick.AddListener(() => OnTopicSelected(button.name));
        }

    }

    void OnTopicSelected(string topic)
    {
        Debug.Log("Topic selected: " + topic);
        FindObjectOfType<GameManager>().StartQuiz(topic);
    }

}
