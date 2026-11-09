
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
        // La plataforma empieza siendo atravesable.
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

        Rigidbody2D playerRb =
            other.attachedRigidbody;

        if (playerRb == null) return;

        // Calculamos la parte superior de la plataforma
        // incluso cuando su collider esta desactivado.
        Vector3 localTop = new Vector3(
            solidCollider.offset.x,
            solidCollider.offset.y +
                solidCollider.size.y / 2f,
            0f
        );

        float platformTop =
            solidCollider.transform
                .TransformPoint(localTop).y;

        // Esperamos hasta que los pies del personaje
        // hayan superado la superficie.
        if (other.bounds.min.y < platformTop)
        {
            return;
        }

        // Evitamos ejecutar la transicion varias veces.
        activated = true;

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

        // MOSTRAR AVISO DE FASE 2
        if (notifications != null)
        {
            notifications.MostrarAvisoFase2();
        }

        Debug.Log("TRANSICION FASE 2 ACTIVADA");
    }
}
