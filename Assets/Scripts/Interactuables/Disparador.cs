using UnityEngine;
using System.Collections;

public class Disparador : Interactuable {
    [Header("Configuración del disparador")]
    [Tooltip("Asigna un tiempo de espera del cactus")]
    [SerializeField, Min(0)] protected float tiempoEspera = 0.5f;
    [Space(5)]
    [Tooltip("Elige el sistema de funcionamiento")]
    [SerializeField] private bool siempre;
    [Tooltip("Agrega un multiplicador de velocidad")]
    [SerializeField, Min(0)] private float fuerza = 5.0f;
    [Space(5)]
    [Tooltip("Permite detectar cercanía o cruce de rayo con el jugador")]
    [SerializeField] private bool cercania;
    [Tooltip("Rango de detección")]
    [SerializeField, Min(0)] private float rango = 5.0f;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Marca el estado actual del interactuable")]
    [SerializeField] private bool enEspera = false;
    [Tooltip("Arrastra el tipo de proyectil")]
    [SerializeField] private GameObject proyectil;

    protected override void Update() {
        //Hacemos un sistema de procesamiento
        if (!enEspera && activo) {
            //Activamos según el tipo de comportamiento
            if (siempre || DetectarJugador()) {
                //Disparamos un proyectil
                Disparar();

                //Ponemos en cooldown el disparador
                StartCoroutine(TiempoDeEspera(tiempoEspera));
            }
        }
    }

    private bool DetectarJugador() {
        //Actuamos por cercanía o haciendo raycast
        if (cercania) {
            //Si está cerca dispara
            return BuscarJugador();
        } else {
            //Si está en la mira dispara
            return MirarJugador();
        }
    }

    private bool MirarJugador() {
        //Lanzamos un rayo
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, rango);

        //Devolvemos si el rayo colisionó con un jugador
        return hit.collider && hit.collider.CompareTag("Jugador");
    }

    private bool BuscarJugador() {
        //Referenciamos al jugador temporalmente
        Transform jugador = GameObject.FindGameObjectWithTag("Jugador").transform;

        //Respondemos si está cerca
        return Vector2.Distance(transform.position, jugador.position) <= rango;
    }

    private void Disparar() {
        //Generamos nuestro proyectil
        GameObject bala = Instantiate(proyectil, transform.position + (0.8f * transform.up), transform.rotation);
        if(bala.TryGetComponent<Rigidbody2D>(out Rigidbody2D rbBala)) {
            //Le damos un impulso a la bala según atributos
            rbBala.AddRelativeForce(Vector2.up * fuerza, ForceMode2D.Impulse);
        }
    }

    private IEnumerator TiempoDeEspera(float espera) {
        //Activar el tiempo de espera del resorte
        enEspera = true;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Desactivar el tiempo de espera del resorte
        enEspera = false;
    }
    
    private void OnDrawGizmos() {
        //Visualizamos la dirección de salida del proyectil
        Gizmos.color = Color.red;
        Gizmos.DrawLine((Vector2)transform.position + (0.8f * (Vector2)transform.up), (Vector2)transform.position + ((Vector2)transform.up * rango));
    }
}