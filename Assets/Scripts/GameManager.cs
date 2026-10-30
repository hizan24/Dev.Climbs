
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Progreso")]
    [Range(1, 4)]
    public int currentPhase = 1;

    [Header("Desbloqueos")]
    public bool jetpackUnlocked = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void UnlockJetpack()
    {
        jetpackUnlocked = true;
        Debug.Log("¡Jetpack desbloqueado!");
    }

    public void SetPhase(int phase)
    {
        currentPhase = Mathf.Clamp(phase, 1, 4);
        Debug.Log("Fase actual: " + currentPhase);
    }
}

