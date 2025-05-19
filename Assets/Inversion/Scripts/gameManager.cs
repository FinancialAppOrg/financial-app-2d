using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    public TextMeshProUGUI balanceText; //text
    public GameObject playScreen;
    public GameObject player;// Referencia al objeto del jugador    
    public playerController playerController;
    public int balance = 1025;
    private Animator animator;
    public popManager popManager;
    public Button testButton;
    public TextMeshProUGUI saldoFinalText;
    public Button assistantIcon;
    private int clickCount = 0;
    private int maxClicks = 5;
    private string selectedArea;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (popManager == null)
            popManager = FindObjectOfType<popManager>();
        UpdateBalance(balance);  // Inicializa el balance en la UI
        //finalizar
        if (testButton != null)
        {
            testButton.onClick.AddListener(ContarClicks);
        }
    }

    public void ShowWelcomeScreen()
    {
        Debug.Log("Mostrando pantalla de bienvenida...");
        popManager.ShowWelcomeScreen(); 
    }
    public void ShowInvestmentOptions(string areaName)
    {
        // Mostrar las opciones específicas según el área seleccionada
        popManager.ShowAreaPanel(areaName);
    }
    
    public int GetAreaIndicador(string areaName)
    {
        // Mostrar las opciones específicas según el área seleccionada
        return popManager.GetAreaIndicador(areaName);
    }

    public void SetSelectedArea(string areaName)
    {
        selectedArea = areaName;
        Debug.Log("Área establecida en GameManager: " + selectedArea);
    }

    public string GetSelectedArea()
    {
        return selectedArea;
    }

    public int GetBalance()
    {
        return balance;
    }


    public void UpdateBalance(int newBalance)
    {
        balance = newBalance;
        balanceText.text = "$" + balance.ToString();  // Actualiza el texto del balance
        saldoFinalText.text = "S/" + balance.ToString() + "\nsaldo final";
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

    public void ContarClicks()
    {
        clickCount++;

        Debug.Log("Click número: " + clickCount);

        if (clickCount >= maxClicks)
        {
            FinalizarJuego();
        }
    }
    public void FinalizarJuego()
    {
        Debug.Log("Juego finalizado.");
        popManager.OpenResult();
    }

}
