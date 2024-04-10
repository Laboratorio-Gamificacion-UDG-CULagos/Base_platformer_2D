using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlEscenas : MonoBehaviour {
    [SerializeField] int IDEscena = 0;

    public void CambiarEscena() {
        SceneManager.LoadScene(IDEscena);
    }
}