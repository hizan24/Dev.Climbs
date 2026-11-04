
using UnityEngine;

public class PhaseUpgrade : MonoBehaviour
{
    private bool upgradeActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (upgradeActivated) return;

        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.ActivatePhase2JumpUpgrade();
                upgradeActivated = true;

                Debug.Log("MEJORA DE SALTO DE FASE 2 ACTIVADA");
            }
        }
    }
}
