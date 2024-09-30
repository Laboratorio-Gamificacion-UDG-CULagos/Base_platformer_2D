using UnityEngine;

public class Puas : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D colisionado) {
        if (colisionado.gameObject.name == "Jugador") {
            colisionado.gameObject.GetComponent<Personaje>().Herir();
        }
    }
}