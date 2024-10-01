using UnityEngine;
using System.Collections;

public class Puas : Heridor {
    [Header("Configuración de las púas")]
    [Tooltip("Elige si respeta un angulo personalizado")]
    [SerializeField] private bool anguloPersonalizado;
    [Tooltip("Asigna un ángulo si está activa la personalización")]
    [SerializeField, Range(0, 359)] private int anguloFuerza;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Arrastra el objeto hijo que contiene el sprite")]
    [SerializeField] private Transform sprite;
    [Tooltip("Indica la dirección a la cual disparar")]
    [SerializeField] private Vector2 direccion;

    private void OnValidate() {
        ActualizarDireccion();
    }
    
    protected void Start() {
        ActualizarDireccion();
    }

    private void ActualizarDireccion() {
        //Obtener la rotación Z del objeto y ajustar el ángulo de lanzamiento
        if(!anguloPersonalizado) anguloFuerza = (int)transform.eulerAngles.z % 359;

        //Calcula la dirección de la normal del resorte según el ángulo
        float radianes = (anguloFuerza + 90) * Mathf.Deg2Rad;
        direccion = new Vector2(Mathf.Cos(radianes), Mathf.Sin(radianes));
    }

    protected override IEnumerator TiempoDeEspera(float espera) {
        //Activar el tiempo de espera del resorte
        enEspera = true;

        //Baja su sprite si está asignado
        if (sprite && sprite.localPosition.y <= 0.0f) sprite.position += (Vector3)direccion * 0.4f;

        //Espera el tiempo especificado
        yield return new WaitForSeconds(espera);

        //Desactivar el tiempo de espera del resorte
        enEspera = false;

        //Regresa a la posición original el sprite si existe
        while (sprite && sprite.localPosition.y > 0.0f) {
            sprite.position = Vector2.MoveTowards(sprite.position, transform.position, 0.01f);
            yield return null;
        }
    }

    private void OnDrawGizmos() {
        //Visualizamos la dirección de salida de las púas
        Gizmos.color = Color.red;
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + direccion / 2);
    }
}