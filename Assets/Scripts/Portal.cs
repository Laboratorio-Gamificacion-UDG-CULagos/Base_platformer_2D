using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour {
    [Header("Configuraci�n del Portal")]
    [Tooltip("Define si el portal est� activo o no")]
    public bool activo = true;

    [Tooltip("Arrastra un portal de salida")]
    public Portal portalSalida;
    [Tooltip("A�ade un multiplicador de lanzamiento (redirecci�n)")]
    public float multiplicadorInercia = 1.0f;
    [Tooltip("A�ade un �ngulo de lanzamiento (redirecci�n)")]
    [Range(0, 359)]
    public int angulo = 0;

    [Tooltip("Define si se conserva la inercia tras teletransportarse")]
    public bool conservarInercia = true;
    [Tooltip("Elige si debe redirigir tras teletransportar")]
    public bool redirigir = false;

    [Tooltip("Tiempo de espera (en segundos) para reutilizar el portal")]
    public float tiempoEspera = 1.0f;
    private bool enEspera = false;

    private Rigidbody2D rbJugador;

    private void OnTriggerEnter2D(Collider2D collision) {
        //Salir si el portal no est� activo
        if (!activo || enEspera) return;

        //Detectar si es un jugador lo que toca
        if (collision.CompareTag("Jugador")) {
            rbJugador = collision.GetComponent<Rigidbody2D>();
        }
    }
    
    void FixedUpdate() {
        //Bloqueamos si no hay conexi�n
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

        //Visualizamos la direcci�n de salida del portal
        if (conservarInercia && redirigir) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + (AnguloADireccion(angulo) * multiplicadorInercia));
        }
    }
}