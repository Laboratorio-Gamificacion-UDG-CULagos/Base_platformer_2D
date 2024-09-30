using UnityEngine;

public class Interactuable : MonoBehaviour {
    [Header("INTERACCIONES")]
    [Tooltip("Define si el objeto funciona")]
    public bool activo = true;

    protected virtual void Update() {
        if (TryGetComponent<Collider2D>(out Collider2D collider)) collider.enabled = activo;
        if (TryGetComponent<Light>(out Light light)) light.enabled = activo;
    }
}