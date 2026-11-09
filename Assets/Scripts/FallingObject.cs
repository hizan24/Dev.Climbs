
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FallingObject : MonoBehaviour
{
    public enum ObjectType
    {
        Purple,
        Red,
        Green
    }

    [Header("Tipo de objeto")]
    public ObjectType objectType;

    [Header("Caida")]
    public float fallSpeed = 6f;
    public float maxLifetime = 15f;

    [Header("Impacto morado")]
    public float downwardKnockback = 16f;

    [Range(0f, 1f)]
    public float jetpackKnockbackMultiplier = 0.5f;

    private Rigidbody2D rb;
    private bool consumed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, maxLifetime);
    }

    private void FixedUpdate()
    {
        if (consumed) return;

        rb.velocity = new Vector2(
            0f,
            -fallSpeed
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed) return;

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            consumed = true;

            ApplyEffect(playerHealth);

            Destroy(gameObject);
            return;
        }

        if (other.gameObject.layer ==
            LayerMask.NameToLayer("Ground"))
        {
            consumed = true;
            Destroy(gameObject);
        }
    }

    private void ApplyEffect(PlayerHealth playerHealth)
    {
        switch (objectType)
        {
            case ObjectType.Purple:
                ApplyPurpleEffect(playerHealth);
                break;

            case ObjectType.Red:
                playerHealth.TakeDamage(1);
                Debug.Log("OBJETO ROJO: -1 CORAZON");
                break;

            case ObjectType.Green:
                playerHealth.Heal(1);
                Debug.Log("OBJETO VERDE: +1 CORAZON");
                break;
        }
    }

    private void ApplyPurpleEffect(
        PlayerHealth playerHealth)
    {
        Rigidbody2D playerRb =
            playerHealth.GetComponent<Rigidbody2D>();

        if (playerRb == null) return;

        PlayerMovement movement =
            playerHealth.GetComponent<PlayerMovement>();

        bool jetpackActive =
            movement != null &&
            movement.IsJetpackModeActive();

        float appliedKnockback = downwardKnockback;

        if (jetpackActive)
        {
            appliedKnockback *= jetpackKnockbackMultiplier;
        }

        // Si el jetpack esta propulsando,
        // dejamos que PlayerMovement gestione
        // el impacto durante varios fotogramas.
        if (jetpackActive &&
            movement.IsJetpackFlying())
        {
            movement.ApplyPurpleJetpackImpact(
                appliedKnockback
            );
        }
        else
        {
            // Comportamiento normal:
            // el impacto empuja hacia abajo.
            float newVelocityY =
                Mathf.Min(playerRb.velocity.y, 0f)
                - appliedKnockback;

            playerRb.velocity = new Vector2(
                playerRb.velocity.x,
                newVelocityY
            );
        }

        Debug.Log(
            jetpackActive
                ? "OBJETO MORADO: IMPACTO REDUCIDO AL 50%"
                : "OBJETO MORADO: IMPULSO NORMAL"
        );
    }
}
