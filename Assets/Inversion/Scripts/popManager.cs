using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popManager : MonoBehaviour
{
    public GameObject welcomeScreen;
    public GameObject cryptoOptionsScreen;
    public GameObject realEstateOptionsScreen;  // Pantalla de opciones para Bienes raíces
    public GameObject businessOptionsScreen;//business
    public GameObject initialScreen;
    public GameObject instructionsScreen;
    public GameObject playScreen;
    public GameObject resultsScreen;
    public GameObject summaryScreen;
    public GameObject[] investmentAreas;

    void Start()
    {
        // Desactiva los colliders inicialmente
        ActivateInvestmentAreas(false);
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
        resultsScreen.SetActive(false);
    }
    public void CloseStatePanel()
    {
        resultsScreen.SetActive(false);
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
    public void CloseInstructionsPanel()
    {
        Debug.Log("Cerrando panel de instrucciones...");
        instructionsScreen.SetActive(false);
        playScreen.SetActive(true);
        ActivateInvestmentAreas(false);
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
                break;
            case "State":
                realEstateOptionsScreen.SetActive(true);
                cryptoOptionsScreen.SetActive(false);
                businessOptionsScreen.SetActive(false);
                break;
            case "Business":
                businessOptionsScreen.SetActive(true);
                cryptoOptionsScreen.SetActive(false);
                realEstateOptionsScreen.SetActive(false);
                break;
            default:
                Debug.LogWarning("Área no reconocida: " + areaName);
                break;
        }
    }


}
