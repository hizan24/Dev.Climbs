
using UnityEngine;

public class TransitionPlatform : MonoBehaviour
{
    [Header("Colliders")]
    public BoxCollider2D triggerCollider;
    public BoxCollider2D solidCollider;

    [Header("Avisos")]
    public PhaseNotificationUI notifications;

    private bool activated = false;

    private void Start()
    {
        solidCollider.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        Rigidbody2D playerRb =
            other.GetComponent<Rigidbody2D>();

        if (playerRb == null) return;
        if (playerRb.velocity.y <= 0f) return;

        if (other.bounds.min.y >= solidCollider.bounds.max.y)
        {
            // MEJORA DE SALTO DE FASE 2
            PlayerMovement playerMovement =
                other.GetComponent<PlayerMovement>();

            if (playerMovement != null)
            {
                playerMovement.ActivatePhase2JumpUpgrade();
            }

            // EVOLUCION VISUAL DEL PERSONAJE
            CharacterPhaseVisuals characterVisuals =
                other.GetComponent<CharacterPhaseVisuals>();

            if (characterVisuals != null)
            {
                characterVisuals.ApplyPhase2Visuals();
            }

            // ACTIVAR PLATAFORMA SOLIDA
            solidCollider.enabled = true;

            activated = true;

            // MOSTRAR AVISO DE FASE 2
            if (notifications != null)
            {
                notifications.MostrarAvisoFase2();
            }

            Debug.Log("TRANSICION FASE 2 ACTIVADA");
        }
    }
}
