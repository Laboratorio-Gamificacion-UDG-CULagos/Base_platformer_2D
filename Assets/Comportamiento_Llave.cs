using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Comportamiento_Llave : MonoBehaviour
{
    [SerializeField] GameObject puerta;
    //public GameObject puerta;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.name == "Jugador")
        {
            Debug.Log("Entró " + col.name);
            puerta.SetActive(false);
            Destroy(gameObject);
        }
    }
}
