using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class PalancaAlternante : Interactuable {
    [Header("Configuración de la palanca alternante")]
    [Space(5)]
    [Tooltip("Tiempo de espera (en segundos) para reutilizar la palanca")]
    [SerializeField, Min(0)] private float tiempoEspera = 0.5f;
    [Tooltip("Arrastra objetos interactuables para alternar su estado (derecha)")]
    [SerializeField] private Interactuable[] accionesIzq;
    [Tooltip("Arrastra objetos interactuables para alternar su estado (derecha)")]
    [SerializeField] private Interactuable[] accionesDer;
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

    protected override void Update() {
        //Llamamos al update heredado
        base.Update();
    }

    private void EjecucionCompleja(Interactuable[] acciones) {
        //Activamos los objetos que son activables
        if(acciones.Length > 0) { 
            //Se itera en cada objeto asignado, siendo que hay mínimo uno
            for(int i = 0; i < acciones.Length; i++) {
                //Buscamos si son objetos activables
                if (acciones[i]) {
                    //Marcamos su estado
                    acciones[i].activo = !acciones[i].activo;
                }
            }
        }
    }

    private void EjecucionSimple() {
        //Activamos los objetos que son activables
        if(accionesDer.Length > 0) { 
            //Se itera en cada objeto asignado, siendo que hay mínimo uno
            for(int i = 0; i < accionesDer.Length; i++) {
                //Buscamos si son objetos activables
                if (accionesDer[i]) {
                    //Marcamos su estado
                    accionesDer[i].activo = !accionesDer[i].activo;
                }
            }
        }
        //Activamos los objetos que son activables
        if(accionesIzq.Length > 0) { 
            //Se itera en cada objeto asignado, siendo que hay mínimo uno
            for(int i = 0; i < accionesIzq.Length; i++) {
                //Buscamos si son objetos activables
                if (accionesIzq[i]) {
                    //Marcamos su estado
                    accionesIzq[i].activo = !accionesIzq[i].activo;
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
                if(estado == 1) {
                    if (rbJugador.velocity.x > 0.0f) {
                        GetComponent<SpriteRenderer>().sprite = spriteDer;
                        estado++;
                    } else if (rbJugador.velocity.x < 0.0f) {
                        GetComponent<SpriteRenderer>().sprite = spriteIzq;
                        estado--;
                    }
                } else if (estado == 0 && rbJugador.velocity.x > 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteCen;
                    estado++;
                    EjecucionCompleja(accionesIzq);
                } else if (estado == 2 && rbJugador.velocity.x < 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteCen;
                    estado--;
                    EjecucionCompleja(accionesDer);
                }
            } else {
                if (estado == 0 && rbJugador.velocity.x > 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteDer;
                    estado = 2;
                    EjecucionSimple();
                } else if (estado == 2 && rbJugador.velocity.x < 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteIzq;
                    estado = 0;
                    EjecucionSimple();
                }
            }

            //Establecemos en espera del botón
            StartCoroutine(TiempoDeEspera(tiempoEspera));
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