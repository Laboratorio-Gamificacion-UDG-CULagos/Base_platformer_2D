using System.Collections;
using UnityEngine;

public class Resorte : MonoBehaviour {
    [SerializeField] private Sprite[] spritesArray;
    [SerializeField] private float fuerzaResorte = 20.0f;

    private void OnTriggerEnter2D(Collider2D colisionado) {
        if (colisionado.gameObject.CompareTag("Jugador")) {
            this.GetComponent<SpriteRenderer>().sprite = spritesArray[1];
            //colisionado.gameObject.GetComponent<ComportamientoPersonaje>().SumarFuerza(Vector2.up, fuerzaResorte);
            StartCoroutine(RegresarIdle());
        }
    }

    private IEnumerator RegresarIdle() {
        yield return new WaitForSeconds(1.5f);
        this.GetComponent<SpriteRenderer>().sprite = spritesArray[0];
    }
}