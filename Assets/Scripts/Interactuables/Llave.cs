using UnityEngine;

public class Llave : Objeto {
    [Header("Configuración de la llave")]
    [Tooltip("Elige si desencadena acciones")]
    [SerializeField] private bool desencadenador = false;
    [Tooltip("Arrastra interactuables objetos para habilitarlos")]
    [SerializeField] private Interactuable[] acciones;

    protected override void OnTriggerEnter2D(Collider2D colisionado) {
        //Llamamos al método del que hereda
        base.OnTriggerEnter2D (colisionado);

        //Detecta colisiones con el jugador
        if(colisionado.CompareTag("Jugador")) {
            //Activamos los objetos que son activables
            if(acciones.Length > 0 && desencadenador) {
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
    }
}