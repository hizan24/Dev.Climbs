
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;

    [Header("Energia")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;

    [Header("Salto")]
    public float jumpForce = 8f;

    [Header("Salto cargado")]
    public float minJumpForce = 5f;
    public float maxJumpForce = 12f;
    public float chargeSpeed = 5f;

    [Header("Mejoras por fase")]
    public float phase2MaxJumpForce = 15f;
    public float phase3MaxJumpForce = 16f;

    [Header("Animacion de salto cargado")]
    [Min(2)]
    public int chargedJumpFrames = 8;

    [Header("Deteccion del suelo")]
    public Transform groundCheck;

    [Header("Jetpack - Desbloqueo")]
    public bool jetpackUnlocked = false;

    [Header("Jetpack - Combustible")]
    public float maxJetpackFuel = 4f;
    public float currentJetpackFuel = 4f;
    public float jetpackRechargeTime = 6f;

    [Header("Jetpack - Vuelo")]
    public float jetpackForce = 9f;
    public float jetpackDoubleTapTime = 0.3f;

    [Header("Jetpack - Impacto morado")]
    public float purpleJetpackImpactDuration = 0.15f;

    private float purpleImpactTimer = 0f;
    private float purpleImpactVelocity = 0f;

    private float activeMaxJumpForce;
    private bool isCharging;
    private float currentJumpForce;
    private bool isGrounded;

    private bool jetpackModeActive = false;
    private bool isJetpackFlying = false;
    private bool flightBlockedUntilRelease = false;

    private bool facingRight = true;
    private float lastSpaceTapTime = -10f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float horizontal;
    private float movingPlatformVelocityX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        activeMaxJumpForce = maxJumpForce;
        currentJetpackFuel = maxJetpackFuel;
    }

    // =====================================
    // MEJORAS DE SALTO
    // =====================================

    public void ActivatePhase2JumpUpgrade()
    {
        activeMaxJumpForce = phase2MaxJumpForce;

        Debug.Log(
            "SALTO FASE 2: " + activeMaxJumpForce
        );
    }

    public void ActivatePhase3JumpUpgrade()
    {
        activeMaxJumpForce = phase3MaxJumpForce;

        Debug.Log(
            "SALTO FASE 3: " + activeMaxJumpForce
        );
    }

    // =====================================
    // DESBLOQUEAR JETPACK
    // =====================================

    public void UnlockJetpack()
    {
        if (jetpackUnlocked) return;

        jetpackUnlocked = true;
        currentJetpackFuel = maxJetpackFuel;

        jetpackModeActive = false;
        isJetpackFlying = false;

        flightBlockedUntilRelease = false;
        lastSpaceTapTime = -10f;

        purpleImpactTimer = 0f;

        Debug.Log("JETPACK DESBLOQUEADO");
    }

    // =====================================
    // INTERRUPCION POR DAÑO
    // =====================================

    public void InterruptJetpack()
    {
        if (!isJetpackFlying) return;

        isJetpackFlying = false;
        flightBlockedUntilRelease = true;

        purpleImpactTimer = 0f;

        Debug.Log(
            "PROPULSION INTERRUMPIDA POR DAÑO"
        );
    }

    // =====================================
    // IMPACTO MORADO CON JETPACK
    // =====================================

    public void ApplyPurpleJetpackImpact(
        float reducedDownwardKnockback)
    {
        if (!jetpackModeActive || !isJetpackFlying)
        {
            return;
        }

        purpleImpactVelocity =
            -Mathf.Abs(reducedDownwardKnockback);

        purpleImpactTimer = Mathf.Max(
            0f,
            purpleJetpackImpactDuration
        );

        Debug.Log(
            "JETPACK: IMPACTO MORADO AMORTIGUADO"
        );
    }

    // =====================================
    // CONSULTAS JETPACK
    // =====================================

    public bool HasJetpackUnlocked()
    {
        return jetpackUnlocked;
    }

    public bool IsJetpackModeActive()
    {
        return jetpackModeActive;
    }

    public bool IsJetpackFlying()
    {
        return isJetpackFlying;
    }

    public float GetJetpackFuelNormalized()
    {
        if (maxJetpackFuel <= 0f)
        {
            return 0f;
        }

        return Mathf.Clamp01(
            currentJetpackFuel / maxJetpackFuel
        );
    }

    // =====================================
    // PLATAFORMAS MOVILES
    // =====================================

    public void SetMovingPlatformVelocity(float velocityX)
    {
        movingPlatformVelocityX = velocityX;
    }

    // =====================================
    // UPDATE
    // =====================================

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        Collider2D groundCollider = null;

        if (groundCheck != null)
        {
            groundCollider = Physics2D.OverlapCircle(
                groundCheck.position,
                0.3f,
                LayerMask.GetMask("Ground")
            );
        }

        isGrounded = groundCollider != null;

        UpdateFacingDirection();

        bool modeChanged = HandleJetpackModeToggle();

        HandleChargedJump(modeChanged);
        HandleJetpackPropulsion();
        HandleJetpackRecharge(groundCollider);
        UpdateAnimator();
    }

    // =====================================
    // DIRECCION
    // =====================================

    private void UpdateFacingDirection()
    {
        if (horizontal > 0.01f)
        {
            facingRight = true;
        }
        else if (horizontal < -0.01f)
        {
            facingRight = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
        }
    }

    // =====================================
    // CAMBIO DE MODO
    // =====================================

    private bool HandleJetpackModeToggle()
    {
        if (!jetpackUnlocked)
        {
            return false;
        }

        if (!Input.GetKeyDown(KeyCode.Space))
        {
            return false;
        }

        float timeSinceLastTap =
            Time.time - lastSpaceTapTime;

        if (timeSinceLastTap <= jetpackDoubleTapTime)
        {
            jetpackModeActive = !jetpackModeActive;

            isCharging = false;
            isJetpackFlying = false;

            flightBlockedUntilRelease = false;
            purpleImpactTimer = 0f;

            lastSpaceTapTime = -10f;

            Debug.Log(
                jetpackModeActive
                    ? "MODO JETPACK ACTIVADO"
                    : "MODO SALTO NORMAL ACTIVADO"
            );

            return true;
        }

        lastSpaceTapTime = Time.time;
        return false;
    }

    // =====================================
    // SALTO CARGADO
    // =====================================

    private void HandleChargedJump(bool modeChanged)
    {
        if (jetpackModeActive)
        {
            isCharging = false;
            return;
        }

        if (modeChanged)
        {
            isCharging = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isCharging = true;
            currentJumpForce = minJumpForce;
        }

        if (isCharging && Input.GetKey(KeyCode.Space))
        {
            currentJumpForce +=
                chargeSpeed * Time.deltaTime;

            currentJumpForce = Mathf.Clamp(
                currentJumpForce,
                minJumpForce,
                activeMaxJumpForce
            );
        }

        if (isCharging && Input.GetKeyUp(KeyCode.Space))
        {
            rb.velocity = new Vector2(
                rb.velocity.x,
                currentJumpForce
            );

            isCharging = false;
        }
    }

    // =====================================
    // PROPULSION
    // =====================================

    private void HandleJetpackPropulsion()
    {
        if (!jetpackUnlocked || !jetpackModeActive)
        {
            isJetpackFlying = false;
            return;
        }

        if (!Input.GetKey(KeyCode.Space))
        {
            flightBlockedUntilRelease = false;
        }

        bool wantsToFly =
            Input.GetKey(KeyCode.Space) &&
            !flightBlockedUntilRelease;

        bool hasFuel =
            currentJetpackFuel > 0f;

        isJetpackFlying =
            wantsToFly && hasFuel;
    }

    // =====================================
    // RECARGA DE COMBUSTIBLE
    // =====================================

    private void HandleJetpackRecharge(
        Collider2D groundCollider)
    {
        if (!jetpackUnlocked) return;
        if (isJetpackFlying) return;
        if (!isGrounded) return;
        if (groundCollider == null) return;

        if (!CanRechargeOnPlatform(groundCollider))
        {
            return;
        }

        float rechargeRate =
            maxJetpackFuel /
            Mathf.Max(0.1f, jetpackRechargeTime);

        currentJetpackFuel = Mathf.MoveTowards(
            currentJetpackFuel,
            maxJetpackFuel,
            rechargeRate * Time.deltaTime
        );
    }

    private bool CanRechargeOnPlatform(
        Collider2D groundCollider)
    {
        if (groundCollider.GetComponentInParent<
            HazardDamage>() != null)
        {
            return false;
        }

        if (groundCollider.GetComponentInParent<
            BreakablePlatform>() != null)
        {
            return false;
        }

        if (groundCollider.GetComponentInParent<
            BreakablePlatform>() != null)
        {
            return false;
        }

        if (groundCollider.GetComponentInParent<
            ElectromagneticPlatformPhase3>() != null)
        {
            return false;
        }

        return true;
    }

    // =====================================
    // MOVIMIENTO
    // =====================================

    private void FixedUpdate()
    {
        float movementX = isCharging
            ? 0f
            : horizontal * moveSpeed;

        float finalMovementX =
            movementX + movingPlatformVelocityX;

        if (isJetpackFlying &&
            currentJetpackFuel > 0f)
        {
            float verticalVelocity = jetpackForce;

            // Mantener brevemente el empujon
            // hacia abajo de un objeto morado.
            // El modo jetpack sigue activo.
            if (purpleImpactTimer > 0f)
            {
                verticalVelocity =
                    purpleImpactVelocity;

                purpleImpactTimer = Mathf.Max(
                    0f,
                    purpleImpactTimer - Time.fixedDeltaTime
                );
            }

            rb.velocity = new Vector2(
                finalMovementX,
                verticalVelocity
            );

            // El consumo sigue siendo normal:
            // no hay penalizacion extra.
            currentJetpackFuel = Mathf.Max(
                0f,
                currentJetpackFuel - Time.fixedDeltaTime
            );

            if (currentJetpackFuel <= 0f)
            {
                isJetpackFlying = false;
                purpleImpactTimer = 0f;

                Debug.Log(
                    "COMBUSTIBLE DEL JETPACK AGOTADO"
                );
            }

            return;
        }

        // Si dejamos de propulsarnos,
        // el impacto ya no necesita mantenerse.
        purpleImpactTimer = 0f;

        rb.velocity = new Vector2(
            finalMovementX,
            rb.velocity.y
        );
    }

    // =====================================
    // ANIMACIONES
    // =====================================

    private void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetFloat(
            "Horizontal",
            horizontal
        );

        animator.SetFloat(
            "Speed",
            isCharging ? 0f : Mathf.Abs(horizontal)
        );

        animator.SetBool(
            "IsJumping",
            !isGrounded && !jetpackModeActive
        );

        animator.SetBool(
            "IsCharging",
            isCharging
        );

        animator.SetFloat(
            "ChargeProgress",
            CalculateChargeAnimationTime()
        );

        // Mantener JetpackRight/JetpackLeft
        // mientras el modo este activo
        // y el personaje este en el aire.
        bool showJetpackAnimation =
            jetpackModeActive && !isGrounded;

        if (HasBoolParameter("IsFlying"))
        {
            animator.SetBool(
                "IsFlying",
                showJetpackAnimation
            );
        }

        if (HasBoolParameter("FacingRight"))
        {
            animator.SetBool(
                "FacingRight",
                facingRight
            );
        }
    }

    private bool HasBoolParameter(string parameterName)
    {
        if (animator == null) return false;

        foreach (AnimatorControllerParameter parameter
            in animator.parameters)
        {
            if (parameter.name == parameterName &&
                parameter.type ==
                AnimatorControllerParameterType.Bool)
            {
                return true;
            }
        }

        return false;
    }

    // =====================================
    // ANIMACION SALTO CARGADO
    // =====================================

    private float CalculateChargeAnimationTime()
    {
        if (!isCharging)
        {
            return 0f;
        }

        float forceRange =
            activeMaxJumpForce - minJumpForce;

        if (forceRange <= 0f)
        {
            return 1f;
        }

        float chargePercent = Mathf.Clamp01(
            (currentJumpForce - minJumpForce) /
            forceRange
        );

        if (chargePercent >= 1f)
        {
            return 1f;
        }

        int frames = Mathf.Max(2, chargedJumpFrames);

        float lastFrameStart =
            (frames - 1f) / frames;

        float timeBeforeLastFrame =
            Mathf.Max(0f, lastFrameStart - 0.002f);

        return chargePercent * timeBeforeLastFrame;
    }
}
