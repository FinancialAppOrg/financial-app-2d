using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BusinessOptionsManager : MonoBehaviour
{
    public gameManager gameManager;  // Referencia al GameManager
    public Slider investmentSlider;  // Slider para seleccionar la cantidad a invertir
    public TextMeshProUGUI investmentAmountText;  // Muestra la cantidad de inversión
    public Button investButton;  // Botón de "Invertir"
    public TextMeshProUGUI resultText;  // Texto para mostrar el resultado de la inversión

    void Start()
    {
        investButton.onClick.AddListener(Invest);  // Asocia el evento del botón "Invertir"
        investmentSlider.maxValue = gameManager.GetBalance();  // Establece el máximo valor del slider al saldo disponible
        UpdateInvestmentAmountText();  // Muestra el valor inicial del slider
    }

    void Update()
    {
        UpdateInvestmentAmountText();  // Actualiza el monto de inversión mientras el slider se mueve
    }

    void UpdateInvestmentAmountText()
    {
        int investmentAmount = Mathf.FloorToInt(investmentSlider.value);
        investmentAmountText.text = "Inversión: $" + investmentAmount.ToString();
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
            resultText.text = "Saldo insuficiente para esta inversión.";
            return;
        }

        // Calcular el rendimiento de la inversión (por ejemplo, un rendimiento aleatorio para empresas)
        float performance = Random.Range(0.01f, 0.04f);  // Ganancia entre 1% y 4%
        int profit = Mathf.FloorToInt(investmentAmount * performance);

        // Actualizamos el saldo del jugador
        gameManager.UpdateBalance(currentBalance + profit);

        // Mostrar el resultado de la inversión
        resultText.text = "Tu inversión de $" + investmentAmount + " en empresas ha resultado en un aumento de $" + profit + ". Ahora tienes $" + (currentBalance + profit) + ".";
    }

    public int CalculateReturn(int investmentAmount)
    {
        // Porcentaje de retorno de Negocios (7%)
        return Mathf.FloorToInt(investmentAmount * 0.07f);
    }
}
