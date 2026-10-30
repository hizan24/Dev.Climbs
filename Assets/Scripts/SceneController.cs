using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadNextLevel()
    {
        Debug.Log("BOTÓN CONTINUAR PULSADO");
        SceneManager.LoadScene("Nivel2");
    }
}