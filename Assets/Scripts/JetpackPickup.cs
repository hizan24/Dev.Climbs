
using UnityEngine;

public class JetpackPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnlockJetpack();
                Destroy(gameObject);
            }
        }
    }
}
