using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cambio_escena : MonoBehaviour
{
    [SerializeField] int sceneId = 0;

    public void CambiarEscena()
    {
        SceneManager.LoadScene(sceneId);
    }
}
