using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    private Animator animator;  // Referencia al Animator del jugador
    public float speed = 20f;    // Velocidad de movimiento del jugador

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void MoveToArea(Vector3 targetPosition)
    {
        StartCoroutine(MoveToPosition(targetPosition));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        // Primero, aseguramos que el personaje empiece en idle
        animator.SetFloat("speed", 0f); // Aseguramos que empieza en idle

        // Esperar un pequeño tiempo para que el cambio de animación ocurra
        yield return new WaitForSeconds(0.1f);

        // Ahora el personaje empieza a caminar
        animator.SetFloat("speed", 1f); // Iniciar la animación de caminar

        Vector3 direction = (targetPosition - transform.position).normalized;

        // Actualizamos las direcciones en el Animator
        animator.SetFloat("directionX", direction.x);
        animator.SetFloat("directionY", direction.z);

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Mover el personaje hacia la posición objetivo
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Recalcular dirección y actualizar Animator
            direction = (targetPosition - transform.position).normalized;
            animator.SetFloat("directionX", direction.x);
            animator.SetFloat("directionY", direction.z);

            Debug.Log($"Speed: {animator.GetFloat("speed")}, directionX: {animator.GetFloat("directionX")}, directionY: {animator.GetFloat("directionY")}");
            
            yield return null;  // Esperar al siguiente frame
        }

        // Cuando llegue al destino, se detiene la animación de caminar y se vuelve a idle
        animator.SetFloat("speed", 0f); // Detener la animación de caminar y volver a idle
    }
}

