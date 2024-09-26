using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour {
    [Header("Configuración del Portal")]
    [Tooltip("Define si el portal está activo o no")]
    [SerializeField] private bool activo = true;
    [Tooltip("Arrastra un portal de salida")]
    [SerializeField] private Portal portalSalida;
    [Tooltip("Añade un multiplicador de lanzamiento (redirección)")]
    [SerializeField] private float multiplicadorInercia = 1.0f;
    [Tooltip("Añade un ángulo de lanzamiento (redirección)")]
    [Range(0, 359)]
    [SerializeField] private int angulo = 0;
    [Tooltip("Define si se conserva la inercia tras teletransportarse")]
    [SerializeField] private bool conservarInercia = true;
    [Tooltip("Elige si debe redirigir tras teletransportar (requiere inercia)")]
    [SerializeField] private bool redirigir = false;
    [Tooltip("Tiempo de espera (en segundos) para reutilizar el portal")]
    [SerializeField] private float tiempoEspera = 1.0f;

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
        //Bloqueamos si no hay conexión
        if (!portalSalida) activo = false;

        //Si hay un jugador, teletransportarlo
        if (rbJugador) {
            TeletransportarJugador();
            rbJugador = null;
        }
    }

    private void TeletransportarJugador() {
        //Mover al jugador al nuevo portal de salida
        rbJugador.position = portalSalida.transform.position;

        //Reiniciar la velocidad si no quieres conservar la inercia
        if (portalSalida.conservarInercia) {
            if (portalSalida.redirigir) {
                //Redirigimos al jugador
                rbJugador.velocity = AnguloADireccion(portalSalida.angulo) * rbJugador.velocity.magnitude * multiplicadorInercia;
            }
        } else {
            //Reseteamos la velocidad del jugador
            rbJugador.velocity = Vector2.zero;
        }

        //Poner ambos portales en espera
        StartCoroutine(TiempoDeEspera(tiempoEspera));
        StartCoroutine(portalSalida.GetComponent<Portal>().TiempoDeEspera(tiempoEspera));
    }

    private Vector2 AnguloADireccion(float angulo) {
        //Convertimos el angulo a coordenadas
        float radianes = (angulo + 90) * Mathf.Deg2Rad;

        //Obtenemos las coordenadas
        float x = Mathf.Cos(radianes);
        float y = Mathf.Sin(radianes);
        return new Vector2(x, y).normalized;
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
            Gizmos.DrawLine(transform.position, portalSalida.transform.position);
        }

        //Visualizamos la dirección de salida del portal
        if (conservarInercia && redirigir) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + (AnguloADireccion(angulo) * multiplicadorInercia));
        }
    }
}