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
        if (knowledgeSliders == null || knowledgeSliders.Length == 0)
        {
            Debug.LogError("knowledgeSliders no está configurado.");
            return;
        }

        foreach (Slider slider in knowledgeSliders)
        {
            if (slider == null)
            {
                Debug.LogError("Un slider en knowledgeSliders es null.");
                continue;
            }

            PlayerData.SetKnowledge(slider.name, slider.value);
        }

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager no encontrado.");
            return;
        }

        gameManager.ShowInterestSelectionScreen();
    }

}