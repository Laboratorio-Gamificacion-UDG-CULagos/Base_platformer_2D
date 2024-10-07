using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class PalancaAlternante : Interactuable {
    [Header("Configuraci�n de la palanca alternante")]
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
    [Tooltip("Establecer direcci�n de estado (0 = izquierda)")]
    [SerializeField, Range(0, 2)] private int estado = 0;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Arrastra un sprite a mostrar (izquierda)")]
    [SerializeField] private Sprite spriteIzq;
    [Tooltip("Arrastra un sprite a mostrar (centro)")]
    [SerializeField] private Sprite spriteCen;
    [Tooltip("Arrastra un sprite a mostrar (derecha)")]
    [SerializeField] private Sprite spriteDer;
    [Tooltip("Referenc�a al RigidBody2D del jugador")]
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
    }

    private void EjecucionIndividual(Interactuable[] acciones) {
        //Activamos los objetos que son interactuables
        if (acciones.Length > 0) { 
            //Se itera en cada objeto asignado, siendo que hay m�nimo uno
            for(int i = 0; i < acciones.Length; i++) {
                //Buscamos si son objetos interactuables
                if (acciones[i]) {
                    //Invertimos el estado
                    acciones[i].activo = !acciones[i].activo;
                }
            }
        }
    }

    private void EjecucionTotal() {
        //Activamos los objetos que son interactuables de derecha
        EjecucionIndividual(accionesDer);
        //Activamos los objetos que son interactuables de izquierda
        EjecucionIndividual(accionesIzq);
    }

    private void OnTriggerEnter2D(Collider2D colisionado) {
        //Detectamos la colisi�n con el jugador
        if (colisionado.CompareTag("Jugador") && !enEspera && activo) {
            //Guardamos referencia al jugador
            rbJugador = colisionado.GetComponent<Rigidbody2D>();

            //Comparamos direcciones
            if (!simple) {
                //Sistema avanzado de asignaci�n
                if(estado == 1) {
                    if (rbJugador.velocity.x > 0.0f) {
                        GetComponent<SpriteRenderer>().sprite = spriteDer;
                        estado++;
                        EjecucionIndividual(accionesDer);
                    } else if (rbJugador.velocity.x < 0.0f) {
                        GetComponent<SpriteRenderer>().sprite = spriteIzq;
                        estado--;
                        EjecucionIndividual(accionesIzq);
                    }
                //Sistemas avanzados de reset
                } else if (estado == 0 && rbJugador.velocity.x > 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteCen;
                    estado++;
                    EjecucionIndividual(accionesIzq);
                } else if (estado == 2 && rbJugador.velocity.x < 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteCen;
                    estado--;
                    EjecucionIndividual(accionesDer);
                }
            } else {
                //Sistema de intermitencia simple
                if (estado == 0 && rbJugador.velocity.x > 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteDer;
                    estado = 2;
                    EjecucionTotal();
                } else if (estado == 2 && rbJugador.velocity.x < 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteIzq;
                    estado = 0;
                    EjecucionTotal();
                }
            }

            //Establecemos en espera del bot�n
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