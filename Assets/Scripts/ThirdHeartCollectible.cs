
using UnityEngine;

public class ThirdHeartCollectible : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        Rigidbody2D playerRb =
            other.attachedRigidbody;

        if (playerRb == null) return;

        if (!playerRb.CompareTag("Player"))
        {
            return;
        }

        PlayerHealth playerHealth =
            playerRb.GetComponent<PlayerHealth>();

        if (playerHealth == null) return;

        bool unlocked =
            playerHealth.UnlockThirdHeart();

        if (!unlocked) return;

        collected = true;

        Debug.Log("TERCER CORAZON RECOGIDO");

        // Evitar que vuelva a detectar al jugador
        Collider2D collectibleCollider =
            GetComponent<Collider2D>();

        if (collectibleCollider != null)
        {
            collectibleCollider.enabled = false;
        }

        Destroy(gameObject);
    }
}
