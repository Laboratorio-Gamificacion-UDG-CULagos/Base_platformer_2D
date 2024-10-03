using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Configuración del nivel")]
    [Space(5)]
    [Tooltip("Arrastra el prefab del jugador a usar en el nivel")]
    [SerializeField] private GameObject prefabJugador;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Marca la posición actual del ultimo punto de control")]
    [SerializeField] private Vector2 checkpoint;
    [Tooltip("Marca el valor del ultimo punto de control (conserva el mayor)")]
    [SerializeField, Min(0)] private int valorCheckpoint = 0;
    [Tooltip("Establece el objeto del personaje del jugador")]
    [SerializeField] private GameObject jugador;

    private void Start() {
        //Marcamos la posición del jugador default como spawn base
        if (jugador) 
            checkpoint = jugador.transform.position;        
        else if (prefabJugador) jugador = Instantiate(prefabJugador, checkpoint, Quaternion.identity);
    }

    public void CheckPoint(Vector2 posicion, int valor) {
        //Comparamos si es un checkpoint adelantado
        if (valor >= valorCheckpoint) {
            //Guardamos los valores recibidos
            valorCheckpoint = valor;
            checkpoint = posicion;
        }
    }

    public Vector2 SpawnPoint() { 
        //Devolvemos el sitio para revivir
        return checkpoint;
    }

    public int AvanceActual() {
        //Devolvemos el valor del checkpoint actual
        return valorCheckpoint;
    }
}