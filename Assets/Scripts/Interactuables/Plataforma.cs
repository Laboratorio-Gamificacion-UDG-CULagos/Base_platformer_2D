using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(PlatformEffector2D))]
public class Plataforma : Interactuable {
    [Header("Configuración de la plataforma")]
    [Tooltip("Define si la plataforma se mueve o no")]
    [SerializeField] private bool estatica = true;
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
    [Tooltip("Asigna un tiempo de espera durante giros")]
    [SerializeField, Min(0)] protected float tiempoGiro = 0.1f;
    [Space(5)]
    [Tooltip("Establece la distancia maxima de recorrido")]
    [SerializeField, Min(0)] private float distancia = 10.0f;
    [Tooltip("Establece el tiempo hasta final de recorrido")]
    [SerializeField, Min(0)] private float tiempoCambio = 5.0f;
    [Space(5)]
    [Tooltip("Asignar capas de redirección por colisión")]
    [SerializeField] private LayerMask colisiones;
    [Space(5)]
    [Tooltip("Establecer direccionalidad")]
    [SerializeField] private bool unidireccional = false;
    [Tooltip("Permite arrastrado de objetos cargados")]
    [SerializeField] private bool cargar = true;
    [Space(5)]
    [Tooltip("Permite plataformas con caída")]
    [SerializeField] private bool dinamica = false;
    [Tooltip("Asigna un tiempo de espera para caer (dinámica)")]
    [SerializeField, Min(0)] protected float tiempoCaida = 1.0f;
    [Tooltip("Asigna un tiempo de espera para reaparecer (dinámica)")]
    [SerializeField, Min(0)] protected float tiempoSpawn = 2.5f;
    [Space(20)]

    [Header("DEV (Variables de control)")]
    [Tooltip("Marca el estado actual del interactuable")]
    [SerializeField] protected bool enEspera = false;
    [Tooltip("Marca el estado default de la direccion")]
    [SerializeField] protected bool direccion = true;
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
        direccion = moviendoDerecha;
    }

    protected override void Update() {
        //Llamamos al base heredada para conservar el comportamiento
        base.Update();

        //Comportamiento base
        if (!estatica && activo && !enEspera) MoverPlataforma();

        //Actualizar funcion del usuario
        if (effector) effector.enabled = unidireccional;
    }
    
    private void FixedUpdate() {
        //Movemos los objetos con los que tocamos
        if (activo && !estatica && !enEspera) MoverObjetos();
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
        if (arrastrables.Count > 0 && cargar) { 
            //Movemos cada objeto sobre la plataforma
            foreach (Rigidbody2D objeto in arrastrables) {
                //Aplicamos el delta de movimiento para arrastrarlo
                objeto.position += (moviendoDerecha? 1 : -1) * Time.deltaTime * velocidad * Vector2.right;
            }
        }
    }

    private void CambiarDireccion() {
        //Establecemos en espera del heridor
        StartCoroutine(TiempoDeGiro(tiempoGiro));

        //Invertir direcciones
        moviendoDerecha = !moviendoDerecha;
        ResetAvance();
    }

    private void ResetAvance() {
        //Reset de variables
        tiempoActual = tiempoCambio;
        distanciaRecorrida = 0.0f;
        rbPlataforma.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D colisionado) {
        //Verificamos si estamos tocando un objeto arrastrable
        if (activo && (effector.colliderMask & (1 << colisionado.gameObject.layer)) > 0) {
            //Comprobamos si es posible arrastrarlo con su RigidBody2D
            if (colisionado.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rbObjeto)) {
                //Añadimos nuestro objeto a la lista de arrastrables si está en posición
                if (rbObjeto.position.y > transform.position.y + GetComponent<SpriteRenderer>().size.y) arrastrables.Add(rbObjeto);

                //Manejamos la lógica de plataformas dinámcias
                if (colisionado.gameObject.CompareTag("Jugador") && dinamica && !enEspera && rbObjeto.velocity.y <= 0) StartCoroutine(TiempoDeCaida(tiempoCaida));
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
    
    protected virtual IEnumerator TiempoDeGiro(float espera) {
        //Activar el tiempo de espera
        enEspera = true;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Desactivar el tiempo de espera
        enEspera = false;
    }

    protected virtual IEnumerator TiempoDeCaida(float espera) {
        //Cambiamos forma de funcionamiento
        enEspera = true;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Desactivar el tiempo de espera
        rbPlataforma.gravityScale = 1;
        StartCoroutine(TiempoDeEspera(tiempoSpawn));
    }
    
    protected virtual IEnumerator TiempoDeEspera(float espera) {
        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Desactivar el tiempo de espera
        rbPlataforma.gravityScale = 0;
        enEspera = false;
        transform.position = posicionInicial;
        moviendoDerecha = direccion;
        ResetAvance();
    }

    private void OnDrawGizmos() {
        //Mostramos en el Editor los Rays de dirección
        Debug.DrawRay(transform.position, (moviendoDerecha ? -1 : 1) * 0.5f * GetComponent<SpriteRenderer>().size.x * Vector2.left, Color.red);

        //Mostramos la dirección y magnitud del delta
        Debug.DrawRay(transform.position, (moviendoDerecha ? 2 : -2) * Time.deltaTime * velocidad * Vector2.left, Color.blue);
    }
}