using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Boton : Interactuable {
    [Header("Configuración del botón")]
    [Tooltip("Arrastra un sprite a mostrar default")]
    [SerializeField] private Sprite spriteOff;
    [Tooltip("Arrastra un sprite a mostrar al activarse")]
    [SerializeField] private Sprite spriteOn;
    [Space(5)]
    [Tooltip("Tiempo de espera (en segundos) para reutilizar el botón")]
    [SerializeField, Min(0)] private float tiempoEspera = 0.5f;
    [Tooltip("Arrastra interactuables objetos para habilitarlos")]
    [SerializeField] private Interactuable[] Acciones;

    private bool enEspera = false;
    private Rigidbody2D rbJugador;

    private void OnTriggerEnter2D(Collider2D collision) {
        //Detectamos la colisión con el jugador
        if (collision.CompareTag("Jugador") && !enEspera && activo) {
            //Establecemos en espera del botón
            StartCoroutine(TiempoDeEspera(tiempoEspera));
        }

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
        GetComponent<SpriteRenderer>().sprite = spriteOn;

        //Esperar el tiempo definido
        yield return new WaitForSeconds(time);
        
        //Desactivar el tiempo de espera del botón
        enEspera = false;
        GetComponent<SpriteRenderer>().sprite = spriteOff;
    }
}