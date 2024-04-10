using UnityEngine;
using UnityEngine.SceneManagement;

public class ZonaMuerte : MonoBehaviour {
    [SerializeField] private int IDEscena = 0;
    
    private void OnTriggerEnter2D(Collider2D colisionado) {
        if(colisionado.CompareTag("Jugador")) {
            SceneManager.LoadScene(this.IDEscena);
        }
    }
}