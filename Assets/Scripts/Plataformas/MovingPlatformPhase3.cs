
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatformPhase3 : MonoBehaviour
{
    [Header("Movimiento horizontal")]
    public float distance = 1.5f;
    public float speed = 1.5f;

    [Header("Sprites hacia la derecha")]
    public Sprite[] rightFrames;

    [Header("Sprites hacia la izquierda")]
    public Sprite[] leftFrames;

    [Header("Velocidad de animacion")]
    [Min(0.1f)]
    public float animationFPS = 3f;

    private Rigidbody2D rb;
    private Collider2D platformCollider;
    private SpriteRenderer spriteRenderer;

    private float leftLimit;
    private float rightLimit;

    // 1 = derecha, -1 = izquierda
    private int direction = 1;

    private float platformVelocityX;
    private float animationTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        float startX = rb.position.x;

        leftLimit = startX - distance;
        rightLimit = startX + distance;

        UpdatePlatformSprite();
    }

    void Update()
    {
        animationTimer += Time.deltaTime;

        UpdatePlatformSprite();
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

        rb.MovePosition(
            new Vector2(newX, rb.position.y)
        );

        platformVelocityX =
            (newX - oldX) / Time.fixedDeltaTime;

        // Cambiar direccion al llegar al extremo.
        if (Mathf.Abs(newX - targetX) < 0.01f)
        {
            direction *= -1;

            // Reiniciar solo la animacion visual.
            animationTimer = 0f;

            UpdatePlatformSprite();
        }
    }

    private void UpdatePlatformSprite()
    {
        if (spriteRenderer == null)
            return;

        // Elegir solamente los sprites
        // correspondientes a la direccion actual.
        Sprite[] currentFrames = direction > 0
            ? rightFrames
            : leftFrames;

        if (currentFrames == null ||
            currentFrames.Length == 0)
            return;

        int frameIndex = Mathf.FloorToInt(
            animationTimer * animationFPS
        ) % currentFrames.Length;

        Sprite selectedSprite = currentFrames[frameIndex];

        if (selectedSprite != null)
        {
            spriteRenderer.sprite = selectedSprite;
        }

        // No reflejar la imagen.
        // Cada direccion tiene sus propios sprites.
        spriteRenderer.flipX = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        PlayerMovement player =
            collision.gameObject
                .GetComponentInParent<PlayerMovement>();

        if (player == null)
            return;

        // Mantener el transporte del personaje.
        if (collision.collider.bounds.min.y >=
            platformCollider.bounds.max.y - 0.15f)
        {
            player.SetMovingPlatformVelocity(
                platformVelocityX
            );
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        PlayerMovement player =
            collision.gameObject
                .GetComponentInParent<PlayerMovement>();

        if (player != null)
        {
            player.SetMovingPlatformVelocity(0f);
        }
    }
}
