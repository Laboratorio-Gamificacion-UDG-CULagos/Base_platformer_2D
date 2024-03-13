using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawneoCantidad : MonoBehaviour
{
    [SerializeField] GameObject enemigo;
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] int cantidadMinima = 5;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < cantidadMinima; i++)
        {
            int indiceSpawnPoint = Random.Range(0, spawnPoints.Count);
            GameObject spawnPoint = spawnPoints[indiceSpawnPoint];
            Instantiate(enemigo, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Nemico");
        int numeroEnemigos = enemigos.Length;
        if (numeroEnemigos == 0)
        {
            for (int i = 0; i < cantidadMinima; i++)
            {
                int indiceSpawnPoint = Random.Range(0, spawnPoints.Count);
                GameObject spawnPoint = spawnPoints[indiceSpawnPoint];
                Instantiate(enemigo, spawnPoint.transform.position, spawnPoint.transform.rotation);
            }
        }
    }
}
