using System;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Personaje : MonoBehaviour {
    [SerializeField] private int vida = 6;
    [SerializeField] private float fuerzaMovimiento = 0.9f;
    [SerializeField] private float largoRaycast = 1.05f;
    [SerializeField] private float velocidadBala = 20.0f;
    [SerializeField] private float velocidadMaxima = 15.0f;
    [SerializeField] private float fuerzaSalto = 10.0f;
    [SerializeField] private float resistencia = 0.85f;
    [SerializeField] private float beelX = 0.0f;
    [SerializeField] private float inputX = 0.0f;
    [SerializeField] private float airTime = 0.0f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator ani;
    [SerializeField] private RectTransform hud;
    [SerializeField] private GameObject prefabBala;

    private void Update() {
        this.inputX = Input.GetAxis("Horizontal");
        this.beelX = Mathf.Sqrt(this.rb.velocity.x * this.rb.velocity.x);
        this.ani.SetFloat("influenciaMovimiento", this.beelX);
        this.ani.speed = (this.rb.velocity.magnitude / this.velocidadMaxima * 0.5f) + 0.5f;
        
        if (this.rb.velocity.x < -0.15f){
            this.sr.flipX = true;
        } else if (this.rb.velocity.x > 0.15f) {
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

        //Debug.DrawRay(this.transform.position, this.rb.velocity, Color.red); //?

        //DEBUG
        if (Input.GetKeyDown(KeyCode.Space)) {
            //Debug.Log(hit1.collider.gameObject.name); //?
        };
    }

    private void FixedUpdate() {
        bool parado = DetectarPlataforma();
        this.airTime = parado ? 0.0f : this.airTime + Time.deltaTime;
        this.ani.SetBool("saltando", this.airTime >= this.coyoteTime);
        if (Input.GetAxis("Vertical") > 0.05f && !this.ani.GetBool("saltando")) {
            this.rb.velocity = new Vector2(this.rb.velocity.x, this.fuerzaSalto);
            //this.rb.AddForce(salto, ForceMode2D.Impulse);
        }

        if (this.rb.velocity.magnitude < this.velocidadMaxima) {
            float movimientoHorizontal = parado ? this.inputX * this.fuerzaMovimiento : this.inputX * this.fuerzaMovimiento / 10.0f;
            Vector2 movTotal;
            if (parado) {
                movTotal = new Vector2(movimientoHorizontal + (this.rb.velocity.x * this.resistencia), this.rb.velocity.y);
            } else {
                movimientoHorizontal *= 0.3f;
                movTotal = new Vector2(movimientoHorizontal + this.rb.velocity.x, this.rb.velocity.y);
            }
            this.rb.velocity = movTotal;
            //this.rb.AddForce(movimiento);

            if (!parado && this.rb.velocity.y < 0.0f) {
                if ((this.rb.velocity.x < 0.0f && DetectarPared(-1)) || (this.rb.velocity.x > 0.0f && DetectarPared(1))) {
                    this.rb.velocity = new Vector3(0.0f, this.rb.velocity.y / 2.0f);
                }
            }
        }
    }

    private bool DetectarPared(int direccion) {
        //Physics2D.queriesHitTriggers = false; //?
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector2.right * direccion, 0.3f);
        //Physics2D.queriesHitTriggers = true; //?
        Debug.DrawRay(this.transform.position, Vector2.right * direccion * 0.3f, Color.red); //?
        return hit.collider != null && hit.collider.gameObject.CompareTag("Plataforma");
    }

    private bool DetectarPlataforma() {
        Vector3 offsetRayo = Vector3.left / 5;
        Vector3 posRayo1 = this.transform.position - offsetRayo;
        Vector3 posRayo2 = this.transform.position + offsetRayo;

        bool tocaIzq = DetectarIzq(posRayo1);
        bool tocaDer = DetectarDer(posRayo2);

        return tocaIzq || tocaDer;
    }

    private bool DetectarIzq(Vector3 posRayo) {
        //Physics2D.queriesHitTriggers = false; //?
        RaycastHit2D hit = Physics2D.Raycast(posRayo, Vector2.down, this.largoRaycast);
        //Physics2D.queriesHitTriggers = true; //?
        Debug.DrawRay(posRayo, Vector2.down * this.largoRaycast, Color.blue); //?
        return hit.collider != null && hit.collider.gameObject.CompareTag("Plataforma");
    }

    private bool DetectarDer(Vector3 posRayo) {
        //Physics2D.queriesHitTriggers = false; //?
        RaycastHit2D hit = Physics2D.Raycast(posRayo, Vector2.down, this.largoRaycast);
        //Physics2D.queriesHitTriggers = true; //?
        Debug.DrawRay(posRayo, Vector2.down * this.largoRaycast, Color.blue); //?
        return hit.collider != null && hit.collider.gameObject.CompareTag("Plataforma");
    }

    public void Herir() {
        if (this.vida > 1) {
            this.vida--;
            this.hud.sizeDelta = new Vector2(50.0f * this.vida, 100.0f);
        }
        else {
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