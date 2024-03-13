using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comportamiento_Resorte : MonoBehaviour
{
    SpriteRenderer sr;
    public Sprite[] sprites;
    [SerializeField] float fuerzaResorte = 20f;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Jugador")
        {
            sr.sprite = sprites[1];
            collision.gameObject.GetComponent<ComportamientoPersonaje>().SumarFuerza(Vector2.up, fuerzaResorte);
            StartCoroutine(RegresarIdle());
        }
    }

    IEnumerator RegresarIdle()
    {
        yield return new WaitForSeconds(2f);// Wait for one second

        sr.sprite = sprites[0];
    }
}
