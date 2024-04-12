using UnityEngine;

public class Personaje : MonoBehaviour {
    [SerializeField] private int vida = 6;
    [SerializeField] private float fuerzaMovimiento = 20.0f;
    [SerializeField] private float largoRaycast = 1.05f;
    [SerializeField] private float velocidadBala = 20.0f;
    [SerializeField] private float velocidadMaxima = 6.0f;
    [SerializeField] private float fuerzaSalto = 8.0f;
    [SerializeField] private float resistencia = 1.0f;
    [SerializeField] private float beelX;
    [SerializeField] private float inputX;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator ani;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject prefabBala;

    private void Update() {
        this.inputX = Input.GetAxis("Horizontal");
        this.beelX = Mathf.Sqrt(this.rb.velocity.x * this.rb.velocity.x);
        this.ani.SetFloat("influenciaMovimiento", this.beelX);
        this.ani.speed = (this.rb.velocity.magnitude / this.velocidadMaxima * 0.5f) + 0.5f;
        
        if (this.rb.velocity.x < -0.1f){
            this.sr.flipX = true;
        } else if (this.rb.velocity.x > 0.1f) {
            this.sr.flipX = false;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            Vector3 posicionDisparo = new Vector3(this.transform.localPosition.x + (this.sr.flipX ? -1 : 1), this.transform.localPosition.y - 0.3f, this.transform.localPosition.z);
            GameObject proyectil = Instantiate(this.prefabBala, posicionDisparo, this.transform.rotation);
            /*if (this.sr.flipX) {
                posicionBala.x = posicionBala.x - 1.0f;
                proyectil.GetComponent<Rigidbody2D>().velocity = new Vector2(this.rb.velocity.x - this.velocidadBala, 0.0f);
            } else {
                posicionBala.x = posicionBala.x + 1.0f;
            }*/
            //posicionDisparo.x = posicionDisparo.x - ((this.rb.velocity.x + 1) / (1 + Mathf.Sqrt(this.rb.velocity.x * this.rb.velocity.x)));
            proyectil.GetComponent<Rigidbody2D>().velocity = new Vector2(this.velocidadBala * (this.sr.flipX ? -1 : 1), 0.0f);
            proyectil.GetComponent<SpriteRenderer>().flipX = this.sr.flipX;
        }
    }

    private void FixedUpdate() {
        //Physics2D.queriesHitTriggers = false; //?
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector2.down * this.rb.gravityScale, this.largoRaycast);
        //Physics2D.queriesHitTriggers = true; //?
        bool parado = hit.collider != null && hit.collider.gameObject.CompareTag("Plataforma");
        this.ani.SetBool("saltando", !parado);
        if (this.rb.velocity.magnitude < this.velocidadMaxima) {
            float movimientoHorizontal = parado ? this.inputX * this.fuerzaMovimiento : 0.0f;
            if (!parado) {
                movimientoHorizontal *= 0.3f;
            }
            Vector2 movTotal = Vector2.zero;
            if (parado) {
                movTotal = new Vector2(movimientoHorizontal + (this.rb.velocity.x * this.resistencia), this.rb.velocity.y);
            } else {
                movTotal = new Vector2(movimientoHorizontal + this.rb.velocity.x, this.rb.velocity.y);
            }
            this.rb.velocity = movTotal;
            //this.rb.AddForce(movimiento);
        }
    
        if (Input.GetAxis("Vertical") > 0.05f && parado) {
            Vector2 salto = Vector2.up * this.fuerzaSalto;

            this.rb.velocity = new Vector2(this.rb.velocity.x, this.fuerzaSalto);
            //this.rb.AddForce(salto, ForceMode2D.Impulse);
        }

        //DEBUG
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log(hit.collider.gameObject.name); //?
        };
        Debug.DrawRay(this.transform.position, -Vector2.up * this.largoRaycast, Color.blue); //?
        Debug.DrawRay(this.transform.position, this.rb.velocity, Color.red); //?
    }

    public void Herir() {
        if (this.vida > 1) {
            this.vida--;
            GameObject vidaHUD = this.hud.transform.gameObject;
            //hud.GetComponent<RectTransform>().sizeDelta = new Vector2(50.0f * this.vida, 100.0f);
        } else {
            this.GetComponent<ControlEscenas>().CambiarEscena();
        }
    }

    public void Mover(Vector2 direccion, float fuerza) {
        direccion *= fuerza;
        this.rb.AddForce(direccion, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D colisionado) {
        if(colisionado.gameObject.CompareTag("Enemigo")) {
            this.Herir();
        }
    }
}