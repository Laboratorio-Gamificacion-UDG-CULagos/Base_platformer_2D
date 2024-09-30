using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Objeto : Interactuable {
    [Header("Configuración del objeto")]
    [Tooltip("Elige un valor cuantitativo")]
    [SerializeField, Min(0)] private int valor = 1;
    
    protected virtual void OnTriggerEnter2D(Collider2D colisionado) {
        //Detecta colisiones con el jugador
        if(colisionado.CompareTag("Jugador")) {
            //PENDIENTE AÑADIR DESENCADENAMIENTO DE ACCIONES UI
        }

        //Destruye la llave milisegundos despues
        Destroy(gameObject, 0.05f);
    }
}