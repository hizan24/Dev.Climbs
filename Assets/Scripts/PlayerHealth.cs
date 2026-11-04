
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 2;
    public int currentHealth = 2;

    [Header("Corazones")]
    public Text heart1;
    public Text heart2;

    private bool isDead = false;

    private void Start()
    {
        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHearts();

        Debug.Log("VIDA ACTUAL: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHearts()
    {
        if (heart1 != null)
            heart1.text = currentHealth >= 1 ? "♥" : "♡";

        if (heart2 != null)
            heart2.text = currentHealth >= 2 ? "♥" : "♡";
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
