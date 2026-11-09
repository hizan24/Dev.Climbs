
using UnityEngine;
using System.Collections;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Configuración")]
    public float breakDelay = 1.2f;
    public float respawnDelay = 4f;
    public float shakeAmount = 0.05f;
    public float shakeSpeed = 25f;

    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private bool isBreaking = false;

    private void Awake()
    {
        originalPosition = transform.localPosition;
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBreaking) return;
        if (!collision.gameObject.CompareTag("Player")) return;
        if (collision.contactCount == 0) return;

        // Detectar el aterrizaje sobre la plataforma.
        if (collision.contacts[0].normal.y > -0.5f) return;

        StartCoroutine(BreakPlatform());
    }

    private IEnumerator BreakPlatform()
    {
        isBreaking = true;

        float timer = 0f;

        while (timer < breakDelay)
        {
            float offset = Mathf.Sin(timer * shakeSpeed) * shakeAmount;

            transform.localPosition = originalPosition +
                                      Vector3.right * offset;

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;

        platformCollider.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        transform.localPosition = originalPosition;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        platformCollider.enabled = true;
        isBreaking = false;
    }
}