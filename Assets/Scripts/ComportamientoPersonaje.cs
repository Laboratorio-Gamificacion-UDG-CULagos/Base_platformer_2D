using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportamientoPersonaje : MonoBehaviour
{
    // vidas y puntos
    int vidas = 6; //cada vida es medio corazón
    
    // variables de movimiento
    public float velocidadMaxima = 6;
    float fuerzaMovimiento = 20;
    public float fuerzaSalto = 8;
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    float raycastLength = 1.05f;
    
    // variables bala
    public GameObject bala;
    float velocidadBala = 20f;

    GameObject hud;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        hud = GameObject.Find("HUD");
    }

    // Update is called once per frame
    void Update()
    {
        float Xmov = Input.GetAxis("Horizontal");
        anim.SetFloat("mov_influence", Mathf.Abs(Xmov));
        anim.speed = ((rb.velocity.magnitude / velocidadMaxima) * 0.5f) + 0.5f;
        
        if (Xmov < -0.1)
        {
            sr.flipX = true;
        }
        else if (Xmov > 0.1)
        {
            sr.flipX = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 posBala = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            if (sr.flipX)
            {
                posBala.x = posBala.x - 1f;
                GameObject disparo = Instantiate(bala, posBala, transform.rotation);
                disparo.GetComponent<Rigidbody2D>().velocity = new Vector2(rb.velocity.x - velocidadBala, 0);
                disparo.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                posBala.x = posBala.x + 1f;
                GameObject disparo = Instantiate(bala, posBala, transform.rotation);
                disparo.GetComponent<Rigidbody2D>().velocity = new Vector2(rb.velocity.x + velocidadBala, 0);
            }
        }
    }
    void FixedUpdate()
    {
        Physics2D.queriesHitTriggers = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up * -1.0f * rb.gravityScale, raycastLength);
        Physics2D.queriesHitTriggers = true;
        bool enPiso = hit.collider != null && hit.collider.gameObject.tag == "Terrain";
        anim.SetBool("saltando", !enPiso);

        if (rb.velocity.magnitude < velocidadMaxima)
        {
            float movimientoHorizontal = Input.GetAxis("Horizontal") * fuerzaMovimiento;
            if (!enPiso)
            {
                movimientoHorizontal *= 0.2f;
            }
            Vector2 movimiento = new Vector2(movimientoHorizontal, 0);
            rb.AddForce(movimiento);
        }
    
        if (Input.GetAxis("Vertical") > 0.2 && enPiso)
        {
            Vector2 salto = new Vector2(0 , fuerzaSalto);
            rb.AddForce(salto, ForceMode2D.Impulse);
            //Debug.Log(hit.collider.gameObject.tag);
        }
        if (Input.GetKeyDown(KeyCode.Space)) { Debug.Log(hit.collider.gameObject.name); };
        Debug.DrawRay(transform.position, -Vector2.up * raycastLength, Color.blue);
        Debug.DrawRay(transform.position, rb.velocity, Color.red);
    }

    public void VidaBaja()
    {
        if (vidas > 1)
        {
            vidas--;
            GameObject vida = hud.transform.GetChild(1).gameObject;
            vida.GetComponent<RectTransform>().sizeDelta = new Vector2(50 * vidas, 100);
        }
        else
        {
            GetComponent<Cambio_escena>().CambiarEscena();
        }
    }

    public void Dano()
    {
        VidaBaja();
        //rb.AddForce(rb.velocity * -10f);
    }

    public void SumarFuerza(Vector2 direccion, float fuerza)
    {
        direccion *= fuerza;
        rb.AddForce(direccion, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Nemico")
        {
            Dano();
        }
    }
}
