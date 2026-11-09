using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 2;
    public int currentHealth = 2;

    [Header("Corazones UI")]
    public Image heart1;
    public Image heart2;
    public Image heart3;

    [Header("Colores corazones")]
    public Color fullHeartColor = Color.white;
    public Color emptyHeartColor = new Color(1f, 1f, 1f, 0.2f);

    [Header("Animaciones")]
    private Animator animator;

    [Header("Fisica")]
    private Rigidbody2D rb;

    private bool facingRight = true;
    private bool isDead = false;
    private bool thirdHeartUnlocked = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        rb = GetComponent<Rigidbody2D>();

        thirdHeartUnlocked = maxHealth >= 3;
    }

    private void Start()
    {
        UpdateHearts();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal > 0.01f)
        {
            facingRight = true;
        }
        else if (horizontal < -0.01f)
        {
            facingRight = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;

        currentHealth = Mathf.Clamp(
            currentHealth - damage,
            0,
            maxHealth
        );

        UpdateHearts();

        PlayerMovement playerMovement =
            GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.InterruptJetpack();
        }

        Debug.Log("VIDA ACTUAL: " + currentHealth);

        CancelAscendingMovement();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlayHurtAnimation();
        }
    }

    private void CancelAscendingMovement()
    {
        if (rb == null) return;

        if (rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(
                rb.velocity.x,
                0f
            );

            Debug.Log("GOLPE RECIBIDO: ASCENSO CANCELADO");
        }
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;

        int previousHealth = currentHealth;

        currentHealth = Mathf.Clamp(
            currentHealth + amount,
            0,
            maxHealth
        );

        UpdateHearts();

        if (currentHealth > previousHealth)
        {
            Debug.Log("JUGADOR CURADO. VIDA: " + currentHealth);
            PlayHealAnimation();
        }
    }

    public bool UnlockThirdHeart()
    {
        if (isDead || thirdHeartUnlocked)
        {
            return false;
        }

        thirdHeartUnlocked = true;
        maxHealth = 3;
        currentHealth = Mathf.Min(currentHealth + 1, maxHealth);

        UpdateHearts();
        PlayHealAnimation();

        Debug.Log(
            "TERCER CORAZON DESBLOQUEADO. VIDA: " +
            currentHealth + "/" + maxHealth
        );

        return true;
    }

    private void PlayHurtAnimation()
    {
        if (animator == null) return;

        string trigger = facingRight
            ? "HurtRight"
            : "HurtLeft";

        if (!HasTrigger(trigger)) return;

        string oppositeTrigger = facingRight
            ? "HurtLeft"
            : "HurtRight";

        if (HasTrigger(oppositeTrigger))
        {
            animator.ResetTrigger(oppositeTrigger);
        }

        animator.SetTrigger(trigger);
    }

    private void PlayHealAnimation()
    {
        if (animator == null) return;

        string trigger = facingRight
            ? "HealRight"
            : "HealLeft";

        if (!HasTrigger(trigger)) return;

        string oppositeTrigger = facingRight
            ? "HealLeft"
            : "HealRight";

        if (HasTrigger(oppositeTrigger))
        {
            animator.ResetTrigger(oppositeTrigger);
        }

        animator.SetTrigger(trigger);
    }

    private bool HasTrigger(string triggerName)
    {
        if (animator == null) return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == triggerName &&
                parameter.type == AnimatorControllerParameterType.Trigger)
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateHearts()
    {
        UpdateHeartVisual(heart1, currentHealth >= 1, true);
        UpdateHeartVisual(heart2, currentHealth >= 2, true);

        UpdateHeartVisual(
            heart3,
            currentHealth >= 3,
            thirdHeartUnlocked
        );
    }

    private void UpdateHeartVisual(Image heart, bool isFull, bool isVisible)
    {
        if (heart == null) return;

        heart.gameObject.SetActive(isVisible);

        if (!isVisible) return;

        heart.color = isFull
            ? fullHeartColor
            : emptyHeartColor;
    }

    private void Die()
    {
        isDead = true;

        Debug.Log("EL JUGADOR HA MUERTO");

        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.RespawnPlayer();
        }

        currentHealth = maxHealth;
        UpdateHearts();

        isDead = false;

        Debug.Log("VIDA RESTAURADA");
    }
}