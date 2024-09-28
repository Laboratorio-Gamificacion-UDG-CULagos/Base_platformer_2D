using UnityEngine;
using System.Collections;

public class Empujador : MonoBehaviour {
    [Header("Configuración del resorte")]
    [Tooltip("Permite activar y desactivar el comportamiento del resorte")]
    [SerializeField] private bool activo = true;
    [Tooltip("Vincula el sprite del resorte para animarlo")]
    [SerializeField] private Transform sprite;
    [Tooltip("Permite establecer un angulo personalizado")]
    [SerializeField] private bool anguloPersonalizado;
    [Tooltip("Asigna un ángulo si está activa la personalización")]
    [Range(0, 359)]
    [SerializeField] private int anguloFuerza;
    [Tooltip("Permite añadir una fuerza de lanzamiento")]
    [SerializeField] private bool arrojar = true;
    [Tooltip("Asigna un valor de fuerza de lanzamiento")]
    [SerializeField] private float factorLanzamiento = 5.0f;
    [Tooltip("Asigna un tiempo de enfriamiento para el lanzamiento")]
    [SerializeField] private float tiempoEspera = 1.0f;
    [Tooltip("Permite añadir una magnitud para limitar la fuerza")]
    [SerializeField] private bool limitar = true;
    [Tooltip("Asigna un valor para limitar el factor")]
    [SerializeField] private float limitador = 0.5f;

    private bool enEspera = false;
    private Vector2 direccion;
    private Rigidbody2D rbJugador;

    private void Start() {
        //Obtener la rotación Z del objeto y ajustar el ángulo de lanzamiento
        if(!anguloPersonalizado) anguloFuerza = (int)transform.eulerAngles.z % 359;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        //Detectamos la colisión con el jugador
        if (collision.CompareTag("Jugador") && !enEspera && activo) {
            //Referenciamos al jugador
            rbJugador = collision.GetComponent<Rigidbody2D>();

            //Calcula la dirección de la normal del resorte según el ángulo
            float radianes = (anguloFuerza + 90) * Mathf.Deg2Rad;
            direccion = new Vector2(Mathf.Cos(radianes), Mathf.Sin(radianes));

            //Obtenemos la velocidad del jugador
            Vector2 velocidadJugador = rbJugador.velocity;

            //Reflejamos la velocidad del jugador en relación a la normal (rebote)
            Vector2 velocidadReflejada = Vector2.Reflect(velocidadJugador, direccion);

            //Aplicamos la velocidad reflejada al jugador
            rbJugador.velocity = velocidadReflejada;

            //Limitamos el movimiento vertical al doble de la fuerza del resorte si está habilitado
            if (rbJugador.velocity.y > factorLanzamiento * limitador && limitar) rbJugador.velocity = new Vector2(rbJugador.velocity.x, factorLanzamiento * limitador);

            //Aplicamos una fuerza adicional en la dirección del resorte si está habilitado
            if (arrojar) rbJugador.AddForce(direccion * factorLanzamiento * 100, ForceMode2D.Impulse);

            //Establecemos en espera el resorte
            StartCoroutine(TiempoDeEspera(tiempoEspera));
        }
    }

    private IEnumerator TiempoDeEspera(float espera) {
        //Activar el tiempo de espera del resorte
        enEspera = true;

        //Baja su sprite si está asignado
        if (sprite) sprite.position += (Vector3)direccion * 0.2f;

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

    private void OnDrawGizmos() {
        //Visualizamos la dirección de salida del portal
        Gizmos.color = Color.red;
        Vector2 finalPos = new Vector2(Mathf.Cos((anguloFuerza + 90) * Mathf.Deg2Rad), Mathf.Sin((anguloFuerza + 90) * Mathf.Deg2Rad));
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + (finalPos * factorLanzamiento / 10));
    }
}