using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LlamitaPulse : MonoBehaviour
{
    public float velocidad = 1f;
    public float escalaMin = 0.70f;
    public float escalaMax = 1.05f;

    private Vector3 escalaOriginal;

    void Start()
    {
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        float escala = Mathf.Lerp(escalaMin, escalaMax, (Mathf.Sin(Time.time * velocidad) + 1f) / 2f);
        transform.localScale = escalaOriginal * escala;
    }
}
