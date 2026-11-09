
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ElectromagneticPlatformPhase3 : MonoBehaviour
{
    [Header("Impulso electromagnetico")]
    public float launchSpeed = 18f;
    public float rechargeTime = 8f;

    [Header("Animacion activa")]
    public Sprite[] activeFrames;

    [Min(0.1f)]
    public float animationFPS = 4f;

    [Header("Aspecto durante la recarga")]
    public Sprite cooldownSprite;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Sprite originalSprite;

    private bool isReady = true;
    private float readyTime;
    private float animationTimer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
        originalSprite = spriteRenderer.sprite;
    }

    private void Start()
    {
        isReady = true;
        animationTimer = 0f;
    }

    private void Update()
    {
        // COMPROBAR FIN DE RECARGA
        if (!isReady && Time.time >= readyTime)
        {
            isReady = true;
            animationTimer = 0f;

            spriteRenderer.color = originalColor;

            Debug.Log(
                "PLATAFORMA ELECTROMAGNETICA RECARGADA"
            );
        }

        // ASPECTO DURANTE RECARGA
        if (!isReady)
        {
            if (cooldownSprite != null)
            {
                spriteRenderer.sprite = cooldownSprite;
            }

            spriteRenderer.color = new Color(
                originalColor.r * 0.35f,
                originalColor.g * 0.45f,
                originalColor.b * 0.55f,
                originalColor.a
            );

            return;
        }

        // ANIMACION CUANDO ESTA ACTIVA
        spriteRenderer.color = originalColor;

        if (activeFrames == null ||
            activeFrames.Length == 0)
        {
            return;
        }

        animationTimer += Time.deltaTime;

        int frameIndex = Mathf.FloorToInt(
            animationTimer * animationFPS
        ) % activeFrames.Length;

        if (activeFrames[frameIndex] != null)
        {
            spriteRenderer.sprite = activeFrames[frameIndex];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // No impulsar durante la recarga.
        if (!isReady) return;

        // Solo afecta al personaje.
        if (!collision.gameObject.CompareTag("Player"))
            return;

        Rigidbody2D playerRb =
            collision.collider.attachedRigidbody;

        if (playerRb == null)
            return;

        // COMPROBAR ATERRIZAJE DESDE ARRIBA
        bool landedOnTop = false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                landedOnTop = true;
                break;
            }
        }

        if (!landedOnTop)
            return;

        // IMPULSAR VERTICALMENTE AL PERSONAJE
        playerRb.velocity = new Vector2(
            playerRb.velocity.x,
            launchSpeed
        );

        // INICIAR RECARGA
        isReady = false;
        readyTime = Time.time + rechargeTime;

        animationTimer = 0f;

        if (cooldownSprite != null)
        {
            spriteRenderer.sprite = cooldownSprite;
        }
        else
        {
            spriteRenderer.sprite = originalSprite;
        }

        Debug.Log(
            "IMPULSO ELECTROMAGNETICO ACTIVADO"
        );
    }
}
