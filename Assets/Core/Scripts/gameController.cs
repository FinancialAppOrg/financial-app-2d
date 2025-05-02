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
        SceneManager.LoadScene(inversionSceneIndex);
    }

    public void CargarEvaluacion()
    {
        SceneManager.LoadScene(evaluacionSceneIndex);
    }
}
