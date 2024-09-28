using UnityEngine;

public class Llave : Interactuable {
    [Header("Configuración de la llave")]
    [Tooltip("Elige si desencadena acciones")]
    [SerializeField] private bool desencadenador = false;
    [Tooltip("Arrastra interactuables objetos para habilitarlos")]
    [SerializeField] private Interactuable[] Acciones;

    private void OnTriggerEnter2D(Collider2D colisionado) {
        //Detecta colisiones con el jugador
        if(colisionado.CompareTag("Jugador")) {
            //Activamos los objetos que son activables
            if(Acciones.Length > 0) { 
                //Se itera en cada objeto asignado, siendo que hay minimo uno
                for(int i = 0; i < Acciones.Length; i++) {
                    //Buscamos si son objetos activables
                    if (Acciones[i]) {
                        //Invertimos su estado
                        Acciones[i].activo = !Acciones[i].activo;
                    }
                }
            }
        }

        //Destruye la llave milisegundos despues
        Destroy(gameObject, 0.05f);
    }
}