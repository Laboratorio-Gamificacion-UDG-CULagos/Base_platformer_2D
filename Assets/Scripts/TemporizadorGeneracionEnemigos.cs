using System.Collections.Generic;
using UnityEngine;

public class TemporizadorGeneracionEnemigos : MonoBehaviour {
    [SerializeField] private GameObject prefabEnemigo;
    [SerializeField] private float intervalo = 3.0f;
    [SerializeField] private List<GameObject> puntosGeneracion = new List<GameObject>();
    private float tiempoTranscurrido = 0.0f;

    private void Update() {
        if(this.tiempoTranscurrido > this.intervalo) {
            int indiceSpawnPoint = Random.Range(0, this.puntosGeneracion.Count);
            GameObject spawnPoint = this.puntosGeneracion[indiceSpawnPoint];
            Instantiate(this.prefabEnemigo, spawnPoint.transform.position, spawnPoint.transform.rotation);
            this.tiempoTranscurrido = 0.0f; 
        }
        this.tiempoTranscurrido += Time.deltaTime;
    }
}