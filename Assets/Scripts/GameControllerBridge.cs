using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerBridge : MonoBehaviour
{
    public gameController gameController;
    public void CargarJuegoInversion()
    {
        if (gameController.Instancia != null)
            gameController.Instancia.CargarJuegoInversion();
        else
            Debug.LogWarning("No se encontró la instancia de GameController.");
    }

    public void CargarEvaluacion()
    {
        if (gameController.Instancia != null)
            gameController.Instancia.CargarEvaluacion();
        else
            Debug.LogWarning("No se encontró la instancia de GameController.");
    }
}
