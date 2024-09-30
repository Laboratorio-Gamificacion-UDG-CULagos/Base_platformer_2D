using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class PalancaSimple : Interactuable {
    [Header("Configuración de la palanca simple")]
    [Space(5)]
    [Tooltip("Tiempo de espera (en segundos) para reutilizar la palanca")]
    [SerializeField, Min(0)] private float tiempoEspera = 0.5f;
    [Tooltip("Arrastra objetos interactuables para habilitarlos")]
    [SerializeField] private Interactuable[] Acciones;
    [Space(5)]
    [Tooltip("Establecer dirección de estado")]
    [SerializeField] private bool apuntaDerecha = false;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Arrastra un sprite a mostrar default")]
    [SerializeField] private Sprite spriteIzq;
    [Tooltip("Arrastra un sprite a mostrar contraria")]
    [SerializeField] private Sprite spriteDer;
    private bool enEspera = false;

    private void OnTriggerEnter2D(Collider2D colisionado) {
        //Detectamos la colisión con el jugador
        if (colisionado.CompareTag("Jugador") && !enEspera && activo) {
            //Establecemos en espera del botón
            StartCoroutine(TiempoDeEspera(tiempoEspera));

            //Obtenemos información del jugador
            if (colisionado.TryGetComponent<Rigidbody2D>(out Rigidbody2D rbJugador)) { 
                //Comparamos direcciones
                if(!apuntaDerecha && rbJugador.velocity.x > 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteDer;
                    EjecutarAcciones();
                } else if (apuntaDerecha && rbJugador.velocity.x < 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteIzq;
                    EjecutarAcciones();
                }
            }
        }
    }

    private void EjecutarAcciones() {
        apuntaDerecha = !apuntaDerecha;

        //Activamos los objetos que son activables
        if(Acciones.Length > 0) { 
            //Se itera en cada objeto asignado, siendo que hay mínimo uno
            for(int i = 0; i < Acciones.Length; i++) {
                //Buscamos si son objetos activables
                if (Acciones[i]) {
                    //Invertimos su estado
                    Acciones[i].activo = !Acciones[i].activo;
                }
            }
        }
    }

    public IEnumerator TiempoDeEspera(float time) {
        //Activar el tiempo de espera del botón
        enEspera = true;

        //Esperar el tiempo definido
        yield return new WaitForSeconds(time);
        
        //Desactivar el tiempo de espera del botón
        enEspera = false;
    }
}