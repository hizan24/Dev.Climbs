using UnityEngine;

public class JetpackCollectible : MonoBehaviour
{
    [Header("Avisos")]
    public PhaseNotificationUI notifications;

    [Header("Generador de objetos")]
    public FallingObjectSpawnerPhase3 objectSpawner;
    public float pauseSpawnSeconds = 4f;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        PlayerMovement player =
            other.GetComponent<PlayerMovement>();

        if (player == null) return;

        collected = true;

        player.UnlockJetpack();

        if (notifications != null)
        {
            notifications.MostrarAvisoJetpack();
        }

        if (objectSpawner == null)
        {
            objectSpawner =
                FindObjectOfType<FallingObjectSpawnerPhase3>();
        }

        if (objectSpawner != null)
        {
            objectSpawner.PauseSpawning(pauseSpawnSeconds);
        }

        Debug.Log("JETPACK DESBLOQUEADO");

        gameObject.SetActive(false);
        Destroy(gameObject, 0.05f);
    }
}