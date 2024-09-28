using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlatformEffector2D))]
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

    [HideInInspector, SerializeField] private Rigidbody2D rbPlataforma;
    [HideInInspector, SerializeField] private PlatformEffector2D effector;
    private float tiempoActual;
    private float distanciaRecorrida = 0.0f;
    private Vector3 posicionInicial;

    protected void Awake() {
        //Obteniendo parametros iniciales
        posicionInicial = transform.position;
    }

    protected override void Update() {
        //Llamamos al base heredada para conservar el comportamiento
        base.Update();

        //Comportamiento base
        if (!esEstatica && activo) MoverPlataforma();
        else if (!activo) rbPlataforma.velocity = Vector3.zero;

        //Actualizar funcion del usuario
        if (effector) effector.enabled = unidireccional;

        // Detectar si hay un muro a la izquierda o a la derecha
        bool hayMuro = DetectarMuro(moviendoDerecha ? -1 : 1);
        //Al tocar algo invertir el movimiento
        if (hayMuro) CambiarDireccion();
    }

    bool DetectarMuro(int dir) {
        //Raycasteamos para detectar un muro en la direccion que avanza
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left * dir, 0.5f * GetComponent<SpriteRenderer>().size.x, colisiones);
        return hit.collider != null;
    }

    private void MoverPlataforma() {
        //Movimiento base
        float direccion = moviendoDerecha ? 1 : -1;
        Vector2 fuerza = new Vector2(direccion * velocidad, rbPlataforma.velocity.y);
        rbPlataforma.velocity = fuerza;

        //Procesamiento para cambio por distancia
        distanciaRecorrida += Mathf.Abs(fuerza.x * Time.deltaTime);
        if (cambiarPorDistancia && distanciaRecorrida >= distancia) {
            CambiarDireccion();
        }

        //Cooldown para cambio de direccion
        if (cambiarPorTiempo) {
            tiempoActual -= Time.deltaTime;
            if (tiempoActual <= 0) {
                CambiarDireccion();
            }
        }
    }

    private void CambiarDireccion() {
        //Invertir direcciones y resetear variables
        moviendoDerecha = !moviendoDerecha;
        tiempoActual = tiempoDeCambio;
        distanciaRecorrida = 0f;
    }
    
    private void OnDrawGizmos() {
        //Mostramos en el Editor los Rays de dirección
        Debug.DrawRay(transform.position, Vector2.left * 0.5f * GetComponent<SpriteRenderer>().size.x * (moviendoDerecha ? -1 : 1), Color.red);
    }
}