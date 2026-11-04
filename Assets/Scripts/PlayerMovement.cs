
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

    private float activeMaxJumpForce;

    private bool isCharging;
    private float currentJumpForce;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public Transform groundCheck;

    private float horizontal;
    private float movingPlatformVelocityX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        activeMaxJumpForce = maxJumpForce;
    }

    public void ActivatePhase2JumpUpgrade()
    {
        activeMaxJumpForce = phase2MaxJumpForce;
    }

    public void SetMovingPlatformVelocity(float velocityX)
    {
        movingPlatformVelocityX = velocityX;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            0.3f,
            LayerMask.GetMask("Ground")
        );

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isCharging = true;
            currentJumpForce = minJumpForce;
        }

        if (isCharging && Input.GetKey(KeyCode.Space))
        {
            currentJumpForce += chargeSpeed * Time.deltaTime;

            currentJumpForce = Mathf.Clamp(
                currentJumpForce,
                minJumpForce,
                activeMaxJumpForce
            );
        }

        // GIRAR SIN DESPLAZARSE AL CARGAR EL SALTO
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
        }

        // EJECUTAR SALTO CARGADO
        if (isCharging && Input.GetKeyUp(KeyCode.Space))
        {
            rb.velocity = new Vector2(
                rb.velocity.x,
                currentJumpForce
            );

            isCharging = false;
        }

        // ACTUALIZAR ANIMACIONES
        if (animator != null)
        {
            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat(
                "Speed",
                isCharging ? 0f : Mathf.Abs(horizontal)
            );

            animator.SetBool("IsJumping", !isGrounded);
            animator.SetBool("IsCharging", isCharging);
        }
    }

    void FixedUpdate()
    {
        // BLOQUEAR MOVIMIENTO DURANTE LA CARGA
        float movementX = isCharging
            ? 0f
            : horizontal * moveSpeed;

        // MANTENER MOVIMIENTO DE PLATAFORMAS MOVILES
        rb.velocity = new Vector2(
            movementX + movingPlatformVelocityX,
            rb.velocity.y
        );
    }
}
