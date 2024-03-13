using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    public Sprite impacto;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Nemico")
        {
            Destroy(collision.gameObject);
        }
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = impacto;
        sr.flipX = !sr.flipX;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Destroy(gameObject, 0.1f);
    }
}
