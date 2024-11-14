using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaseScreen : MonoBehaviour
{
    [Header("Common Elements")]
    [SerializeField] protected Image backgroundImage;
    [SerializeField] protected TextMeshProUGUI titleText;
    [SerializeField] protected Button continueButton;

    protected virtual void Start()
    {
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
    }

    protected virtual void OnContinueClicked()
    {
        // Implementar en clases derivadas
    }

    public void SetTitle(string title)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }
    }

    public void SetBackground(Sprite background)
    {
        if (backgroundImage != null)
        {
            backgroundImage.sprite = background;
        }
    }
}