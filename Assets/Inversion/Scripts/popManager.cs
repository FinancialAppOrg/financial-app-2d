using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class popManager : MonoBehaviour
{
    public GameObject welcomeScreen;
    public GameObject optionsScreen;
    public GameObject initialScreen;
    public GameObject instructionsScreen;
    public GameObject playScreen;
    public GameObject resultsScreen;
    public GameObject summaryScreen;
    public GameObject chatScreen;
    public Button assistantIcon;
    public GameObject[] investmentAreas;
    public GameObject player;
    public UIManager uIManager;
    private int areaIndicador;
    public GameObject glossaryScreen;

    void Start()
    {
        ActivateInvestmentAreas(false);
        playScreen.SetActive(false);
        welcomeScreen.SetActive(false);
        glossaryScreen.SetActive(false);
    }

    public void OpenGlossaryScreen()
    {
        glossaryScreen.SetActive(true);
        player.SetActive(false);
    }

    public void CloseGlossaryScreen()
    {
        glossaryScreen.SetActive(false);
        player.SetActive(true);
    }


    public void ShowWelcomeScreen()
    {
        OpenWelcomePanel();//welcomeScreen.SetActive(true);
    }
    // close
    public void CloseWelcomePanel()
    {
        welcomeScreen.SetActive(false);
        ActivateInvestmentAreas(true);
        assistantIcon.gameObject.SetActive(true);
    }
   
    public void CloseOptionsPanel()
    {
        optionsScreen.SetActive(false);
        ActivateInvestmentAreas(true);
    }

    public void CloseOptionsPanelToResult()
    {
        optionsScreen.SetActive(false);
        //resultsScreen.SetActive(true);
        Invoke(nameof(ActivateResultsScreen), 2f);
    }
    public void OpenAssistantPanel()
    {
        optionsScreen.SetActive(false);
        //resultsScreen.SetActive(true);
        Invoke(nameof(ActivateResultsScreen), 2f);
    }
    public void ActivateResultsScreen()
    {
        resultsScreen.SetActive(true);
        assistantIcon.gameObject.SetActive(false);
    }
    public void OpenWelcomePanel()
    {
        playScreen.SetActive(false);
        resultsScreen.SetActive(false);
        optionsScreen.SetActive(false);
        summaryScreen.SetActive(false);
        chatScreen.SetActive(false);
        assistantIcon.gameObject.SetActive(false);
        uIManager.ShowWelcomeAssistant();
        Invoke(nameof(ActivateWelcomeScreen), 2.2f);
    }
    public void ActivateWelcomeScreen()
    {
        welcomeScreen.SetActive(true);
    }
    public void OpenResult()
    {
        resultsScreen.SetActive(false);
        summaryScreen.SetActive(true);
        player.SetActive(false);
    }
    public void CloseResultPanel()
    {
        resultsScreen.SetActive(false);
        assistantIcon.gameObject.SetActive(true);
        ActivateInvestmentAreas(true);
    }
    public void CloseInitialPanel()
    {
        initialScreen.SetActive(false);
        instructionsScreen.SetActive(true);
    }
    public void CloseChatPanel()
    {
        assistantIcon.gameObject.SetActive(true);
        chatScreen.SetActive(false);
        player.SetActive(true);
    }
    public void CloseInstructionsPanel()
    {
        //Debug.Log("Cerrando panel de instrucciones...");
        instructionsScreen.SetActive(false);
        playScreen.SetActive(true);
        ActivateInvestmentAreas(false);
    }
    //open

    public void OpenChatPanel()
    {
        assistantIcon.gameObject.SetActive(false);
        chatScreen.SetActive(true);
        resultsScreen.SetActive(false);
        optionsScreen.SetActive(false);
        welcomeScreen.SetActive(false);
        player.SetActive(false);
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
        Debug.Log("ShowAreaPanel llamado con: " + areaName);
        OptionsManager optionsManager = FindObjectOfType<OptionsManager>();
        switch (areaName)
        {
            case "Crypto":
                optionsScreen.SetActive(true);
                resultsScreen.SetActive(false);
                ActivateInvestmentAreas(false);
                // Cargar datos para el área seleccionada
                if (optionsManager != null)
                {
                    optionsManager.LoadSituacionDataForSelectedArea();
                }
                break;
            case "State":
                optionsScreen.SetActive(true);
                resultsScreen.SetActive(false);
                ActivateInvestmentAreas(false);
                if (optionsManager != null)
                {
                    optionsManager.LoadSituacionDataForSelectedArea();
                }
                break;
            case "Business":
                optionsScreen.SetActive(true);
                resultsScreen.SetActive(false);
                ActivateInvestmentAreas(false);
                if (optionsManager != null)
                {
                    optionsManager.LoadSituacionDataForSelectedArea();
                }
                break;
            case "Risk":
                optionsScreen.SetActive(true);
                resultsScreen.SetActive(false);
                ActivateInvestmentAreas(false);
                if (optionsManager != null)
                {
                    optionsManager.LoadSituacionDataForSelectedArea();
                }
                break;
            case "Interest":
                optionsScreen.SetActive(true);
                resultsScreen.SetActive(false);
                ActivateInvestmentAreas(false);
                if (optionsManager != null)
                {
                    optionsManager.LoadSituacionDataForSelectedArea();
                }
                break;
            default:
                Debug.LogWarning("Área no reconocida: " + areaName);
                break;
        }
    }
    public int GetAreaIndicador(string areaName)
    {
        Debug.Log("GetAreaIndicador - areaName recibido: " + areaName);
        switch (areaName)
        {
            case "Crypto":
                areaIndicador = 1;
                break;
            case "State":
                areaIndicador = 2;
                break;
            case "Business":
                areaIndicador = 3;
                break;
            case "Risk":
                areaIndicador = 4;
                break;
            case "Interest":
                areaIndicador = 5;
                break;
            default:
                Debug.LogWarning("Área Indicador no reconocida: " + areaName);
                break;
        }
        Debug.Log("Indicador retornado: " + areaIndicador);
        return areaIndicador;
    }

}
