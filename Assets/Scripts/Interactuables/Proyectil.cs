using UnityEngine;

public class Proyectil : Heridor {
    [Header("Configuración del proyectil")]
    [Tooltip("Establece (en segundos) el tiempo de vida del proyectil")]
    [SerializeField, Min(0)] private float vida = 10.0f;
    [Tooltip("Determina si desaparece al colisionar con jugadores")]
    [SerializeField] private bool oneHit;

    private void Start() {
        //Asignamos un tiempo para eliminar el proyectil
        Destroy(gameObject, vida);
    }

    protected override void OnTriggerEnter2D(Collider2D colisionado) {
        //Llamamos al método heredado
        base.OnTriggerEnter2D(colisionado);

        //Eliminamos al proyectil según su colisión
        if (colisionado.CompareTag("Jugador") && oneHit) Destroy(gameObject);
        else if (!colisionado.CompareTag("Jugador")) Destroy(gameObject);
    }
}