using UnityEngine;
using System.Collections;

public class PalancaAvanzada : Interactuable {
    [Header("Configuración de la palanca simple")]
    [Space(5)]
    [Tooltip("Tiempo de espera (en segundos) para reutilizar la palanca")]
    [SerializeField, Min(0)] private float tiempoEspera = 0.5f;
    [Tooltip("Arrastra objetos interactuables para habilitarlos repetidamente")]
    [SerializeField] private Interactuable[] AccionesIzq;
    [Tooltip("Arrastra objetos interactuables para habilitarlos repetidamente")]
    [SerializeField] private Interactuable[] AccionesDer;
    [Space(5)]
    [Tooltip("Establecer dirección de estado")]
    [SerializeField, Range(0, 2)] private int estado = 1;
    [Tooltip("Valor del estado")]
    [SerializeField] private bool valor = true;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Arrastra un sprite a mostrar (izquierda)")]
    [SerializeField] private Sprite spriteIzq;
    [Tooltip("Arrastra un sprite a mostrar (default)")]
    [SerializeField] private Sprite spriteDef;
    [Tooltip("Arrastra un sprite a mostrar (contraria)")]
    [SerializeField] private Sprite spriteDer;
    private Rigidbody2D rbJugador;
    private bool enEspera = false;

    protected override void Update() {
        //Llamamos al update heredado
        base.Update();

        //Llamamos repetidamente cada estado
        if (estado == 0) {
            EjecutarAcciones(AccionesIzq, valor);
        } else if (estado == 2) {
            EjecutarAcciones(AccionesDer, valor);
        }
    }

    private void EjecutarAcciones(Interactuable[] acciones, bool estado) {
        //Activamos los objetos que son activables
        if(acciones.Length > 0) { 
            //Se itera en cada objeto asignado, siendo que hay mínimo uno
            for(int i = 0; i < acciones.Length; i++) {
                //Buscamos si son objetos activables
                if (acciones[i] && acciones[i].activo != estado) {
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
            if(estado == 1) {
                if (rbJugador.velocity.x > 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteDer;
                    estado++;
                } else if (rbJugador.velocity.x < 0.0f) {
                    GetComponent<SpriteRenderer>().sprite = spriteIzq;
                    estado--;
                }
            } else if (estado == 0 && rbJugador.velocity.x > 0.0f) {
                GetComponent<SpriteRenderer>().sprite = spriteDef;
                estado++;
                EjecutarAcciones(AccionesIzq, !valor);
            } else if (estado == 2 && rbJugador.velocity.x < 0.0f) {
                GetComponent<SpriteRenderer>().sprite = spriteDef;
                estado--;
                EjecutarAcciones(AccionesDer, !valor);
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