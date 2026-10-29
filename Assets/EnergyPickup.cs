
using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
    [Range(0f, 100f)]
    public float energyAmount = 50f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player == null)
            return;

        player.currentStamina = Mathf.Min(
            player.currentStamina + energyAmount,
            player.maxStamina
        );

        Destroy(gameObject);
    }
}
