using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryptoAreaInteraction : MonoBehaviour
{
    public gameManager gameManager;  // Referencia al GameManager
    public playerController playerController;  // Referencia al PlayerController
    public string areaName = "Crypto";
    void Start()
    {
        playerController = FindObjectOfType<playerController>();  // Obtener referencia al playerController
        gameManager = FindObjectOfType<gameManager>();
    }

    void OnMouseDown()
    {
        if (GetComponent<Collider2D>().enabled)
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);  // Posición del objetivo (CryptoArea)

            // Llamar a la función para mover al jugador
            playerController.MoveToArea(targetPosition);

            // Llamar a la función del gameManager para mostrar la pantalla de opciones de inversión
            gameManager.ShowInvestmentOptions(areaName);  // Activa las opciones de inversión

            // Si necesitas hacer algo más al activar la pantalla, lo puedes agregar aquí
        }

 

    }

}
