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
    [Tooltip("Marca el estado actual del interactuable")]
    [SerializeField] protected bool enGiro = false;
    [Tooltip("Marca el estado actual del interactuable")]
    [SerializeField] protected bool enCaida = false;
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
    }

    private void Start() {
        //Obteniendo parámetros iniciales
        posicionInicial = transform.position;
        direccion = moviendoDerecha;

        //Establecemos valor por default
        if (estatica) rbPlataforma.bodyType = RigidbodyType2D.Static;

        //Comenzamos su comportamiento
        if (activo && !estatica && !enGiro) MoverPlataforma();
    }

    protected override void Update() {
        //Llamamos al base heredada para conservar el comportamiento
        base.Update();

        //Actualizar funcion del usuario
        if (effector) effector.enabled = unidireccional;

        //Comportamiento base
        if (activo && !estatica && !enGiro && !enEspera) DetectarLimites();
    }
    
    private void FixedUpdate() {
        //Movemos los objetos con los que tocamos
        if (activo && !estatica && !enGiro) MoverObjetos();

        //Detener cuando sea estatica
        if (estatica && rbPlataforma.bodyType != RigidbodyType2D.Static && !enEspera) rbPlataforma.bodyType = RigidbodyType2D.Static;
    }

    private bool DetectarMuro(int direccion) {
        //Raycasteamos para detectar un muro en la direccion que avanza
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * direccion, 0.5f * GetComponent<SpriteRenderer>().size.x, colisiones);
        return hit.collider;
    }

    private void MoverPlataforma() {
        //Claculamos el control de movimiento
        int direccion = moviendoDerecha ? 1 : -1;
        Vector2 fuerza = new(direccion * velocidad, rbPlataforma.velocity.y);

        //Movimiento base
        rbPlataforma.velocity = fuerza;
    }

    private void DetectarLimites() {
        int direccion = moviendoDerecha ? 1 : -1;
        Vector2 fuerza = new(direccion * velocidad, rbPlataforma.velocity.y);

        //Detectamos muros por cercanía
        bool hayMuro = DetectarMuro(direccion);

        //Detener en limite para evitar caída
        if (hayMuro && enCaida) return;

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
    }

    private void MoverObjetos() {
        //Revisamos si existen objetos
        if (arrastrables.Count > 0 && cargar) { 
            //Movemos cada objeto sobre la plataforma
            foreach (Rigidbody2D objeto in arrastrables) {
                //Aplicamos el delta de movimiento para arrastrarlo
                objeto.position += (moviendoDerecha? 1 : -1) * Time.deltaTime * velocidad * Vector2.right;
            }
        }
    }

    private void CambiarDireccion() {
        //Establecemos en espera el objeto
        StartCoroutine(TiempoDeGiro(tiempoGiro));

        //Invertir direcciones
        moviendoDerecha = !moviendoDerecha;
        ResetAvance();
    }

    private void ResetAvance() {
        //Resets de variables
        tiempoActual = tiempoCambio;
        distanciaRecorrida = 0.0f;

        //Resets de movimientos
        rbPlataforma.bodyType = RigidbodyType2D.Kinematic;
        if (enEspera) rbPlataforma.velocity = Vector2.zero;
        rbPlataforma.gravityScale = 0;
    }

    private void OnCollisionEnter2D(Collision2D colisionado) {
        //Verificamos si estamos tocando un objeto arrastrable
        if (activo && (effector.colliderMask & (1 << colisionado.gameObject.layer)) > 0) {
            //Comprobamos si es posible arrastrarlo con su RigidBody2D
            Rigidbody2D rbObjeto = colisionado.rigidbody;

            //Añadimos nuestro objeto a la lista de arrastrables si está en posición
            if (rbObjeto.position.y > transform.position.y + GetComponent<SpriteRenderer>().size.y) {
                arrastrables.Add(rbObjeto);

                //Manejamos la lógica de plataformas dinámcias
                if (colisionado.gameObject.CompareTag("Jugador") && dinamica && rbObjeto.velocity.y <= 0 && !enCaida) StartCoroutine(TiempoDeCaida(tiempoCaida));
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
        enGiro = true;
        //Detenemos el movimiento temporalmente
        rbPlataforma.velocity = Vector2.zero;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Desactivar el tiempo de espera
        enGiro = false;
        //Reactivamos el movimiento
        MoverPlataforma();
    }

    protected virtual IEnumerator TiempoDeCaida(float espera) {
        //Activar el tiempo de espera
        enCaida = true;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);
        
        //Hacemos caer la plataforma
        rbPlataforma.bodyType = RigidbodyType2D.Dynamic;
        rbPlataforma.gravityScale = 2;

        //Desactivar el tiempo de espera
        enCaida = false;
        //Ponemos en periodo de reaparecimiento
        StartCoroutine(TiempoDeEspera(tiempoSpawn));
    }
    
    protected virtual IEnumerator TiempoDeEspera(float espera) {
        //Cambiamos forma de funcionamiento
        enEspera = true;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Hacemos reset de spawneo
        ResetAvance();
        //Desactivar el tiempo de espera
        enEspera = false;

        //Reiniciamos valores extra
        transform.position = posicionInicial;
        moviendoDerecha = direccion;
        //Reactivamos el movimiento
        MoverPlataforma();
    }

    private void OnDrawGizmos() {
        //Mostramos en el Editor los Rays de dirección
        Debug.DrawRay(transform.position, (moviendoDerecha ? 1 : -1) * 0.5f * GetComponent<SpriteRenderer>().size.x * Vector2.right, Color.red);

        //Mostramos la dirección y magnitud del delta
        Debug.DrawRay(transform.position, (moviendoDerecha ? 2 : -2) * Time.deltaTime * velocidad * Vector2.left, Color.blue);
    }
}