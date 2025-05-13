using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popManager : MonoBehaviour
{
    public GameObject welcomeScreen;
    public GameObject cryptoOptionsScreen;
    public GameObject realEstateOptionsScreen;  // Pantalla de opciones para Bienes raíces
    public GameObject businessOptionsScreen;//business
    public GameObject riskOptionsScreen;
    public GameObject interestOptionsScreen;
    public GameObject initialScreen;
    public GameObject instructionsScreen;
    public GameObject playScreen;
    public GameObject resultsScreen;
    public GameObject summaryScreen;
    public GameObject chatScreen;
    public GameObject[] investmentAreas;

    void Start()
    {
        // Desactiva los colliders inicialmente
        ActivateInvestmentAreas(false);
        playScreen.SetActive(false);
    }
    

    // close
    public void CloseWelcomePanel()
    {
        welcomeScreen.SetActive(false);
        ActivateInvestmentAreas(true);
    }
    public void CloseCrytoPanel()
    {
        cryptoOptionsScreen.SetActive(false);
    }
    public void CloseBusinessPanel()
    {
        businessOptionsScreen.SetActive(false);
    }
    public void CloseStatePanel()
    {
        realEstateOptionsScreen.SetActive(false);
    }
    public void CloseInterestPanel()
    {
        interestOptionsScreen.SetActive(false);
    }
    public void CloseRiskPanel()
    {
        riskOptionsScreen.SetActive(false);
    }
    public void CloseCrytoPanelToResult()
    {
        cryptoOptionsScreen.SetActive(false);
        resultsScreen.SetActive(true);
    }
    public void CloseBusinessPanelToResult()
    {
        businessOptionsScreen.SetActive(false);
        resultsScreen.SetActive(true);
    }
    public void CloseStatePanelToResult()
    {
        realEstateOptionsScreen.SetActive(false);
        resultsScreen.SetActive(true);
    }
    public void CloseRiskPanelToResult()
    {
        riskOptionsScreen.SetActive(false);
        resultsScreen.SetActive(true);
    }
    public void CloseInterestPanelToResult()
    {
        interestOptionsScreen.SetActive(false);
        resultsScreen.SetActive(true);
    }
    public void OpenResult()
    {
        resultsScreen.SetActive(false);
        summaryScreen.SetActive(true);
    }
    public void CloseInitialPanel()
    {
        initialScreen.SetActive(false);
        instructionsScreen.SetActive(true);
    }
    public void CloseChatPanel()
    {
        chatScreen.SetActive(false);
    }
    public void CloseInstructionsPanel()
    {
        Debug.Log("Cerrando panel de instrucciones...");
        instructionsScreen.SetActive(false);
        playScreen.SetActive(true);
        ActivateInvestmentAreas(false);
    }
    //open

    public void OpenChatPanel()
    {
        chatScreen.SetActive(true);
        resultsScreen.SetActive(false);
        cryptoOptionsScreen.SetActive(false);
        realEstateOptionsScreen.SetActive(false);
        businessOptionsScreen.SetActive(false);
        riskOptionsScreen.SetActive(false);
        interestOptionsScreen.SetActive(false);
        welcomeScreen.SetActive(false);
    }

    // Método para activar o desactivar los colliders de las áreas de inversión
    public void ActivateInvestmentAreas(bool activate)
    {
        foreach (GameObject area in investmentAreas)
        {
            // Desactivar o activar el collider de cada área
            area.GetComponent<Collider2D>().enabled = activate;
        }
    }

    public void ShowAreaPanel(string areaName)
    {
        switch (areaName)
        {
            case "Crypto":
                cryptoOptionsScreen.SetActive(true);
                realEstateOptionsScreen.SetActive(false);
                businessOptionsScreen.SetActive(false);
                riskOptionsScreen.SetActive(false);
                interestOptionsScreen.SetActive(false);
                resultsScreen.SetActive(false);
                break;
            case "State":
                realEstateOptionsScreen.SetActive(true);
                cryptoOptionsScreen.SetActive(false);
                businessOptionsScreen.SetActive(false);
                riskOptionsScreen.SetActive(false);
                interestOptionsScreen.SetActive(false);
                resultsScreen.SetActive(false);
                break;
            case "Business":
                businessOptionsScreen.SetActive(true);
                cryptoOptionsScreen.SetActive(false);
                realEstateOptionsScreen.SetActive(false);
                riskOptionsScreen.SetActive(false);
                interestOptionsScreen.SetActive(false);
                resultsScreen.SetActive(false);
                break;
            case "Risk":
                businessOptionsScreen.SetActive(false);
                cryptoOptionsScreen.SetActive(false);
                realEstateOptionsScreen.SetActive(false);
                riskOptionsScreen.SetActive(true);
                interestOptionsScreen.SetActive(false);
                resultsScreen.SetActive(false);
                break;
            case "Interest":
                businessOptionsScreen.SetActive(false);
                cryptoOptionsScreen.SetActive(false);
                realEstateOptionsScreen.SetActive(false);
                riskOptionsScreen.SetActive(false);
                interestOptionsScreen.SetActive(true);
                resultsScreen.SetActive(false);
                break;
            default:
                Debug.LogWarning("Área no reconocida: " + areaName);
                break;
        }
    }


}
