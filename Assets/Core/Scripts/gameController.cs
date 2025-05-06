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
    [SerializeField] private int authSceneIndex;

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
        CargarAuthScene();
    }
    public void CargarJuegoInversion()
    {
        SceneManager.LoadScene(inversionSceneIndex);
    }

    public void CargarEvaluacion()
    {
        SceneManager.LoadScene(evaluacionSceneIndex);
    }

    public void CargarAuthScene()
    {
        SceneManager.LoadScene(authSceneIndex);
    }
}
