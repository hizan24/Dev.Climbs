
using UnityEngine;
using System.Collections;

public class BreakablePlatformPhase3 : MonoBehaviour
{
    [Header("Configuración")]
    public float breakDelay = 1.2f;
    public float respawnDelay = 4f;
    public float shakeAmount = 0.05f;
    public float shakeSpeed = 25f;

    [Header("Animación de ruptura")]
    public Sprite[] breakFrames;

    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;

    private Sprite originalSprite;
    private bool isBreaking = false;

    private void Awake()
    {
        originalPosition = transform.localPosition;

        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();

        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBreaking) return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (collision.contactCount == 0)
            return;

        // Detectar aterrizaje desde arriba.
        if (collision.contacts[0].normal.y > -0.5f)
            return;

        StartCoroutine(BreakPlatform());
    }

    private IEnumerator BreakPlatform()
    {
        isBreaking = true;

        float timer = 0f;

        while (timer < breakDelay)
        {
            // Mantener el temblor original.
            float offset =
                Mathf.Sin(timer * shakeSpeed) * shakeAmount;

            transform.localPosition =
                originalPosition + Vector3.right * offset;

            // Progreso de la ruptura entre 0 y 1.
            float progress = Mathf.Clamp01(
                timer / Mathf.Max(0.01f, breakDelay)
            );

            // Cambiar los sprites progresivamente.
            if (spriteRenderer != null &&
                breakFrames != null &&
                breakFrames.Length > 0)
            {
                int frameIndex = Mathf.FloorToInt(
                    progress * breakFrames.Length
                );

                frameIndex = Mathf.Clamp(
                    frameIndex,
                    0,
                    breakFrames.Length - 1
                );

                if (breakFrames[frameIndex] != null)
                {
                    spriteRenderer.sprite =
                        breakFrames[frameIndex];
                }
            }

            timer += Time.deltaTime;

            yield return null;
        }

        // ROMPER PLATAFORMA
        transform.localPosition = originalPosition;

        if (platformCollider != null)
        {
            platformCollider.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // ESPERAR REAPARICIÓN
        yield return new WaitForSeconds(respawnDelay);

        // RESTAURAR PLATAFORMA
        transform.localPosition = originalPosition;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = originalSprite;
            spriteRenderer.enabled = true;
        }

        if (platformCollider != null)
        {
            platformCollider.enabled = true;
        }

        isBreaking = false;
    }
}
