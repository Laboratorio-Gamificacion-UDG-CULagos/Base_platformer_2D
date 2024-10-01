using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Boton : Interactuable {
    [Header("Configuración del botón")]
    [Space(5)]
    [Tooltip("Tiempo de espera (en segundos) para reutilizar el botón")]
    [SerializeField, Min(0)] private float tiempoEspera = 0.5f;
    [Tooltip("Arrastra objetos interactuables para habilitarlos")]
    [SerializeField] private Interactuable[] acciones;
    [Tooltip("Elige el estado de habilitación mientras hay pulsaciones")]
    [SerializeField] private bool valor = true;
    [Space(5)]
    [Tooltip("Permite pulsaciones sostenidas o alternantes")]
    [SerializeField] private bool mantener = false;
    [Tooltip("Establece una duración (en segundos) para pulsaciones alternantes")]
    [SerializeField, Min(0)] private float duracion = 1.0f;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Arrastra un sprite a mostrar default")]
    [SerializeField] private Sprite spriteOff;
    [Tooltip("Arrastra un sprite a mostrar al activarse")]
    [SerializeField] private Sprite spriteOn;
    [Tooltip("Marca como presionado mientras está en contacto")]
    [SerializeField] private bool presionado;
    private bool enEspera = false;

    protected override void Update() {
        //Llamamos a la clase heredada
        base.Update();

        //Ejecutamos repetidamente la acción
        if (presionado) MantenerAcciones(valor);
    }
    
    private void InvertirAcciones() {
        //Activamos los objetos que son activables
        if (acciones.Length > 0) {
            //Se itera en cada objeto asignado, siendo que hay mínimo uno
            for (int i = 0; i < acciones.Length; i++) {
                //Buscamos si son objetos activables
                if (acciones[i]) {
                    //Invertimos su estado
                    acciones[i].activo = !acciones[i].activo;
                }
            }
        }
    }

    private void MantenerAcciones(bool estado) {
        //Activamos los objetos que son activables
        if (acciones.Length > 0) {
            //Se itera en cada objeto asignado, siendo que hay mínimo uno
            for (int i = 0; i < acciones.Length; i++) {
                //Buscamos si son objetos activables
                if (acciones[i]) {
                    //Invertimos su estado
                    acciones[i].activo = estado;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D colisionado) {
        //Detectamos la colisión con el jugador
        if (colisionado.CompareTag("Jugador") && !enEspera && activo) {
            //Si permite mantener pulsaciones 
            if (mantener) {
                //Animamos presion sobre el boton
                enEspera = true;
                GetComponent<SpriteRenderer>().sprite = spriteOn;

                //Establecemos presionado el boton
                presionado = true;
            } else {
                //Activar momentáneamente el boton
                StartCoroutine(TiempoActivo(duracion));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D colisionado) {
        //Detectamos la colisión con el jugador
        if (colisionado.CompareTag("Jugador") && activo) {
            if (mantener) {
                //Establecemos en espera del botón
                StartCoroutine(TiempoDeEspera(0.5f - tiempoEspera));
            }
        }
    }

    public IEnumerator TiempoDeEspera(float time) {
        //Activar el tiempo de espera del botón
        enEspera = true;
        GetComponent<SpriteRenderer>().sprite = spriteOn;

        //Esperar el tiempo definido
        yield return new WaitForSeconds(time);
        
        //Desactivar el tiempo de espera del botón
        enEspera = false;
        GetComponent<SpriteRenderer>().sprite = spriteOff;

        //Cancelamos la activación
        if (presionado) presionado = false;

        //Reiniciamos el valor de las acciones
        InvertirAcciones();
    }

    public IEnumerator TiempoActivo(float time) {
        //Activar el tiempo de espera del botón
        presionado = true;
        GetComponent<SpriteRenderer>().sprite = spriteOn;

        //Esperar el tiempo definido
        yield return new WaitForSeconds(time);
        
        //Desactivar el tiempo de espera del botón
        enEspera = false;
        GetComponent<SpriteRenderer>().sprite = spriteOff;

        //Establecemos en espera del botón
        StartCoroutine(TiempoDeEspera(0.5f - tiempoEspera));
    }
}