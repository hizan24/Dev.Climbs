
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movimiento horizontal")]
    public float distance = 1.5f;
    public float speed = 1.5f;

    private Rigidbody2D rb;
    private Collider2D platformCollider;

    private float leftLimit;
    private float rightLimit;
    private int direction = 1;

    private float platformVelocityX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        float startX = rb.position.x;

        leftLimit = startX - distance;
        rightLimit = startX + distance;
    }

    void FixedUpdate()
    {
        float oldX = rb.position.x;

        float targetX = direction > 0
            ? rightLimit
            : leftLimit;

        float newX = Mathf.MoveTowards(
            oldX,
            targetX,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(new Vector2(newX, rb.position.y));

        platformVelocityX =
            (newX - oldX) / Time.fixedDeltaTime;

        if (Mathf.Abs(newX - targetX) < 0.01f)
        {
            direction *= -1;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        PlayerMovement player =
            collision.gameObject.GetComponentInParent<PlayerMovement>();

        if (player == null)
            return;

        // Comprobar que el jugador está encima de la plataforma
        if (collision.collider.bounds.min.y >=
            platformCollider.bounds.max.y - 0.15f)
        {
            player.SetMovingPlatformVelocity(platformVelocityX);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        PlayerMovement player =
            collision.gameObject.GetComponentInParent<PlayerMovement>();

        if (player != null)
        {
            player.SetMovingPlatformVelocity(0f);
        }
    }
}
