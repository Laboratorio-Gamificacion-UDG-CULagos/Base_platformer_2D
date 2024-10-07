using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Resorte : Interactuable {
    [Header("Configuración del resorte")]
    [Space(5)]
    [Tooltip("Elige para usar una fuerza de lanzamiento")]
    [SerializeField] private bool instantaneo = true;
    [Tooltip("Asignar capas habilitadas para lanzar")]
    [SerializeField] private LayerMask lanzables;
    [Space(5)]
    [Tooltip("Elige si respeta un angulo personalizado")]
    [SerializeField] private bool anguloPersonalizado;
    [Tooltip("Asigna un ángulo si está activa la personalización")]
    [SerializeField, Range(0, 359)] private int anguloFuerza;
    [Space(5)]
    [Tooltip("Elige para usar una fuerza de lanzamiento")]
    [SerializeField] private bool arrojar = true;
    [Tooltip("Asigna un valor de fuerza de lanzamiento")]
    [SerializeField, Min(0)] private float factorLanzamiento = 5.0f;
    [Space(5)]
    [Tooltip("Asigna un tiempo de enfriamiento para el lanzamiento")]
    [SerializeField, Min(0)] private float tiempoEspera = 1.0f;
    [Tooltip("Asigna un tiempo de lanzamiento")]
    [SerializeField, Min(0)] private float tiempoProceso = 0.5f;
    [Space(5)]
    [Tooltip("Elige para añadir una magnitud para limitar la fuerza")]
    [SerializeField] private bool limitar = true;
    [Tooltip("Asigna un valor para limitar el factor")]
    [SerializeField, Min(0)] private float limitador = 0.5f;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Referencía al sprite renderer del objeto")]
    [SerializeField] private SpriteRenderer sp;
    [Tooltip("Referencía al Rigidbody2D del objeto a lanzar")]
    [SerializeField] private List<Rigidbody2D> rbs;
    [Tooltip("Marca el estado actual del interactuable")]
    [SerializeField] private bool enEspera = false;
    [Tooltip("Marca el estado actual del interactuable")]
    [SerializeField] private bool porLanzar = false;
    [Tooltip("Arrastra el sprite para mostrar por default")]
    [SerializeField] private Sprite spriteDefault;
    [Tooltip("Arrastra el sprite para mostrar en espera")]
    [SerializeField] private Sprite spriteEspera;
    [Tooltip("Arrastra el sprite para mostrar durante carga")]
    [SerializeField] private Sprite spriteCarga;
    [Tooltip("Indica la dirección actual del funcionamiento")]
    [SerializeField] private Vector2 direccion;
    
    private void OnValidate() {
        ActualizarDireccion();
    }

    private void Awake() {
        //Obtenemos parámetros iniciales
        if (!sp) sp = GetComponent<SpriteRenderer>();
    }

    protected void Start() {
        ActualizarDireccion();
    }

    protected override void Update() {
        //Llamamos al método del que hereda
        base.Update();

        //Animamos el comportamiento del resorte
        if (enEspera || !activo) sp.sprite = spriteEspera;
        else if (porLanzar) sp.sprite = spriteCarga;
        else sp.sprite = spriteDefault;
    }

    private void ActualizarDireccion() {
        //Obtener la rotación Z del objeto y ajustar el ángulo de lanzamiento
        if (!anguloPersonalizado) anguloFuerza = (int)transform.eulerAngles.z % 359;

        //Calcula la dirección de la normal del objeto según el ángulo
        float radianes = (anguloFuerza + 90) * Mathf.Deg2Rad;
        direccion = new Vector2(Mathf.Cos(radianes), Mathf.Sin(radianes));
    }

    private void OnTriggerEnter2D(Collider2D colisionado) {
        //Detectamos la colisión con el jugador
        if (((lanzables & (1 << colisionado.gameObject.layer)) > 0) && !enEspera && activo) {
            //Referenciamos al jugador
            if (colisionado.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)) {
                rbs.Add(rb);
                if (instantaneo) ActivarRebote();
                else StartCoroutine(TiempoDeLanzado(tiempoProceso));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D colisionado) {
        if (rbs.Count > 0 && colisionado.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb) && rbs.Contains(rb)) rbs.Remove(rb);
    }

    private void ActivarRebote() {
        foreach (Rigidbody2D rb in rbs) { 
            //Obtenemos la velocidad del jugador
            Vector2 velocidadJugador = rb.velocity;

            //Reflejamos la velocidad del jugador en relación a la normal (rebote)
            Vector2 velocidadReflejada = Vector2.Reflect(velocidadJugador, direccion);
            rb.velocity = velocidadReflejada;

            //Limitamos el movimiento vertical al doble de la fuerza del resorte si está habilitado
            if (rb.velocity.y > factorLanzamiento * limitador && limitar) rb.velocity = new Vector2(rb.velocity.x, factorLanzamiento * limitador);

            //Aplicamos una fuerza adicional en la dirección del resorte si está habilitado
            if (arrojar) rb.AddForce(direccion * factorLanzamiento * 100, ForceMode2D.Impulse);
        }

        //Establecemos en espera el resorte
        StartCoroutine(TiempoDeEspera(tiempoEspera));
    }

    private IEnumerator TiempoDeEspera(float espera) {
        //Activar el tiempo de espera
        enEspera = true;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Desactivar el tiempo de espera
        enEspera = false;
    }
    
    private IEnumerator TiempoDeLanzado(float espera) {
        //Activar el tiempo de espera
        porLanzar = true;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Desactivar el tiempo de espera
        porLanzar = false;

        //Comprobamos que aún hay objetos en contacto y procesamos
        if (rbs.Count > 0) ActivarRebote();
    }
    
    private void OnDrawGizmos() {
        //Visualizamos la dirección de salida del portal
        Gizmos.color = Color.red;
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + (direccion * factorLanzamiento / 10) + direccion / 2);
    }
}