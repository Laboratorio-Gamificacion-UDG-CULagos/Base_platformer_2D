using UnityEngine;

public class DrsBees : MonoBehaviour {
    [SerializeField] private float beelocity = 2.0f;
    [SerializeField] private bool miraDerecha = false;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Vector2 posicionInicial;
    [SerializeField] private float distanciaMaxima = 10.0f;

    private void Start() {
        this.rb = this.GetComponent<Rigidbody2D>();
        this.rb.velocity = new Vector2(this.beelocity, 0.0f);
        this.sr = this.GetComponent<SpriteRenderer>();
        this.posicionInicial = this.transform.position;
    }

    private void Update() {
        float distanciaRecorrida = Mathf.Abs(this.transform.position.x - this.posicionInicial.x);
        if (distanciaRecorrida > this.distanciaMaxima) {
            this.rb.velocity = new Vector2(this.rb.velocity.x * -1.0f, this.rb.velocity.y);
            this.sr.flipX ^= true;
        }
    }

    private void OnCollisionEnter2D(Collision2D colisionado) {
        if(colisionado.gameObject.CompareTag("Plataforma")) {
            if (!this.miraDerecha) {
                this.rb.velocity = new Vector2(this.rb.velocity.x * -1, this.rb.velocity.y);
                this.miraDerecha = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D colisionado) {
        if(colisionado.gameObject.CompareTag("Plataforma")) {
            if (this.miraDerecha) {
                this.miraDerecha = false;
            }
        }
    }
}