using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Heridor : Interactuable {
    [Header("Configuraci�n del heridor")]
    [Tooltip("Asigna un tiempo de espera del cactus")]
    [SerializeField, Min(0)] protected float tiempoEspera = 0.5f;
    [Space(5)]
    [Tooltip("Elige el valor de da�o al contacto")]
    [SerializeField, Min(0)] protected int golpe = 1;
    [Tooltip("Permite lanzar al jugador de regreso")]
    [SerializeField] protected bool repeler;
    [Tooltip("Agrega un multiplicador de repulsi�n")]
    [SerializeField, Min(0)] protected float factorRepulsion = 1.0f;
    [Tooltip("Anula el movimiento del jugador")]
    [SerializeField] protected bool detener;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [SerializeField] protected Transform sprite;
    [SerializeField] protected bool enEspera = false;
    [SerializeField] protected Vector2 direccion;

    private void OnTriggerEnter2D(Collider2D colisionado) {
        //Detectamos la colisi�n con el jugador
        if (colisionado.CompareTag("Jugador") && !enEspera && activo) {
            //Actualizamos su vida
            colisionado.GetComponent<Personaje>().vida -= golpe;

            //Checamos si habilita repeler
            if (repeler) {
                Rigidbody2D rbJugador = colisionado.GetComponent<Rigidbody2D>();
                Vector2 nuevaVelocidad = new Vector2(-rbJugador.velocity.x, -rbJugador.velocity.y / 5) * factorRepulsion;
                rbJugador.velocity = nuevaVelocidad;
                rbJugador.AddForce(Vector2.up * 500 * factorRepulsion, ForceMode2D.Impulse);
            } else if (detener) {
                colisionado.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }

            //Establecemos en espera de las p�as
            StartCoroutine(TiempoDeEspera(tiempoEspera));
        }
    }

    protected virtual IEnumerator TiempoDeEspera(float espera) {
        //Activar el tiempo de espera del resorte
        enEspera = true;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Desactivar el tiempo de espera del resorte
        enEspera = false;
    }
}