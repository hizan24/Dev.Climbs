
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Transform player;
    private Vector3 respawnPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            respawnPosition = player.position;
        }
    }

    public void ActivateCheckpoint(Vector3 position)
    {
        respawnPosition = position;

        Debug.Log("NUEVO PUNTO DE REAPARICIÓN GUARDADO");
    }

    public void RespawnPlayer()
    {
        if (player == null) return;

        player.position = respawnPosition;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        Debug.Log("JUGADOR REAPARECIDO");
    }
}
