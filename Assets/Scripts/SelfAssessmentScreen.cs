using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SelfAssessmentScreen : MonoBehaviour
{
    [SerializeField] Slider ahorroSlider;
    [SerializeField] Slider inversionSlider;
    [SerializeField] Slider creditoDeudasSlider;
    [SerializeField] Button continueButton;

    void Start()
    {
        continueButton.onClick.AddListener(OnContinueClicked);

        ahorroSlider.value = PlayerData.GetTopicSelfAssessmentLevel("ahorro");
        inversionSlider.value = PlayerData.GetTopicSelfAssessmentLevel("inversion");
        creditoDeudasSlider.value = PlayerData.GetTopicSelfAssessmentLevel("credito-deudas");
    }

    void OnContinueClicked()
    {
        if (ahorroSlider == null || inversionSlider == null || creditoDeudasSlider == null)
        {
            Debug.LogError("Uno o más sliders no están configurados.");
            return;
        }

        int nivelAhorro = Mathf.RoundToInt(ahorroSlider.value);
        int nivelInversion = Mathf.RoundToInt(inversionSlider.value);
        int nivelCreditoDeudas = Mathf.RoundToInt(creditoDeudasSlider.value);

        PlayerData.SetTopicSelfAssessmentLevel("ahorro", nivelAhorro);
        PlayerData.SetTopicSelfAssessmentLevel("inversion", nivelInversion);
        PlayerData.SetTopicSelfAssessmentLevel("credito-deudas", nivelCreditoDeudas);

        PlayerData.SetSelfAssessmentCompleted(true);

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ShowInterestSelectionScreen();
        }
        else
        {
            Debug.LogError("GameManager no encontrado.");
        }

        //StartCoroutine(SendSelfAssessmentData(nivelAhorro, nivelInversion, nivelCreditoDeudas));
    }
}