using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class DrBees : MonoBehaviour
{
    float beelocity = 2f;
    Rigidbody2D rb;
    SpriteRenderer sr;
    bool changedDirection = false;
    Vector2 posicionInicial;
    [SerializeField] float distanciaMaxima = 10;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(beelocity, 0);
        sr = GetComponent<SpriteRenderer>();
        posicionInicial = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distanciaRecorrida = Mathf.Abs(transform.position.x - posicionInicial.x);
        if (distanciaRecorrida > distanciaMaxima)
        {
            if (!changedDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
                changedDirection = true;
            }
        }
        else
        {
            if (changedDirection)
            {
                changedDirection = false;
            }
        }
        /*
        int layerMask = 1 << 6;
        RaycastHit2D hitY = Physics2D.Raycast(transform.position, -Vector2.up, 3.0f, layerMask);
        Debug.DrawLine(transform.position, transform.position - Vector3.up * 3.0f, Color.green);
        if (hitY.collider == null)
        {
            if (!changedDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
                changedDirection = true;
            }
        }
        else
        {
            if (changedDirection)
            {
                changedDirection = false;
            }
        }
        */
        /*
        int layerMaskX = 1 << 7;
        layerMaskX = ~layerMaskX;
        RaycastHit2D hitX = Physics2D.Raycast(transform.position, rb.velocity.normalized, 1.0f, layerMaskX);
        Debug.DrawLine(transform.position, transform.position + new Vector3(rb.velocity.normalized.x, rb.velocity.normalized.y, 0) * 1.0f, Color.green);
        if(hitX.collider != null)
        {
            if (!changedDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
                changedDirection = true;
            }
        }
        else
        {
            if (changedDirection)
            {
                changedDirection = false;
            }
        }
        */
        if(rb.velocity.x > 0)
        {
            sr.flipX = true;
        } else if (rb.velocity.x < 0)
        {
            sr.flipX = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Terrain")
        {
            if (!changedDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
                changedDirection = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Terrain")
        {
            if (changedDirection)
            {
                changedDirection = false;
            }
        }
    }
}
