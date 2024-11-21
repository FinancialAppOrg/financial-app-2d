using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaInteraction : MonoBehaviour
{
    public gameManager gameManager;  // Referencia al GameManager
    public playerController playerController;  // Referencia al PlayerController
    public string areaName;  // Nombre del área (Crypto, Real Estate, Business)

    void Start()
    {
        playerController = FindObjectOfType<playerController>();  // Obtener referencia al playerController
        gameManager = FindObjectOfType<gameManager>();
    }

    void OnMouseDown()
    {
        if (GetComponent<Collider2D>().enabled)
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);  // Posición del objetivo

            // Mover al jugador al área seleccionada
            playerController.MoveToArea(targetPosition);

            // Llamar a la función para mostrar las opciones de inversión para el área seleccionada
            gameManager.ShowInvestmentOptions(areaName);  // Pasamos el nombre del área seleccionado
        }
    }
}
