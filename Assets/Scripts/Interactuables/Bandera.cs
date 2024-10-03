using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Bandera : Interactuable {
    [Header("Configuración de la bandera")]
    [Tooltip("Marca el estado cuando el jugador ya la ha usado")]
    [SerializeField] private bool marcada = false;
    [Tooltip("Arrastra objetos interactuables para habilitarlos")]
    [SerializeField] private Interactuable[] acciones;
    [Space(5)]
    [Tooltip("Permite realizar cambios de escena al ser tocada")]
    [SerializeField] private bool cambiarEscenas = false;
    [Tooltip("Arrastra la escena (en Build) para ser cargada")]
    [SerializeField] private string escena;
    [Space(10)]
    [Tooltip("Arrastra un sprite a mostrar al marcarse")]
    [SerializeField] private GameManager nivel;
    [Tooltip("Inserta el valor (ascendente) el checkpoint")]
    [SerializeField, Min(1)] private int valor = 1;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Arrastra un sprite a mostrar default")]
    [SerializeField] private Sprite spriteDefault;
    [Tooltip("Arrastra un sprite a mostrar al marcarse")]
    [SerializeField] private Sprite spriteMarcada;

    private void Awake() {
        if (!nivel) {
            nivel = FindObjectOfType<GameManager>();
        }
    }

    protected override void Update() {
        //Llamamos al update heredado
        base.Update();

        //Actualizamos segun el nivel
        if (nivel && nivel.AvanceActual() >= valor) marcada = true;

        //Animamos el estado de la bandera
        if (marcada) GetComponent<SpriteRenderer>().sprite = spriteMarcada;
        else GetComponent<SpriteRenderer>().sprite = spriteDefault;
    }

    private void OnTriggerEnter2D(Collider2D colisionado) {
        //Detectamos la colisión con el jugador
        if (colisionado.CompareTag("Jugador") && activo) {
            //Activamos los objetos que son interactuables
            if (acciones.Length > 0) { 
                //Se itera en cada objeto asignado, siendo que hay mínimo uno
                for(int i = 0; i < acciones.Length; i++) {
                    //Buscamos si son objetos interactuables
                    if (acciones[i]) {
                        //Invertimos su estado
                        acciones[i].activo = !acciones[i].activo;
                    }
                }
            }

            //Invertir estado de la bandera
            activo = !activo;

            //Marcamos la bandera y recordamos posición
            marcada = !marcada;
            nivel.CheckPoint(transform.position, valor);

            //Cargar una escena si se habilita
            if (cambiarEscenas) SceneManager.LoadScene(escena);
        }
    }
}