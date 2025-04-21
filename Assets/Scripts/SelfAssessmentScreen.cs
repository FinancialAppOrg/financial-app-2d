using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelfAssessmentScreen : MonoBehaviour
{
    [SerializeField] Slider[] knowledgeSliders;
    [SerializeField] Button continueButton;

    void Start()
    {
        continueButton.onClick.AddListener(OnContinueClicked);
    }

    void OnContinueClicked()
    {
        foreach (Slider slider in knowledgeSliders)
        {
            PlayerData.SetKnowledge(slider.name, slider.value);
        }
        FindObjectOfType<GameManager>().ShowOptionsScreen();
    }
}