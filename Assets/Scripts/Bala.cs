using UnityEngine;

public class Bala : MonoBehaviour {
    public Sprite spriteImpacto;
    private void OnCollisionEnter2D(Collision2D colisionado) {
        if(colisionado.gameObject.tag == "Enemigo") {
            Destroy(colisionado.gameObject);
        }
        this.GetComponent<SpriteRenderer>().sprite = this.spriteImpacto;
        this.GetComponent<SpriteRenderer>().flipX = !this.GetComponent<SpriteRenderer>().flipX;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Destroy(this.gameObject, 0.1f);
    }
}