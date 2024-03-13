using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportamientoPuas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name == "Jugador")
        {
            col.gameObject.GetComponent<ComportamientoPersonaje>().Dano();
        }
    }
}
