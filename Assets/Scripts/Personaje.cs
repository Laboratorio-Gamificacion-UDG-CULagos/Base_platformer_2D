using UnityEngine;

public class Personaje : MonoBehaviour {
    [SerializeField] private int vida = 6;
    [SerializeField] private float fuerzaMovimiento = 20.0f;
    [SerializeField] private float largoRaycast = 1.05f;
    [SerializeField] private float velocidadBala = 20.0f;
    [SerializeField] private float velocidadMaxima = 6.0f;
    [SerializeField] private float fuerzaSalto = 8.0f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator ani;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject prefabBala;

    private void Update() {
        float movX = Input.GetAxis("Horizontal");
        this.ani.SetFloat("influenciaMovimiento", Mathf.Abs(movX));
        this.ani.speed = ((this.rb.velocity.magnitude / this.velocidadMaxima) * 0.5f) + 0.5f;
        
        if (movX < -0.1f){
            this.sr.flipX = true;
        } else if (movX > 0.1f) {
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
        Physics2D.queriesHitTriggers = false; //?
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector2.up * -1.0f * this.rb.gravityScale, this.largoRaycast);
        Physics2D.queriesHitTriggers = true; //?
        bool parado = hit.collider != null && hit.collider.gameObject.CompareTag("Plataforma");
        this.ani.SetBool("saltando", !parado);

        if (this.rb.velocity.magnitude < this.velocidadMaxima) {
            float movimientoHorizontal = Input.GetAxis("Horizontal") * this.fuerzaMovimiento;
            if (!parado) {
                movimientoHorizontal *= 0.2f;
            }
            Vector2 movimiento = new Vector2(movimientoHorizontal, 0.0f);
            this.rb.AddForce(movimiento);
        }
    
        if (Input.GetAxis("Vertical") > 0.2f && parado) {
            Vector2 salto = Vector2.up * this.fuerzaSalto;
            this.rb.AddForce(salto, ForceMode2D.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log(hit.collider.gameObject.name); //?
        };
        Debug.DrawRay(this.transform.position, -Vector2.up * this.largoRaycast, Color.blue); //?
        Debug.DrawRay(this.transform.position, this.rb.velocity, Color.red); //?
    }

    public void Herir() {
        if (this.vida > 1) {
            this.vida--;
            GameObject vidaHUD = this.hud.transform.GetChild(1).gameObject;
            vidaHUD.GetComponent<RectTransform>().sizeDelta = new Vector2(50.0f * this.vida, 100.0f);
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