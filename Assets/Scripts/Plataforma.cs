using UnityEngine;

public class Plataforma : MonoBehaviour
{
    [Header("Movimiento y parámetros")]
    public bool esEstatica = true;
    public float velocidad = 1f;
    public float distancia = 10f;
    public float tiempoDeCambio = 5f;
    public bool moviendoDerecha = true;
    private float tiempoActual;
    private float distanciaRecorrida;
    private Vector3 posicionInicial;
    private Rigidbody2D rbPlataforma;

    [Header("Cambios de dirección")]
    public bool cambiarPorTiempo = true;
    public bool cambiarPorDistancia = false;
    public LayerMask colisiones;

    [Header("Interacción con Jugador")]
    public bool usarEffector = false;
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
        if (effector) effector.enabled = usarEffector;

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