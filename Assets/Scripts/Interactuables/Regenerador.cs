using UnityEngine;

public class Regenerador : Objeto {
    protected override void OnTriggerEnter2D(Collider2D colisionado) {
        //Llamamos al método del que hereda
        base.OnTriggerEnter2D (colisionado);

        //Detecta colisiones con el jugador
        if (colisionado.CompareTag("Jugador")) {
            //Acutalizamos sla vida del jugador
            colisionado.GetComponent<Personaje>().vida += valor;
        }
    }
}