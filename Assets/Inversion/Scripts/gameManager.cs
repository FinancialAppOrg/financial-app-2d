using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    public GameObject welcomeScreen;
    public GameObject cryptoOptionsScreen;  // Pantalla de opciones para Crypto
    public GameObject realEstateOptionsScreen;  // Pantalla de opciones para Bienes raíces
    public GameObject businessOptionsScreen;//business
    public GameObject riskOptionsScreen;
    public GameObject interestOptionsScreen;
    public GameObject resultsScreen;
    public TextMeshProUGUI balanceText; //text
    public GameObject playScreen;
    public GameObject summaryScreen;
    public GameObject player;// Referencia al objeto del jugador    
    public playerController playerController;
    public int balance = 1025;
    private Animator animator;
    public popManager popManager;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (popManager == null)
            popManager = FindObjectOfType<popManager>();
        UpdateBalance(balance);  // Inicializa el balance en la UI
    }

    public void ShowWelcomeScreen()
    {
        Debug.Log("Mostrando pantalla de bienvenida...");

        // Desactivar la pantalla de juego (playScreen)
        playScreen.SetActive(false);
        resultsScreen.SetActive(false);
        cryptoOptionsScreen.SetActive(false);
        realEstateOptionsScreen.SetActive(false);
        businessOptionsScreen.SetActive(false);
        riskOptionsScreen.SetActive(false);
        interestOptionsScreen.SetActive(false);
        welcomeScreen.SetActive(true);
        summaryScreen.SetActive(false);
        UpdateBalance(balance);
    }
    public void ShowInvestmentOptions(string areaName)
    {
        // Mostrar las opciones específicas según el área seleccionada
        popManager.ShowAreaPanel(areaName);
        switch (areaName)
        {
            case "Crypto":
                cryptoOptionsScreen.SetActive(true);
                realEstateOptionsScreen.SetActive(false);
                businessOptionsScreen.SetActive(false);
                riskOptionsScreen.SetActive(false);
                interestOptionsScreen.SetActive(false);
                break;
            case "State":
                realEstateOptionsScreen.SetActive(true);
                cryptoOptionsScreen.SetActive(false);
                businessOptionsScreen.SetActive(false);
                riskOptionsScreen.SetActive(false);
                interestOptionsScreen.SetActive(false);
                break;
            case "Business":
                businessOptionsScreen.SetActive(true);
                cryptoOptionsScreen.SetActive(false);
                realEstateOptionsScreen.SetActive(false);
                riskOptionsScreen.SetActive(false);
                interestOptionsScreen.SetActive(false);
                break;
            case "Risk":
                businessOptionsScreen.SetActive(false);
                cryptoOptionsScreen.SetActive(false);
                realEstateOptionsScreen.SetActive(false);
                riskOptionsScreen.SetActive(true);
                interestOptionsScreen.SetActive(false);
                break;
            case "Interest":
                businessOptionsScreen.SetActive(false);
                cryptoOptionsScreen.SetActive(false);
                realEstateOptionsScreen.SetActive(false);
                riskOptionsScreen.SetActive(false);
                interestOptionsScreen.SetActive(true);
                break;
        }
    }

    public int GetBalance()
    {
        return balance;
    }


    public void UpdateBalance(int newBalance)
    {
        balance = newBalance;
        balanceText.text = "$" + balance.ToString();  // Actualiza el texto del balance
    }
    public void CheckPlayerProgress()
    {
        if (balance >= 1070)
        {
            // Mostrar pantalla de victoria
            popManager.OpenResult();
        }
        else if (balance <= 300)
        {
            // Mostrar pantalla de pérdida
            //summaryScreen.SetActive(false);
        }
    }
 
}
