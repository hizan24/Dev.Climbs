
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Seguimiento normal - Fases 1 y 2")]
    public Transform target;
    public float smoothSpeed = 5f;
    public float verticalOffset = 0f;
    public float minY = 0f;

    [Header("Encuadre Fase 3")]
    public float phase3NormalOffset = 3f;
    public float phase3FallingOffset = -1.5f;

    [Header("Suavidad Fase 3")]
    public float phase3FollowSmoothTime = 0.12f;
    public float fallingTransitionTime = 0.30f;
    public float returnUpTransitionTime = 0.55f;

    private CharacterPhaseVisuals characterVisuals;
    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private Rigidbody2D playerRb;

    private Transform cachedTarget;

    private float currentVerticalOffset;
    private float offsetVelocity;
    private float cameraVelocityY;

    private void Awake()
    {
        currentVerticalOffset = verticalOffset;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // OBTENER REFERENCIAS DEL JUGADOR
        if (cachedTarget != target)
        {
            cachedTarget = target;

            characterVisuals =
                target.GetComponent<CharacterPhaseVisuals>();

            playerMovement =
                target.GetComponent<PlayerMovement>();

            playerAnimator =
                target.GetComponent<Animator>();

            if (playerAnimator == null)
            {
                playerAnimator =
                    target.GetComponentInChildren<Animator>();
            }

            playerRb =
                target.GetComponent<Rigidbody2D>();
        }

        // DETECTAR FASE 3
        bool phase3Active =
            characterVisuals != null &&
            playerAnimator != null &&
            characterVisuals.phase3Controller != null &&
            playerAnimator.runtimeAnimatorController ==
                characterVisuals.phase3Controller;

        // FASES 1 Y 2:
        // CONSERVAR EL SEGUIMIENTO ORIGINAL
        if (!phase3Active)
        {
            currentVerticalOffset = verticalOffset;
            offsetVelocity = 0f;
            cameraVelocityY = 0f;

            float normalTargetY = Mathf.Max(
                target.position.y + verticalOffset,
                minY
            );

            Vector3 normalTargetPosition = new Vector3(
                transform.position.x,
                normalTargetY,
                transform.position.z
            );

            transform.position = Vector3.Lerp(
                transform.position,
                normalTargetPosition,
                smoothSpeed * Time.deltaTime
            );

            return;
        }

        // FASE 3:
        // MIRAR HACIA ARRIBA POR DEFECTO
        float desiredOffset = phase3NormalOffset;

        float transitionTime =
            returnUpTransitionTime;

        if (playerRb != null)
        {
            bool isGrounded = IsPlayerGrounded();

            // EN EL PUNTO MAS ALTO DEL SALTO
            // O DURANTE UNA CAIDA
            if (!isGrounded &&
                playerRb.velocity.y <= 0f)
            {
                desiredOffset = phase3FallingOffset;

                transitionTime =
                    fallingTransitionTime;
            }
        }

        // CAMBIAR EL ENCUADRE CON ACELERACION
        // Y FRENADO SUAVES
        currentVerticalOffset = Mathf.SmoothDamp(
            currentVerticalOffset,
            desiredOffset,
            ref offsetVelocity,
            Mathf.Max(0.01f, transitionTime)
        );

        // CALCULAR DESTINO VERTICAL
        float targetY = Mathf.Max(
            target.position.y + currentVerticalOffset,
            minY
        );

        // SEGUIMIENTO SUAVE DEL PERSONAJE
        float newY = Mathf.SmoothDamp(
            transform.position.y,
            targetY,
            ref cameraVelocityY,
            Mathf.Max(0.01f, phase3FollowSmoothTime)
        );

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }

    private bool IsPlayerGrounded()
    {
        if (playerMovement == null ||
            playerMovement.groundCheck == null)
        {
            return false;
        }

        return Physics2D.OverlapCircle(
            playerMovement.groundCheck.position,
            0.3f,
            LayerMask.GetMask("Ground")
        ) != null;
    }
}
