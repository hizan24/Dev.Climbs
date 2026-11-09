
using UnityEngine;

public class TransitionPlatformPhase3 : MonoBehaviour
{
    [Header("Colliders")]
    public BoxCollider2D triggerCollider;
    public BoxCollider2D solidCollider;

    [Header("Checkpoint Fase 3")]
    public Transform respawnPoint;

    [Header("Avisos")]
    public PhaseNotificationUI notifications;

    private bool activated = false;

    private void Start()
    {
        if (solidCollider != null)
        {
            solidCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryActivateTransition(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryActivateTransition(other);
    }

    private void TryActivateTransition(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;
        if (solidCollider == null) return;

        Rigidbody2D playerRb = other.attachedRigidbody;

        if (playerRb == null) return;

        Vector3 localTop = new Vector3(
            solidCollider.offset.x,
            solidCollider.offset.y +
            solidCollider.size.y / 2f,
            0f
        );

        float platformTop = solidCollider.transform
            .TransformPoint(localTop).y;

        if (other.bounds.min.y < platformTop)
        {
            return;
        }

        activated = true;

        // ACTIVAR PLATAFORMA SOLIDA
        solidCollider.enabled = true;

        // MEJORA DE SALTO FASE 3
        PlayerMovement playerMovement =
            other.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.ActivatePhase3JumpUpgrade();
        }

        // NUEVO: ACTIVAR APARIENCIA FASE 3
        CharacterPhaseVisuals characterVisuals =
            playerRb.GetComponent<CharacterPhaseVisuals>();

        if (characterVisuals != null)
        {
            characterVisuals.ApplyPhase3Visuals();

            Debug.Log(
                "PERSONAJE FASE 3: NUEVO ASPECTO ACTIVADO"
            );
        }
        else
        {
            Debug.LogWarning(
                "No se encontro CharacterPhaseVisuals en Player"
            );
        }

        // ACTIVAR CHECKPOINT
        bool checkpointActivated = false;

        if (CheckpointManager.Instance != null &&
            respawnPoint != null)
        {
            CheckpointManager.Instance.ActivateCheckpoint(
                respawnPoint.position
            );

            checkpointActivated = true;

            Debug.Log("CHECKPOINT FASE 3 ACTIVADO");
        }
        else
        {
            Debug.LogWarning(
                "No se pudo activar el Checkpoint Fase 3"
            );
        }

        // MOSTRAR AMBOS AVISOS
        if (notifications != null)
        {
            notifications.MostrarAvisoFase3();

            if (checkpointActivated)
            {
                notifications.MostrarAvisoCheckpoint();
            }
        }

        Debug.Log("TRANSICION FASE 3 ACTIVADA");
    }
}
