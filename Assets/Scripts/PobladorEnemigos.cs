using System.Collections.Generic;
using UnityEngine;

public class PobladorEnemigos : MonoBehaviour {
    [SerializeField] private GameObject prefabEnemigo;
    [SerializeField] private List<GameObject> puntosGeneracion = new List<GameObject>();
    [SerializeField] private int cantidad = 5;

    private void Start() {
        this.SpawnEnemigos();
    }

    private void Update() {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemigo");
        int numeroEnemigos = enemigos.Length;
        if (numeroEnemigos == 0) {
            this.SpawnEnemigos();
        }
    }

    private void SpawnEnemigos() {
        for (int i = 0; i < this.cantidad; i++) {
            int indiceSpawnPoint = Random.Range(0, this.puntosGeneracion.Count);
            GameObject spawnPoint = this.puntosGeneracion[indiceSpawnPoint];
            Instantiate(this.prefabEnemigo, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }
}