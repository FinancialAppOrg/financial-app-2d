using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelScreen : MonoBehaviour
{
    [SerializeField] Button[] levelButtons;
    [SerializeField] GameControllerBridge gameControllerBridge; //Transicion de escenas

    void Start()
    {
        foreach (Button button in levelButtons)
        {
            button.onClick.AddListener(() => OnLevelSelected(button.name));
        }

    }

    void OnLevelSelected(string level)
    {
        Debug.Log("Level selected: " + level);
        // PlayerData.SetSelectedTopic(temaSeleccionado);
        PlayerData.SetSelectedLevel(level);
        CargarJuegoInversion();//FindObjectOfType<GameManager>().CargarJuegoInversion(level);
    }
    public void CargarJuegoInversion()
    {
        if (gameControllerBridge != null)
        {
            gameControllerBridge.CargarJuegoInversion();
        }
        else
        {
            Debug.LogWarning("No se encontró el GameControllerBridge.");
        }
    }

}
