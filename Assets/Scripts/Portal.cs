using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Portal Configuration")]
    [Tooltip("Define el portal de salida")]
    public Transform portalSalida;

    [Tooltip("Define si el portal está activo o no")]
    public bool activo = true;

    [Tooltip("Define si se conserva la inercia del jugador tras teletransportarse")]
    public bool conservarInercia = false;

    [Tooltip("Tiempo de espera (en segundos) para reutilizar el portal")]
    public float tiempoEspera = 2f;
    private bool enEspera = false;

    private Rigidbody2D rbJugador;

    private void OnTriggerEnter2D(Collider2D collision) {
        //Salir si el portal no está activo
        if (!activo || enEspera) return;

        //Detectar si es un jugador lo que toca
        if (collision.CompareTag("Jugador")) {
            rbJugador = collision.GetComponent<Rigidbody2D>();
        }
    }
    
    void FixedUpdate() {
        //Si hay un jugador, teletransportarlo
        if (rbJugador) {
            TeletransportarJugador();
            rbJugador = null;
        }
    }

    private void TeletransportarJugador() {
        //Mover al jugador al nuevo portal de salida
        rbJugador.position = portalSalida.position;

        //Reiniciar la velocidad si no quieres conservar la inercia
        if (!conservarInercia) {
            rbJugador.velocity = Vector2.zero;
        }

        //Poner ambos portales en espera
        StartCoroutine(TiempoDeEspera(tiempoEspera));
        StartCoroutine(portalSalida.GetComponent<Portal>().TiempoDeEspera(tiempoEspera));
    }

    public IEnumerator TiempoDeEspera(float time) {
        //Activar el tiempo de espera del portal
        enEspera = true;

        //Esperar el tiempo definido
        yield return new WaitForSeconds(time);

        //Desactivar el tiempo de espera del portal
        enEspera = false;
    }

    private void OnDrawGizmos() {
        //Visualizar en el Editor las conexiones entre portales
        if (portalSalida != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, portalSalida.position);
        }
    }
}