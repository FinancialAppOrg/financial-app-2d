using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CryptoOptionsManager : MonoBehaviour
{
    public gameManager gameManager;  // Referencia al GameManager
    public Slider investmentSlider;  // Slider para seleccionar la cantidad a invertir
    public TextMeshProUGUI investmentAmountText;  // Muestra la cantidad de inversi�n
    public Button investButton;  // Bot�n de "Invertir"
    public TextMeshProUGUI resultText;  // Texto para mostrar el resultado de la inversi�n

    void Start()
    {
        investButton.onClick.AddListener(Invest);  // Asocia el evento del bot�n "Invertir"
        investmentSlider.maxValue = gameManager.GetBalance();  // Establece el m�ximo valor del slider al saldo disponible
        UpdateInvestmentAmountText();  // Muestra el valor inicial del slider
    }

    void Update()
    {
        UpdateInvestmentAmountText();  // Actualiza el monto de inversi�n mientras el slider se mueve
    }

    void UpdateInvestmentAmountText()
    {
        int investmentAmount = Mathf.FloorToInt(investmentSlider.value);
        investmentAmountText.text = "Inversi�n: $" + investmentAmount.ToString();
    }

    public void Invest()
    {
        int investmentAmount = Mathf.FloorToInt(investmentSlider.value);
        int currentBalance = gameManager.GetBalance();

        if (investmentAmount <= 0)
        {
            resultText.text = "Debes invertir una cantidad mayor que 0.";
            Debug.Log("Inversi�n no v�lida, monto menor o igual a 0.");
            return;
        }

        if (investmentAmount > currentBalance)
        {
            resultText.text = "Saldo insuficiente para esta inversi�n.";
            Debug.Log("Saldo insuficiente.");
            return;
        }

        // Calcular el rendimiento de la inversi�n (por ejemplo, un rendimiento aleatorio para criptomonedas)
        float performance = Random.Range(0.02f, 0.05f);  // Ganancia entre 2% y 5%
        int profit = Mathf.FloorToInt(investmentAmount * performance);

        // Actualizamos el saldo del jugador
        gameManager.UpdateBalance(currentBalance + profit);

        // Mostrar el resultado de la inversi�n
        resultText.text = "Tu inversi�n de $" + investmentAmount + " en criptomonedas ha resultado en un aumento de $" + profit + ". Ahora tienes $" + (currentBalance + profit) + ".";
        Debug.Log("Inversi�n realizada. Saldo actualizado: " + (currentBalance + profit));
    }

    public int CalculateReturn(int investmentAmount)
    {
        // Porcentaje de retorno de Crypto (5%)
        return Mathf.FloorToInt(investmentAmount * 0.05f);
    }
}
