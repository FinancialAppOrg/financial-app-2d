using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaInteraction : MonoBehaviour
{
    public gameManager gameManager;  // Referencia al GameManager
    public playerController playerController;  // Referencia al PlayerController
    public string areaName;  // Nombre del área (Crypto, Real Estate, Business)
    private bool areaUsada = true;

    void Start()
    {
        playerController = FindObjectOfType<playerController>();  // Obtener referencia al playerController
        gameManager = FindObjectOfType<gameManager>();
    }

    void OnMouseDown()
    {
        if (GetComponent<Collider2D>().enabled && areaUsada==true )
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);  // Posición del objetivo

            // Mover al jugador al área seleccionada
            playerController.MoveToArea(targetPosition);

            // Llamar a la función para mostrar las opciones para el área seleccionada
            gameManager.SetSelectedArea(areaName);
            gameManager.ShowInvestmentOptions(areaName);

            Debug.Log("Área seleccionada: " + areaName);
        }
    }

    public void DesactivarArea()
    {
        areaUsada = false;
    }
}
