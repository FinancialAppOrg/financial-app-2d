using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Configuración")]
    public float escalaPresionado = 0.9f;
    public float velocidadAnimacion = 10f;

    private Vector3 escalaObjetivo;
    private Vector3 escalaOriginal;
    private Button boton;

    private void Awake()
    {
        escalaOriginal = transform.localScale;
        escalaObjetivo = escalaOriginal;
        boton = GetComponent<Button>();
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            escalaObjetivo,
            Time.deltaTime * velocidadAnimacion
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (boton.interactable)
            escalaObjetivo = escalaOriginal * escalaPresionado;
    }

    public void OnPointerUp(PointerEventData eventData) => VolverANormal();
    public void OnPointerExit(PointerEventData eventData) => VolverANormal();

    private void VolverANormal()
    {
        escalaObjetivo = escalaOriginal;
    }
}