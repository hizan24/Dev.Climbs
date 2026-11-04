
using UnityEngine;

public class HazardDamage : MonoBehaviour
{
    [Header("Configuración del daño")]
    public bool causesDamage = true;
    public int damageAmount = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!causesDamage) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }
    }
}
