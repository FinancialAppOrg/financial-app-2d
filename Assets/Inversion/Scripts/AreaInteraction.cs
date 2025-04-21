using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaInteraction : MonoBehaviour
{
    public gameManager gameManager;  // Referencia al GameManager
    public playerController playerController;  // Referencia al PlayerController
    public string areaName;  // Nombre del �rea (Crypto, Real Estate, Business)

    void Start()
    {
        playerController = FindObjectOfType<playerController>();  // Obtener referencia al playerController
        gameManager = FindObjectOfType<gameManager>();
    }

    void OnMouseDown()
    {
        if (GetComponent<Collider2D>().enabled)
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);  // Posici�n del objetivo

            // Mover al jugador al �rea seleccionada
            playerController.MoveToArea(targetPosition);

            // Llamar a la funci�n para mostrar las opciones de inversi�n para el �rea seleccionada
            gameManager.ShowInvestmentOptions(areaName);  // Pasamos el nombre del �rea seleccionado
        }
    }
}
