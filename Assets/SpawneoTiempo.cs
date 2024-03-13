using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawneoTiempo : MonoBehaviour
{
    [SerializeField] GameObject enemigo;
    [SerializeField] float intervalo = 3;
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
    float tiempoTranscurrido = 0;
    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(enemigo, transform.position, transform.rotation); 
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(tiempoTranscurrido);
        if(tiempoTranscurrido > intervalo)
        {
            int indiceSpawnPoint = Random.Range(0, spawnPoints.Count);
            GameObject spawnPoint = spawnPoints[indiceSpawnPoint];
            Instantiate(enemigo, spawnPoint.transform.position, spawnPoint.transform.rotation);
            tiempoTranscurrido = 0; 
        }
        tiempoTranscurrido += Time.deltaTime;
    }
}
