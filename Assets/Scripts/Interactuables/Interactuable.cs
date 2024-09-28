using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Interactuable : MonoBehaviour {
    [Header("INTERACCIONES")]
    [Tooltip("Define si el objeto funciona")]
    public bool activo = true;

    protected virtual void Update() {
        GetComponent<Collider2D>().enabled = activo;
    }
}