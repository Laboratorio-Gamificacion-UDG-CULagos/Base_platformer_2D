using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Collider2D))]
public class Bloque : Interactuable {
    [Header("Configuración del resorte")]
    [Space(5)]
    [Tooltip("Establece si es afectado por gravedad")]
    [SerializeField] private bool dinamico = false;
    [Tooltip("Establece si el objeto se puede romper")]
    [SerializeField] private bool destructible = true;
    [Space(5)]
    [Tooltip("Establece si el objeto se regenera")]
    [SerializeField] private bool regenerativo = false;
    [Tooltip("Asigna un tiempo de regeneración")]
    [SerializeField, Min(0)] private float tiempoSpawn;
    [Space(5)]
    [Tooltip("Elige si desencadena acciones")]
    [SerializeField] private bool desencadenador = false;
    [Tooltip("Elige el tipo de desencadenamiento (simple|avanzado)")]
    [SerializeField] private bool simple = false;
    [Tooltip("Elige el tipo de estado a desencadenar (avanzado)")]
    [SerializeField] private bool estado = false;
    [Tooltip("Arrastra interactuables objetos para habilitarlos")]
    [SerializeField] private Interactuable[] acciones;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Referencía al Rigidbody2D del objeto")]
    [SerializeField] private Rigidbody2D rb;
    [Tooltip("Referencía al SpriteRenderer del objeto")]
    [SerializeField] private SpriteRenderer sp;
    [Tooltip("Indica la posición aparición del objeto")]
    [SerializeField] private Vector2 posicionInicial;
    [Tooltip("Marca el estado actual del interactuable")]
    [SerializeField] private bool enEspera = false;

    private void Awake() {
        //Obtenemos los parámetros iniciales
        if (!sp) sp = GetComponent<SpriteRenderer>();
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        //Establecemos comportamiento por defecto
        if (dinamico) rb.bodyType = RigidbodyType2D.Dynamic;
        else rb.bodyType = RigidbodyType2D.Static;
        
        //Guardamos la posición inicial
        posicionInicial = transform.position;
    }

    protected override void Update() {
        //Llamamos al método del que hereda
        base.Update();

        //Actualizamos su estado visual
        if (enEspera || !activo) sp.enabled = false;
        else if (!enEspera) sp.enabled = true;
    }

    private void Accionar(bool tipo) {
        //Activamos los objetos que son interactuables
        if (acciones.Length > 0 && desencadenador) {
            //Se itera en cada objeto asignado, siendo que hay mínimo uno
            for (int i = 0; i < acciones.Length; i++) {
                //Buscamos si son objetos interactuables
                if (acciones[i]) {
                    //Invertimos su estado
                    if (simple) acciones[i].activo = !acciones[i].activo;
                    else acciones[i].activo = estado;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D colisionado) {
        //Buscamos colisiones con proyectiles
        if (destructible && activo && !enEspera && colisionado.gameObject.CompareTag("Proyectil")) {
            //Destruímos o desaparecemos temporalmente
            if (regenerativo) StartCoroutine(TiempoDeSpawn(tiempoSpawn));
            else Destroy(gameObject);
        } else rb.velocity = Vector2.zero;
    }
    
    private IEnumerator TiempoDeSpawn(float espera) {
        //Activar el tiempo de espera
        enEspera = true;
        Accionar(simple);

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        transform.position = posicionInicial;
        //Desactivar el tiempo de espera
        enEspera = false;
    }
}