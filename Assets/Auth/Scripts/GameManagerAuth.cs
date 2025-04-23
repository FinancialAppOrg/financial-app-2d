using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerAuth : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] GameObject signUpScreen;
    [SerializeField] GameObject signInScreen;

    void Start()
    {
        if (signUpScreen != null) signUpScreen.gameObject.SetActive(true);
        if (signInScreen != null) signInScreen.gameObject.SetActive(false);
    }

    public void OnSignUpComplete()
    {
        if (signUpScreen != null) signUpScreen.gameObject.SetActive(false);
        if (signInScreen != null) signInScreen.gameObject.SetActive(true);
    }

    public void OnSignInComplete()
    {
        Debug.Log("SignIn completo. Transición a la siguiente pantalla.");
    }
}
