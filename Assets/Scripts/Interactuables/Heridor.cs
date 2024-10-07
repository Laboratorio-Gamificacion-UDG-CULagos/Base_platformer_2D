using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Heridor : Interactuable {
    [Header("Configuración del heridor")]
    [Tooltip("Asignar capas de heridos por colisión")]
    [SerializeField] private LayerMask heribles;
    [Tooltip("Asigna un tiempo de espera del heridor")]
    [SerializeField, Min(0)] protected float tiempoEspera = 0.5f;
    [Tooltip("Elige el valor de daño al contacto")]
    [SerializeField, Min(0)] protected int golpe = 1;
    [Space(5)]
    [Tooltip("Permite lanzar al jugador de regreso")]
    [SerializeField] protected bool repeler;
    [Tooltip("Agrega un multiplicador de repulsión")]
    [SerializeField, Min(0)] protected float factorRepulsion = 1.0f;
    [Space(5)]
    [Tooltip("Anula el movimiento del jugador")]
    [SerializeField] protected bool detener;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Marca el estado actual del interactuable")]
    [SerializeField] protected bool enEspera = false;

    protected virtual void OnTriggerEnter2D(Collider2D colisionado) {
        //Detectamos la colisión con el jugador
        if (((heribles & (1 << colisionado.gameObject.layer)) > 0) && !enEspera && activo) {
            //Actualizamos su vida
            if (colisionado.TryGetComponent<Personaje>(out Personaje p)) {
                p.Herir(golpe);
            }

            //Checamos si habilita repeler
            if (repeler) {
                Rigidbody2D rbJugador = colisionado.GetComponent<Rigidbody2D>();
                Vector2 nuevaVelocidad = new Vector2(-rbJugador.velocity.x, -rbJugador.velocity.y / 5) * factorRepulsion;
                rbJugador.velocity = nuevaVelocidad;
                rbJugador.AddForce(Vector2.up * 5 * factorRepulsion, ForceMode2D.Impulse);
            } else if (detener) {
                colisionado.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }

            //Establecemos en espera del heridor
            StartCoroutine(TiempoDeEspera(tiempoEspera));
        }
    }

    protected virtual IEnumerator TiempoDeEspera(float espera) {
        //Activar el tiempo de espera
        enEspera = true;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Desactivar el tiempo de espera
        enEspera = false;
    }
}