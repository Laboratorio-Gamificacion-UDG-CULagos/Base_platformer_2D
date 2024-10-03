using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Resorte : Interactuable {
    [Header("Configuraci�n del resorte")]
    [Space(5)]
    [Tooltip("Elige si respeta un angulo personalizado")]
    [SerializeField] private bool anguloPersonalizado;
    [Tooltip("Asigna un �ngulo si est� activa la personalizaci�n"), Range(0, 359)]
    [SerializeField] private int anguloFuerza;
    [Space(5)]
    [Tooltip("Elige para usar una fuerza de lanzamiento")]
    [SerializeField] private bool arrojar = true;
    [Tooltip("Asigna un valor de fuerza de lanzamiento")]
    [SerializeField, Min(0)] private float factorLanzamiento = 5.0f;
    [Space(5)]
    [Tooltip("Asigna un tiempo de enfriamiento para el lanzamiento")]
    [SerializeField, Min(0)] private float tiempoEspera = 1.0f;
    [Space(5)]
    [Tooltip("Elige para a�adir una magnitud para limitar la fuerza")]
    [SerializeField] private bool limitar = true;
    [Tooltip("Asigna un valor para limitar el factor")]
    [SerializeField, Min(0)] private float limitador = 0.5f;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Referenc�a al RigidBody2D del jugador")]
    [SerializeField] private Rigidbody2D rbJugador;
    [Tooltip("Marca el estado actual del interactuable")]
    [SerializeField] private bool enEspera = false;
    [Tooltip("Arrastra el objeto hijo que contiene el sprite del interactuable")]
    [SerializeField] private Transform sprite;
    [Tooltip("Indica la direcci�n actual del funcionamiento")]
    [SerializeField] private Vector2 direccion;
    
    private void OnValidate() {
        ActualizarDireccion();
    }
    
    protected void Start() {
        ActualizarDireccion();
    }

    private void ActualizarDireccion() {
        //Obtener la rotaci�n Z del objeto y ajustar el �ngulo de lanzamiento
        if (!anguloPersonalizado) anguloFuerza = (int)transform.eulerAngles.z % 359;

        //Calcula la direcci�n de la normal del objeto seg�n el �ngulo
        float radianes = (anguloFuerza + 90) * Mathf.Deg2Rad;
        direccion = new Vector2(Mathf.Cos(radianes), Mathf.Sin(radianes));
    }

    private IEnumerator TiempoDeEspera(float espera) {
        //Activar el tiempo de espera
        enEspera = true;

        //Baja su sprite si est� asignado
        if (sprite) sprite.position += (Vector3)direccion * 0.2f;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Regresa a la posici�n original el sprite si existe
        while (sprite && sprite.localPosition.y > 0.0f) {
            sprite.position = Vector2.MoveTowards(sprite.position, transform.position, 0.01f);
            yield return null;
        }

        //Desactivar el tiempo de espera
        enEspera = false;
    }
    
    private void OnTriggerEnter2D(Collider2D colisionado) {
        //Detectamos la colisi�n con el jugador
        if (colisionado.CompareTag("Jugador") && !enEspera && activo) {
            //Referenciamos al jugador
            rbJugador = colisionado.GetComponent<Rigidbody2D>();

            //Obtenemos la velocidad del jugador
            Vector2 velocidadJugador = rbJugador.velocity;

            //Reflejamos la velocidad del jugador en relaci�n a la normal (rebote)
            Vector2 velocidadReflejada = Vector2.Reflect(velocidadJugador, direccion);
            rbJugador.velocity = velocidadReflejada;

            //Limitamos el movimiento vertical al doble de la fuerza del resorte si est� habilitado
            if (rbJugador.velocity.y > factorLanzamiento * limitador && limitar) rbJugador.velocity = new Vector2(rbJugador.velocity.x, factorLanzamiento * limitador);

            //Aplicamos una fuerza adicional en la direcci�n del resorte si est� habilitado
            if (arrojar) rbJugador.AddForce(direccion * factorLanzamiento * 100, ForceMode2D.Impulse);

            //Establecemos en espera el resorte
            StartCoroutine(TiempoDeEspera(tiempoEspera));
        }
    }

    private void OnDrawGizmos() {
        //Visualizamos la direcci�n de salida del portal
        Gizmos.color = Color.red;
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + (direccion * factorLanzamiento / 10) + direccion / 2);
    }
}