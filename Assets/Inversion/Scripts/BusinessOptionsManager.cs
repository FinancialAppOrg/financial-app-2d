using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BusinessOptionsManager : MonoBehaviour
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
            return;
        }

        if (investmentAmount > currentBalance)
        {
            resultText.text = "Saldo insuficiente para esta inversi�n.";
            return;
        }

        // Calcular el rendimiento de la inversi�n (por ejemplo, un rendimiento aleatorio para empresas)
        float performance = Random.Range(0.01f, 0.04f);  // Ganancia entre 1% y 4%
        int profit = Mathf.FloorToInt(investmentAmount * performance);

        // Actualizamos el saldo del jugador
        gameManager.UpdateBalance(currentBalance + profit);

        // Mostrar el resultado de la inversi�n
        resultText.text = "Tu inversi�n de $" + investmentAmount + " en empresas ha resultado en un aumento de $" + profit + ". Ahora tienes $" + (currentBalance + profit) + ".";
    }

    public int CalculateReturn(int investmentAmount)
    {
        // Porcentaje de retorno de Negocios (7%)
        return Mathf.FloorToInt(investmentAmount * 0.07f);
    }
}
