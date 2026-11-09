
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Punto de reaparicion")]
    public Transform respawnPoint;

    [Header("Avisos")]
    public PhaseNotificationUI notifications;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            if (CheckpointManager.Instance != null)
            {
                // Usar el punto de reaparicion elegido
                Vector3 spawnPosition;

                if (respawnPoint != null)
                {
                    spawnPosition = respawnPoint.position;
                }
                else
                {
                    // Mantener comportamiento anterior
                    // si no hay punto asignado
                    spawnPosition =
                        transform.position + Vector3.up * 1.5f;
                }

                CheckpointManager.Instance.ActivateCheckpoint(
                    spawnPosition
                );

                activated = true;

                if (notifications != null)
                {
                    notifications.MostrarAvisoCheckpoint();
                }

                Debug.Log(
                    "CHECKPOINT ACTIVADO: " + gameObject.name + 
                    "| Reaparicion en: " + spawnPosition
                );
            }
        }
    }
}
