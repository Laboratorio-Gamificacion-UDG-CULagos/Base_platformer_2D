using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Puas : Interactuable {
    [Header("Configuración de las púas")]
    [Tooltip("Asigna un tiempo de enfriamiento las púas")]
    [SerializeField, Min(0)] private float tiempoEspera = 2.0f;
    [Space(5)]
    [Tooltip("Elige si respeta un angulo personalizado")]
    [SerializeField] private bool anguloPersonalizado;
    [Tooltip("Asigna un ángulo si está activa la personalización"), Range(0, 359)]
    [SerializeField] private int anguloFuerza;
    [Space(5)]
    [Tooltip("Elige el valor de daño al contacto")]
    [SerializeField, Min(0)] private int golpe = 1;
    [Tooltip("Permite lanzar al jugador de regreso")]
    [SerializeField] private bool repeler;
    [Tooltip("Agrega un multiplicador de repulsión")]
    [SerializeField, Min(0)] private float factorRepulsion = 1.0f;
    [Tooltip("Anula el movimiento del jugador")]
    [SerializeField] private bool detener;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [SerializeField] private Transform sprite;
    private bool enEspera = false;
    private Vector2 direccion;

    private void OnValidate() {
        ActualizarDireccion();
    }
    
    protected void Start() {
        ActualizarDireccion();
    }

    private void ActualizarDireccion() {
        //Obtener la rotación Z del objeto y ajustar el ángulo de lanzamiento
        if(!anguloPersonalizado) anguloFuerza = (int)transform.eulerAngles.z % 359;

        //Calcula la dirección de la normal del resorte según el ángulo
        float radianes = (anguloFuerza + 90) * Mathf.Deg2Rad;
        direccion = new Vector2(Mathf.Cos(radianes), Mathf.Sin(radianes));
    }

    private IEnumerator TiempoDeEspera(float espera) {
        //Activar el tiempo de espera del resorte
        enEspera = true;

        //Baja su sprite si está asignado
        if (sprite) sprite.position += (Vector3)direccion * 0.4f;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Regresa a la posición original el sprite si existe
        while (sprite && sprite.localPosition.y > 0.0f) {
            sprite.position = Vector2.MoveTowards(sprite.position, transform.position, 0.01f);
            yield return null;
        }

        //Desactivar el tiempo de espera del resorte
        enEspera = false;
    }
    
    private void OnTriggerEnter2D(Collider2D colisionado) {
        //Detectamos la colisión con el jugador
        if (colisionado.CompareTag("Jugador") && !enEspera && activo) {
            //Actualizamos su vida
            colisionado.GetComponent<Personaje>().vida -= golpe;

            //Checamos si habilita repeler
            if (repeler) {
                Rigidbody2D rbJugador = colisionado.GetComponent<Rigidbody2D>();
                Vector2 nuevaVelocidad = new Vector2(-rbJugador.velocity.x, -rbJugador.velocity.y) * factorRepulsion;
                rbJugador.velocity = nuevaVelocidad;
            } else if (detener) {
                colisionado.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }

            //Establecemos en espera de las púas
            StartCoroutine(TiempoDeEspera(tiempoEspera));
        }
    }

    private void OnDrawGizmos() {
        //Visualizamos la dirección de salida del portal
        Gizmos.color = Color.red;
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + direccion / 2);
    }
}