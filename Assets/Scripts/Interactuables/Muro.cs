using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Muro : Interactuable {
    //PENDIENTE AGREGAR CAMBIOS DE SPRITE SEGÚN SU ESTADO
    
    protected override void Update() {
        //Llamamos al update heredado
        base.Update();

        //Animamos el estado del muro
        GetComponent<SpriteRenderer>().enabled = activo;
    }
}