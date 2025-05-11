using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameController : MonoBehaviour
{
    // Singleton
    public static gameController Instancia { get; private set; }
    //escenas
    [SerializeField] private int evaluacionSceneIndex;
    [SerializeField] private int inversionSceneIndex;


    void Awake()
    {
        // Configuración del singleton
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        CargarEvaluacion(); 
    }
    public void CargarJuegoInversion()
    {
        string tema = PlayerData.GetInterest();
        float nivel = PlayerData.GetKnowledge(tema);

        if (!string.IsNullOrEmpty(tema) && nivel > 0)
        {
            SceneManager.LoadScene(inversionSceneIndex);
        }
        else
        {
            Debug.LogWarning("Debe seleccionarse un tema y nivel antes de iniciar el juego.");
        }
        //SceneManager.LoadScene(inversionSceneIndex);
    }

    public void CargarEvaluacion()
    {
        SceneManager.LoadScene(evaluacionSceneIndex);
    }
}
