using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class CampoDeFuerza : Interactuable {
    [Header("Configuración del campo de fuerza")]
    [Tooltip("Asignar capas de interacción con el campo")]
    [SerializeField] private LayerMask afectables;
    [Tooltip("Especifica la velocidad de la plataforma (no estática)")]
    [SerializeField, Min(0)] private float fuerza = 1.0f;
    [Tooltip("Define si la fuerza se ejecuta propulsiva o repulsivamente")]
    [SerializeField] private bool invertir = true;
    [Space(5)]
    [Tooltip("Permite agregar un angulo personalizado")]
    [SerializeField] private bool anguloDinamico = false;
    [Tooltip("Asigna un ángulo si está activa la personalización")]
    [SerializeField, Range(0, 359)] private int anguloFuerza;
    [Space(5)]
    [Tooltip("Permite agregar un multiplicador por cercanía")]
    [SerializeField] private bool incremental = true;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Marca el estado actual del interactuable")]
    [SerializeField] private bool enEspera = false;
    [Tooltip("Marca el ángulo de dirección de la fuerza")]
    [SerializeField] private Vector2 direccion;

    private void OnValidate() {
        if (!anguloDinamico) ActualizarDireccion();
    }
    
    protected void Start() {
        ActualizarDireccion();
    }

    private void ActualizarDireccion() {
        //Obtener la rotación Z del objeto y ajustar el ángulo de lanzamiento
        anguloFuerza = (int)transform.eulerAngles.z % 359;

        //Calcula la fuerza según el input del usuario
        float radianes = (anguloFuerza + 90) * Mathf.Deg2Rad;
        direccion = new Vector2(Mathf.Cos(radianes), Mathf.Sin(radianes));
    }

    private void AplicarFuerza(Rigidbody2D rb) {
        //Calculamos la direccion si el angulo es dinamico
        if(anguloDinamico) direccion = rb.position - (Vector2)transform.position;

        //Calculamos la fuerza con la que se dirige
        Vector2 velocidad = fuerza * direccion / (0.01f + (incremental ? Vector2.Distance(rb.position, transform.position) : 1));

        //Aplicamos la fuerza sobre el objeto
        rb.AddForce((invertir ? -0.1f : 0.1f) * velocidad, ForceMode2D.Impulse);
    }

    private void OnTriggerStay2D(Collider2D colisionado) {
        //Actualizamos los objetos a partir de sus contactos constantes
        if (activo && !enEspera && (afectables & (1 << colisionado.gameObject.layer)) > 0) {
            //Buscamos que el objeto tenga un Rigidbody2D
            if (colisionado.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)) {
                //Movemos con fuerzas cada objeto que toca
                AplicarFuerza(rb);
            }
        }
    }
}