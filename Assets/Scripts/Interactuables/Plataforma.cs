using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(PlatformEffector2D))]
public class Plataforma : Interactuable {
    [Header("Configuración de la plataforma")]
    [Tooltip("Define si la plataforma se mueve o no")]
    [SerializeField] private bool esEstatica = true;
    [Tooltip("Especifica la velocidad de la plataforma (no estática)")]
    [SerializeField, Min(0)] private float velocidad = 1.0f;
    [Space(5)]
    [Tooltip("Establecer dirección de movimiento")]
    [SerializeField] private bool moviendoDerecha = true;
    [Space(5)]
    [Tooltip("Permitir redirección por tiempo")]
    [SerializeField] private bool cambiarPorTiempo = true;
    [Tooltip("Permitir redirección por distancia")]
    [SerializeField] private bool cambiarPorDistancia = false;
    [Space(5)]
    [Tooltip("Establece la distancia maxima de recorrido")]
    [SerializeField, Min(0)] private float distancia = 10.0f;
    [Tooltip("Establece el tiempo hasta final de recorrido")]
    [SerializeField, Min(0)] private float tiempoDeCambio = 5.0f;
    [Space(5)]
    [Tooltip("Asignar capas de redirección por colisión")]
    [SerializeField] private LayerMask colisiones;
    [Space(5)]
    [Tooltip("Establecer direccionalidad")]
    [SerializeField] private bool unidireccional = false;
    [Space(20)]

    [Header("DEV (Variables de control)")]
    [Tooltip("Referencía al RigidBody2D de la plataforma")]
    [SerializeField] private Rigidbody2D rbPlataforma;
    [Tooltip("Referencía al PlatformEffector2D de la plataforma")]
    [SerializeField] private PlatformEffector2D effector;
    [Tooltip("Marca el tiempo que lleva en movimiento desde el último giro")]
    [SerializeField] private float tiempoActual;
    [Tooltip("Marca la distancia del movimiento desde el último giro")]
    [SerializeField] private float distanciaRecorrida = 0.0f;
    [Tooltip("Marca la posición desde donde se generó")]
    [SerializeField] private Vector2 posicionInicial;
    [Tooltip("Selecciona los RigidBody2D de los objetos por mover")]
    [SerializeField] private List<Rigidbody2D> arrastrables;

    protected void Awake() {
        //Checamos que tenga la información correcta
        if (!rbPlataforma) rbPlataforma = GetComponent<Rigidbody2D>();
        if (!effector) effector = GetComponent<PlatformEffector2D>();

        //Obteniendo parámetros iniciales
        posicionInicial = transform.position;
    }

    protected override void Update() {
        //Llamamos al base heredada para conservar el comportamiento
        base.Update();

        //Comportamiento base
        if (!esEstatica && activo) MoverPlataforma();
        else if (!activo) rbPlataforma.velocity = Vector2.zero;

        //Actualizar funcion del usuario
        if (effector) effector.enabled = unidireccional;
    }
    
    private void FixedUpdate() {
        //Movemos los objetos con los que tocamos
        if (arrastrables.Count > 0) MoverObjetos();
    }

    private bool DetectarMuro(int dir) {
        //Raycasteamos para detectar un muro en la direccion que avanza
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left * dir, 0.5f * GetComponent<SpriteRenderer>().size.x, colisiones);
        return hit.collider != null;
    }

    private void MoverPlataforma() {
        //Claculamos el control de movimiento
        float direccion = moviendoDerecha ? 1 : -1;
        Vector2 fuerza = new Vector2(direccion * velocidad, rbPlataforma.velocity.y);
        //Detectamos muros por cercanía
        bool hayMuro = DetectarMuro(moviendoDerecha ? -1 : 1);

        //Cooldown para cambio de direccion
        if (cambiarPorTiempo) {
            tiempoActual -= Time.deltaTime;
            if (tiempoActual <= 0) {
                CambiarDireccion();
            }
        }

        //Procesamiento para cambio por distancia y colisión
        distanciaRecorrida += Mathf.Abs(fuerza.x * Time.deltaTime);
        if (cambiarPorDistancia && distanciaRecorrida >= distancia || hayMuro) CambiarDireccion();

        //Movimiento base
        rbPlataforma.velocity = fuerza;
    }

    private void MoverObjetos() {
        //Movemos cada objeto sobre la plataforma
        foreach (Rigidbody2D objeto in arrastrables) {
            //Aplicamos el delta de movimiento para arrastrarlo
            objeto.position += Time.deltaTime * Vector2.right * (moviendoDerecha? 1 : -1) * velocidad;
        }
    }

    private void CambiarDireccion() {
        //Invertir direcciones y resetear variables
        moviendoDerecha = !moviendoDerecha;
        tiempoActual = tiempoDeCambio;
        distanciaRecorrida = 0.0f;
        rbPlataforma.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D colisionado) {
        //Verificamos si estamos tocando un objeto arrastrable
        if (effector && activo && (effector.colliderMask & (1 << colisionado.gameObject.layer)) > 0) {
            //Comprobamos si es posible arrastrarlo con su RigidBody2D
            if (colisionado.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rbObjeto)) {
                //Añadimos nuestro objeto a la lista de arrastrables si está en posición
                if (rbObjeto.position.y > transform.position.y + GetComponent<SpriteRenderer>().size.y) arrastrables.Add(rbObjeto);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D colisionado) {
        //Comprobamos que es posible arrastrarlo con su RigidBody2D
        if (colisionado.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rbObjeto)) {
            //Añadimos nuestro objeto a la lista de arrastrables
            if (arrastrables.Contains(rbObjeto)) {
                arrastrables.Remove(rbObjeto);
            }
        }
    }

    private void OnDrawGizmos() {
        //Mostramos en el Editor los Rays de dirección
        Debug.DrawRay(transform.position, (moviendoDerecha ? -1 : 1) * 0.5f * GetComponent<SpriteRenderer>().size.x * Vector2.left, Color.red);

        //Mostramos la dirección y magnitud del delta
        Debug.DrawRay(transform.position, (moviendoDerecha ? 2 : -2) * Time.deltaTime * velocidad * Vector2.left, Color.blue);
    }
}