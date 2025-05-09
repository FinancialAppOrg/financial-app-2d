using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CryptoOptionsManager : MonoBehaviour
{
    public gameManager gameManager;  // Referencia al GameManager
    public TextMeshProUGUI investmentAmountText;  // Muestra la cantidad de inversi�n
    public Button optionButton1;  // Bot�n para la primera opci�n
    public Button optionButton2;  // Bot�n para la segunda opci�n
    public Button optionButton3;
    public TextMeshProUGUI resultText;  // Texto para mostrar el resultado de la inversi�n

    void Start()
    {
        optionButton1.onClick.AddListener(() => Invest(1));
        optionButton2.onClick.AddListener(() => Invest(2));
        optionButton3.onClick.AddListener(() => Invest(3));
    }

    public void Invest(int optionId)
    {
        Debug.Log("gameManager: " + gameManager);  // Verificar si es null

        if (gameManager == null)
        {
            Debug.LogError("gameManager es nulo en el m�todo Invest().");
            return;
        }
        int currentBalance = gameManager.GetBalance();
        int impactAmount = GetImpactAmount(optionId);

        if (impactAmount == 0)
        {
            resultText.text = "Opci�n no v�lida.";
            Debug.Log("Opci�n no v�lida seleccionada.");
            return;
        }

        int newBalance = currentBalance + impactAmount;
        gameManager.UpdateBalance(newBalance);

        string resultMessage = impactAmount >= 0 ? "Ganaste $" + impactAmount : "Perdiste $" + Mathf.Abs(impactAmount);
        resultText.text = "Resultado: " + resultMessage + ". Saldo actual: $" + newBalance;

        Debug.Log("Inversi�n realizada. Saldo actualizado: " + newBalance);
    }
    private int GetImpactAmount(int optionId)
    {
        switch (optionId)
        {
            case 1: return -50;
            case 2: return -50;
            case 3: return 100;
            default: return 0;
        }
    }
 
}
