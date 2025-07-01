using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBack : MonoBehaviour
{
    [SerializeField] private GameObject previousScreen;
    [SerializeField] private GameObject currentScreen;  

    private Button backButton;

    void Awake()
    {
        backButton = GetComponent<Button>();
        if (backButton == null)
        {
            Debug.LogError("El componente Button no está asignado al BackButton.");
            return;
        }

        backButton.onClick.AddListener(OnBackButtonPressed);
    }

    private void OnBackButtonPressed()
    {
        if (currentScreen != null) currentScreen.SetActive(false);
        if (previousScreen != null) previousScreen.SetActive(true);
    }
}
