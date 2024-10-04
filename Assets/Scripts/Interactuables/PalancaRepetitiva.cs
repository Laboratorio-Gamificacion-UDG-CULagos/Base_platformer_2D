using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class PalancaRepetitiva : Interactuable {
    [Header("Configuración de la palanca repetitiva")]
    [Space(5)]
    [Tooltip("Tiempo de espera (en segundos) para reutilizar la palanca")]
    [SerializeField, Min(0)] private float tiempoEspera = 0.5f;
    [Tooltip("Arrastra objetos interactuables para habilitar repetidamente (derecha)")]
    [SerializeField] private Interactuable[] accionesIzq;
    [Tooltip("Arrastra objetos interactuables para habilitar repetidamente (derecha)")]
    [SerializeField] private Interactuable[] accionesDer;
    [Tooltip("Valor del estado (avanzada)")]
    [SerializeField] private bool valor = true;
    [Space(5)]
    [Tooltip("Elige el tipo de palanca (simple|avanzada)")]
    [SerializeField] private bool simple = true;
    [Space(5)]
    [Tooltip("Establecer dirección de estado (0 = izquierda)")]
    [SerializeField, Range(0, 2)] private int estado = 0;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Arrastra un sprite a mostrar (izquierda)")]
    [SerializeField] private Sprite spriteIzq;
    [Tooltip("Arrastra un sprite a mostrar (centro)")]
    [SerializeField] private Sprite spriteCen;
    [Tooltip("Arrastra un sprite a mostrar (derecha)")]
    [SerializeField] private Sprite spriteDer;
    [Tooltip("Referencía al RigidBody2D del jugador")]
    [SerializeField] private Rigidbody2D rbJugador;
    [Tooltip("Marca el estado actual del interactuable")]
    [SerializeField] private bool enEspera = false;

    private void Start() {
        //Inicializamos su estado
        switch (estado) { 
            case 0:
                GetComponent<SpriteRenderer>().sprite = spriteIzq;
                break;
            case 1:
                GetComponent<SpriteRenderer>().sprite = spriteCen;
                break;
            case 2:
                GetComponent<SpriteRenderer>().sprite = spriteDer;
                break;
        }
    }

    protected override void Update() {
        //Llamamos al update heredado
        base.Update();

        //Llamamos repetidamente cada estado
        if (estado == 0) {
            EjecucionRepetida(accionesIzq, valor);
        } else if (estado == 2) {
            EjecucionRepetida(accionesDer, valor);
        }
    }

    private void EjecucionRepetida(Interactuable[] acciones, bool estado) {
        //Activamos los objetos que son interactuables
        if (acciones.Length > 0) { 
            //Se itera en cada objeto asignado, siendo que hay mínimo uno
            for(int i = 0; i < acciones.Length; i++) {
                //Buscamos si son objetos interactuables
                if (acciones[i]) {
                    //Marcamos su estado
                    acciones[i].activo = estado;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D colisionado) {
        //Detectamos la colisión con el jugador
        if (colisionado.CompareTag("Jugador") && !enEspera && activo) {
            //Guardamos referencia al jugador
            rbJugador = colisionado.GetComponent<Rigidbody2D>();

            //Comparamos direcciones
            if (!simple) {
                //Sistema avanzado de asignación
                if(estado == 1) {
                    if (rbJugador.velocity.x > 0.0f) {
                        GetComponent<SpriteRenderer>().sprite = spriteDer;
                        estado++;
                    } else if (rbJugador.velocity.x < 0.0f) {
                        GetComponent<SpriteRenderer>().sprite = spriteIzq;
                        estado--;
                    }
                //Sistemas avanzados de reset
                } else if (estado == 0 && rbJugador.velocity.x > 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteCen;
                    estado++;
                    EjecucionRepetida(accionesIzq, !valor);
                } else if (estado == 2 && rbJugador.velocity.x < 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteCen;
                    estado--;
                    EjecucionRepetida(accionesDer, !valor);
                }
            } else {
                //Sistema de intermitencia simple
                if (estado == 0 && rbJugador.velocity.x > 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteDer;
                    estado = 2;
                    EjecucionRepetida(accionesIzq, valor);
                } else if (estado == 2 && rbJugador.velocity.x < 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteIzq;
                    estado = 0;
                    EjecucionRepetida(accionesDer, valor);
                }
            }

            //Establecemos en espera del botón
            StartCoroutine(TiempoDeEspera(tiempoEspera));
        }
    }

    private IEnumerator TiempoDeEspera(float time) {
        //Activar el tiempo de espera
        enEspera = true;

        //Esperar el tiempo definido
        yield return new WaitForSeconds(time);
        
        //Desactivar el tiempo de espera
        enEspera = false;
    }
}