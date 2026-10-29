
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;

    [Header("Salto cargado")]
    public float minJumpForce = 5f;
    public float maxJumpForce = 9f;
    public float chargeSpeed = 3f;

    [Header("Deteccion del suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Jetpack")]
    public Transform jetpackVisual;
    public float maxStamina = 100f;
    public float staminaDrain = 25f;
    public float jetpackForce = 12f;
    public float doubleTapInterval = 0.5f;

    [Header("Estado del jetpack")]
    public float currentStamina;
    public bool jetpackActive;

    private Rigidbody2D rb;
    private Animator animator;

    private float horizontal;
    private bool isGrounded;
    private bool isCharging;
    private bool isFlying;
    private float currentJumpForce;
    private float lastSpacePressTime = -10f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentStamina = maxStamina;

        if (jetpackVisual != null)
        {
            jetpackVisual.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Si el jetpack no esta desbloqueado, solo usamos el salto normal.
        bool unlocked = GameManager.Instance != null &&
                        GameManager.Instance.jetpackUnlocked;


        if (unlocked && Input.GetKeyDown(KeyCode.Space))
        {
            float timeSinceLastPress = Time.time - lastSpacePressTime;

            if (timeSinceLastPress <= doubleTapInterval)
            {
                if (jetpackActive)
                {
                    jetpackActive = false;
                    isFlying = false;
                }
                else if (currentStamina > 0f)
                {
                    jetpackActive = true;
                    isCharging = false;
                    isFlying = false;
                }

                // Evita que la siguiente pulsación
                // vuelva a contar como parte de esta doble pulsación.
                lastSpacePressTime = -10f;
            }
            else
            {
                lastSpacePressTime = Time.time;
            }
        }


        // Salto cargado normal, solo si el jetpack no esta activo.
        if (!jetpackActive)
        {
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
                    maxJumpForce
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

        // El jetpack se impulsa mientras mantenemos Espacio.
        if (jetpackActive)
        {
            if (Input.GetKey(KeyCode.Space) && currentStamina > 0f)
            {
                isFlying = true;
                currentStamina -= staminaDrain * Time.deltaTime;
                currentStamina = Mathf.Max(0f, currentStamina);

                rb.velocity = new Vector2(
                    rb.velocity.x,
                    jetpackForce
                );
            }
            else
            {
                isFlying = false;
            }

            // Se desactiva con otra doble pulsacion o al quedarse sin energia.
            if (currentStamina <= 0f)
            {
                jetpackActive = false;
                isFlying = false;
            }
        }

        // Mostrar el jetpack solo cuando se haya desbloqueado.
        if (jetpackVisual != null)
        {
            jetpackVisual.gameObject.SetActive(unlocked);
        }

        if (animator != null)
        {
            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Speed", Mathf.Abs(horizontal));
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(
            horizontal * moveSpeed,
            rb.velocity.y
        );
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }
    }
}
