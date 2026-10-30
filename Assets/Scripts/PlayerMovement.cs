
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

    private bool isCharging;
    private float currentJumpForce;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Animator animator;

    public Transform groundCheck;

    private float horizontal;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
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

        if (animator != null)
        {
            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Speed", Mathf.Abs(horizontal));
            animator.SetBool("IsJumping", !isGrounded);
            animator.SetBool("IsCharging", isCharging);
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(
            horizontal * moveSpeed,
            rb.velocity.y
        );
    }
}
