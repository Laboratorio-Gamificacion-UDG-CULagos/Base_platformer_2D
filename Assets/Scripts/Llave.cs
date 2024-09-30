using UnityEngine;

public class Llave : MonoBehaviour {
    [SerializeField] private GameObject prefabPuerta;

    private void OnTriggerEnter2D(Collider2D colisionado) {
        if(colisionado.CompareTag("Jugador")) {
            this.prefabPuerta.SetActive(false);
            Destroy(gameObject, 0.05f);
        }
    }
}