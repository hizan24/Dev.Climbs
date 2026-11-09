
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("Jugador")]
    private Transform player;

    [Header("Punto de reaparicion guardado")]
    [SerializeField]
    private Vector3 respawnPosition;

    [SerializeField]
    private bool checkpointActivated = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        FindPlayer();

        // Guardar la posicion inicial solamente
        // si todavia no se ha activado ningun checkpoint.
        if (player != null && !checkpointActivated)
        {
            respawnPosition = player.position;

            Debug.Log(
                "RESPAWN INICIAL: " +
                respawnPosition.ToString("F2")
            );
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    public void ActivateCheckpoint(Vector3 position)
    {
        respawnPosition = position;
        checkpointActivated = true;

        Debug.Log(
            "CHECKPOINT GUARDADO EN: " +
            respawnPosition.ToString("F2")
        );
    }

    public void RespawnPlayer()
    {
        if (player == null)
        {
            FindPlayer();
        }

        if (player == null)
        {
            Debug.LogError(
                "RESPAWN ERROR: No se encuentra el Player."
            );
            return;
        }

        Vector3 positionBeforeDeath = player.position;

        Rigidbody2D rb =
            player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Cancelar cualquier velocidad anterior.
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // Mover tambien el cuerpo fisico.
            rb.position = new Vector2(
                respawnPosition.x,
                respawnPosition.y
            );
        }

        // Situar el objeto en las coordenadas guardadas.
        player.position = respawnPosition;

        // Sincronizar la fisica con el Transform.
        Physics2D.SyncTransforms();

        Debug.Log(
            "RESPAWN REALIZADO" +
            "\nPosicion antes de morir: " +
            positionBeforeDeath.ToString("F2") +
            "\nPosicion guardada: " +
            respawnPosition.ToString("F2") +
            "\nPosicion tras respawn: " +
            player.position.ToString("F2") +
            "\nCheckpoint activado: " +
            checkpointActivated
        );
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
