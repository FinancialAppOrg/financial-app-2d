using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerAuth : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] GameObject signUpScreen;
    [SerializeField] GameObject signInScreen;
    [SerializeField] GameObject forgotPasswordScreen;
    [SerializeField] GameObject enterCodeScreen;
    [SerializeField] GameObject resetPasswordScreen;

    [Header("Game Controller")]
    [SerializeField] gameController gameController;

    void Start()
    {
        if (signInScreen != null) signInScreen.gameObject.SetActive(true);
        if (signUpScreen != null) signUpScreen.gameObject.SetActive(false);
        if (forgotPasswordScreen != null) forgotPasswordScreen.gameObject.SetActive(false);
        if (enterCodeScreen != null) enterCodeScreen.gameObject.SetActive(false);
        if (resetPasswordScreen != null) resetPasswordScreen.gameObject.SetActive(false);
    }

    void Awake()
    {
        gameController = FindObjectOfType<gameController>();
    }

    public void OnSignUpComplete()
    {
        if (signInScreen != null) signInScreen.gameObject.SetActive(true);
        if (signUpScreen != null) signUpScreen.gameObject.SetActive(false);
    }

    public void OnSignInComplete()
    {
        Debug.Log("SignIn completo. Transición a la siguiente pantalla.");
        if (gameController.Instancia != null)
        {
            gameController.Instancia.CargarEvaluacion();
        }
        else
        {
            Debug.LogWarning("No se encontró la instancia de GameController.");
        }
    }

    public void ShowSignUpScreen()
    {
        if (signInScreen != null) signInScreen.gameObject.SetActive(false);
        if (signUpScreen != null) signUpScreen.gameObject.SetActive(true);
    }

    public void ShowForgotPasswordScreen()
    {
        if (signInScreen != null) signInScreen.gameObject.SetActive(false);
        if (forgotPasswordScreen != null) forgotPasswordScreen.gameObject.SetActive(true);
    }

    public void ShowEnterCodeScreen()
    {
        if (forgotPasswordScreen != null) forgotPasswordScreen.gameObject.SetActive(false);
        if (enterCodeScreen != null) enterCodeScreen.gameObject.SetActive(true);
    }

    public void ShowResetPasswordScreen()
    {
        if (enterCodeScreen != null) enterCodeScreen.gameObject.SetActive(false);
        if (resetPasswordScreen != null) resetPasswordScreen.gameObject.SetActive(true);
    }

    public void BackToEnterCodeScreen()
    {
        if (resetPasswordScreen != null) resetPasswordScreen.gameObject.SetActive(false);
        if (enterCodeScreen != null) enterCodeScreen.gameObject.SetActive(true);
    }

    public void BackToForgotPasswordScreen()
    {
        if (enterCodeScreen != null) enterCodeScreen.gameObject.SetActive(false);
        if (forgotPasswordScreen != null) forgotPasswordScreen.gameObject.SetActive(true);
    }

    public void BackToSignInScreen()
    {
        if (forgotPasswordScreen != null) forgotPasswordScreen.gameObject.SetActive(false);
        if (signUpScreen != null) signUpScreen.gameObject.SetActive(false);
        if (enterCodeScreen != null) enterCodeScreen.gameObject.SetActive(false);
        if (resetPasswordScreen != null) resetPasswordScreen.gameObject.SetActive(false);
        if (signInScreen != null) signInScreen.gameObject.SetActive(true);
    }
}
