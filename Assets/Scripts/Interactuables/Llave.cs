using UnityEngine;

public class Llave : Objeto {
    [Header("Configuración de la llave")]
    [Tooltip("Elige si desencadena acciones")]
    [SerializeField] private bool desencadenador = false;
    [Tooltip("Arrastra interactuables objetos para habilitarlos")]
    [SerializeField] private Interactuable[] Acciones;

    protected override void OnTriggerEnter2D(Collider2D colisionado) {
        //Llamamos al método del que hereda
        base.OnTriggerEnter2D (colisionado);

        //Detecta colisiones con el jugador
        if(colisionado.CompareTag("Jugador")) {
            //Activamos los objetos que son activables
            if(Acciones.Length > 0 && desencadenador) {
                //Se itera en cada objeto asignado, siendo que hay mínimo uno
                for (int i = 0; i < Acciones.Length; i++) {
                    //Buscamos si son objetos activables
                    if (Acciones[i]) {
                        //Invertimos su estado
                        Acciones[i].activo = !Acciones[i].activo;
                    }
                }
            }
        }
    }
}