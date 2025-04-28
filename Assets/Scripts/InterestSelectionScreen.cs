using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterestSelectionScreen : MonoBehaviour
{
    [SerializeField] Button[] interestButtons;

    GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        foreach (Button button in interestButtons)
        {
            button.onClick.AddListener(() => OnInterestSelected(button.name));
        }
    }

    void OnInterestSelected(string interest)
    {
        Debug.Log("Interest selected: " + interest);
        PlayerData.SetInterest(interest);


        if (gameManager != null)
        {
            gameManager.ShowOptionsScreen();
        }
        else
        {
            Debug.LogError("GameManager no encontrado.");
        }
    }
}
