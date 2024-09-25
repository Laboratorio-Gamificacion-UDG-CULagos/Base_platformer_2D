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
    public bool cambiarPorColision = true;

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
        if (esEstatica) MoverPlataforma();

        //Actualizar funcion del usuario
        if (effector) effector.enabled = usarEffector;
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

    private void OnCollisionEnter2D(Collision2D collision) {
        //Cambiar direccion al chocar con otras plataformas
        if (cambiarPorColision && collision.gameObject.CompareTag("Plataforma")) {
            CambiarDireccion();
        }
    }
}