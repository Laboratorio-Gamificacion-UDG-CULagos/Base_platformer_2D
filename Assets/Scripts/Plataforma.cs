using UnityEngine;

public class Plataforma : MonoBehaviour {
    [Header("Configuración de la plataforma")]
    [Tooltip("Define si la plataforma se mueve o no")]
    [SerializeField] private bool esEstatica = true;
    [Tooltip("Especifica la velocidad(no estática)")]
    [SerializeField] private float velocidad = 1f;
    [Tooltip("Establece la distancia maxima de recorrido")]
    [SerializeField] private float distancia = 10f;
    [Tooltip("Establece el tiempo hasta final de recorrido")]
    [SerializeField] private float tiempoDeCambio = 5f;
    [Tooltip("Establecer dirección de movimiento")]
    [SerializeField] private bool moviendoDerecha = true;
    private float tiempoActual;
    private float distanciaRecorrida;
    private Vector3 posicionInicial;
    private Rigidbody2D rbPlataforma;

    [Header("Cambios de dirección")]
    [Tooltip("Permitir redirección por tiempo")]
    [SerializeField] private bool cambiarPorTiempo = true;
    [Tooltip("Permitir redirección por distancia")]
    [SerializeField] private bool cambiarPorDistancia = false;
    [Tooltip("Asignar capas de redirección por colisión")]
    [SerializeField] private LayerMask colisiones;

    [Header("Interacción con Jugador")]
    [Tooltip("Establecer direccionalidad")]
    [SerializeField] private bool unidireccional = false;
    private PlatformEffector2D effector;

    private void Awake() {
        //Obteniendo parametros iniciales
        posicionInicial = transform.position;
        tiempoActual = tiempoDeCambio;
        distanciaRecorrida = 0f;
        rbPlataforma = GetComponent<Rigidbody2D>();
        effector = GetComponent<PlatformEffector2D>();
    }

    private void Update() {
        //Comportamiento base
        if (!esEstatica) MoverPlataforma();

        //Actualizar funcion del usuario
        if (effector) effector.enabled = unidireccional;

        //Mostramos en el Editor los Rays
        Debug.DrawRay(transform.position, Vector2.left * 0.5f * GetComponent<SpriteRenderer>().size.x  * (moviendoDerecha ? -1 : 1), Color.red);

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
}