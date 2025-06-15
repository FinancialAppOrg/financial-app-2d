using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlosarioBrillo : MonoBehaviour
{
    public float duracionAgitacion = 0.3f;  
    public float intensidad = 0.03f;        
    public float intervalo = 8f;            

    private Vector3 posicionOriginal;
    private float tiempoUltimaAgitacion;
    private float tiempoAgitando;
    private bool estaAgitando;

    void Start()
    {
        posicionOriginal = transform.localPosition;
        tiempoUltimaAgitacion = Time.time;
    }

    void Update()
    {
        if (!estaAgitando && Time.time - tiempoUltimaAgitacion >= intervalo)
        {
            estaAgitando = true;
            tiempoAgitando = 0f;
        }

        if (estaAgitando)
        {
            tiempoAgitando += Time.deltaTime;

            if (tiempoAgitando <= duracionAgitacion)
            {
                float offsetX = Mathf.Sin(tiempoAgitando * 20f) * intensidad; // 20 = frecuencia del movimiento
                transform.localPosition = posicionOriginal + new Vector3(offsetX, 0, 0);
            }
            else
            {
                transform.localPosition = posicionOriginal;
                estaAgitando = false;
                tiempoUltimaAgitacion = Time.time;
            }
        }
    }
}
