using UnityEngine;
using System.Collections;

public class Generador : Interactuable {
    [Header("Configuración del generador")]
    [Tooltip("Asigna un tiempo de espera del generador")]
    [SerializeField, Min(0)] protected float tiempoEspera = 0.5f;
    [Space(5)]
    [Tooltip("Elige el sistema de funcionamiento")]
    [SerializeField] private bool siempre;
    [Tooltip("Rango de detección")]
    [SerializeField, Min(0)] private float distancia = 0.8f;
    [Space(5)]
    [Tooltip("Permite detectar cercanía o cruce de rayo con el jugador")]
    [SerializeField] private bool cercania;
    [Tooltip("Rango de detección")]
    [SerializeField, Min(0)] private float rango = 5.0f;
    [Space(5)]
    [Tooltip("Permite detectar cercanía o cruce de rayo con el jugador")]
    [SerializeField] private bool fuerza;
    [Tooltip("Agrega un multiplicador de velocidad")]
    [SerializeField, Min(0)] private float velocidad = 5.0f;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Marca el estado actual del interactuable")]
    [SerializeField] private bool enEspera = false;
    [Tooltip("Arrastra el objeto para generar")]
    [SerializeField] private GameObject prefab;

    protected override void Update() {
        //Hacemos un sistema de procesamiento
        if (!enEspera && activo) {
            //Activamos según el tipo de comportamiento
            if (siempre || DetectarJugador()) {
                //Generamos un objeto
                Generar();

                //Ponemos en cooldown el generador
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

    private void Generar() {
        //Generamos nuestro objeto
        GameObject objeto = Instantiate(prefab, transform.position + (distancia * transform.up), transform.rotation);

        //Buscamos su rigidbody si debemos lanzarlo
        if(objeto.TryGetComponent<Rigidbody2D>(out Rigidbody2D rbObjeto) && fuerza) {
            //Le damos un impulso a la bala según atributos
            rbObjeto.AddRelativeForce(Vector2.up * velocidad, ForceMode2D.Impulse);
        }
    }

    private IEnumerator TiempoDeEspera(float espera) {
        //Activar el tiempo de espera
        enEspera = true;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Desactivar el tiempo de espera
        enEspera = false;
    }
    
    private void OnDrawGizmos() {
        //Visualizamos la dirección de salida
        Gizmos.color = Color.red;
        Gizmos.DrawLine((Vector2)transform.position + (distancia * (Vector2)transform.up), (Vector2)transform.position + ((Vector2)transform.up * rango));
    }
}