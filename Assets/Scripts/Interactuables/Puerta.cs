using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Puerta : Interactuable {
    [Header("Configuración de la puerta")]
    [Tooltip("Permite realizar cambios de escena al ser tocada")]
    [SerializeField] private bool cambiarEscenas = true;
    [Tooltip("Arrastra la escena (en Build) para ser cargada")]
    [SerializeField] private string escena;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Arrastra un sprite a mostrar al abrirse")]
    [SerializeField] private Sprite spriteAbierta;
    [Tooltip("Arrastra un sprite a mostrar al cerrarse")]
    [SerializeField] private Sprite spriteCerrada;

    protected override void Update() {
        //Llamamos al update heredado
        base.Update();

        //Animamos su estado
        if (activo) GetComponent<SpriteRenderer>().sprite = spriteAbierta;
        else GetComponent<SpriteRenderer>().sprite = spriteCerrada;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        //Checamos colisiones con jugadores
        if (collision.CompareTag("Jugador") && activo) {
            //Cargar escena
            if (cambiarEscenas) SceneManager.LoadScene(escena);
        }
    }
}