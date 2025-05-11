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
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);  // Posici�n del objetivo (CryptoArea)

            // Llamar a la funci�n para mover al jugador
            playerController.MoveToArea(targetPosition);

            // Llamar a la funci�n del gameManager para mostrar la pantalla de opciones de inversi�n
            gameManager.ShowInvestmentOptions(areaName);  // Activa las opciones de inversi�n

            // Si necesitas hacer algo m�s al activar la pantalla, lo puedes agregar aqu�
        }

 

    }

}
