using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Bandera : Interactuable {
    [Header("Configuración de la bandera")]
    [Tooltip("Marca el estado cuando el jugador ya la ha usado")]
    [SerializeField] private bool marcada = false;
    [Tooltip("Arrastra objetos interactuables para habilitarlos")]
    [SerializeField] private Interactuable[] Acciones;
    [Space(5)]
    [Tooltip("Permite realizar cambios de escena al ser tocada")]
    [SerializeField] private bool cambiarEscenas = false;
    [Tooltip("Arrastra la escena (en Build) para ser cargada")]
    [SerializeField] private string escena;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Arrastra un sprite a mostrar default")]
    [SerializeField] private Sprite spriteDefault;
    [Tooltip("Arrastra un sprite a mostrar al marcarse")]
    [SerializeField] private Sprite spriteMarcada;

    protected override void Update() {
        //Llamamos al update heredado
        base.Update();

        //Animamos el estado de la bandera
        if (marcada && activo) GetComponent<SpriteRenderer>().sprite = spriteDefault;
        else GetComponent<SpriteRenderer>().sprite = spriteMarcada;
    }

    private void OnTriggerEnter2D(Collider2D colisionado) {
        //Detectamos la colisión con el jugador
        if (colisionado.CompareTag("Jugador") && activo) {
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
            //Cargar escena
            if (cambiarEscenas) SceneManager.LoadScene(escena);
        }
    }
}