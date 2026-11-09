
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectSpawnerPhase3 : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Camera gameCamera;
    public Transform phase3Start;
    public RuntimeAnimatorController phase3Controller;

    [Header("Prefabs de objetos")]
    public GameObject purplePrefab;
    public GameObject redPrefab;
    public GameObject greenPrefab;

    [Header("Generacion")]
    [Min(1)]
    public int maxSimultaneousObjects = 4;

    [Range(0f, 1f)]
    public float aimedSpawnChance = 0.25f;

    public float spawnAboveCamera = 1f;

    [Range(0f, 0.4f)]
    public float horizontalMargin = 0.1f;

    [Header("Limites horizontales")]
    public float minSpawnX = -8.5f;
    public float maxSpawnX = 8.5f;

    [Header("Frecuencias por altura")]
    public Vector2 firstSectionInterval =
        new Vector2(2f, 3f);

    public Vector2 secondSectionInterval =
        new Vector2(1.5f, 2.5f);

    public Vector2 thirdSectionInterval =
        new Vector2(1f, 2f);

    [Header("Probabilidades relativas")]
    public float purpleWeight = 60f;
    public float redWeight = 30f;
    public float greenWeight = 10f;

    private Animator playerAnimator;

    private readonly List<GameObject> activeObjects =
        new List<GameObject>();

    private bool generationActivated = false;
    private bool generationStopped = false;

    private float nextSpawnTime;
    private float pausedUntil;

    private void Awake()
    {
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }

        CachePlayerAnimator();
    }

    private void Update()
    {
        if (generationStopped)
        {
            return;
        }

        CachePlayerAnimator();

        if (!ReferencesReady())
        {
            return;
        }

        // ACTIVAR SOLO AL LLEGAR A FASE 3
        if (!generationActivated)
        {
            if (playerAnimator.runtimeAnimatorController
                != phase3Controller)
            {
                return;
            }

            generationActivated = true;

            ScheduleNextSpawn();

            Debug.Log(
                "GENERADOR DE OBJETOS ACTIVADO"
            );
        }

        // UNA VEZ ACTIVADO, CONTINUA EN
        // FASES 3, 4 Y 5.

        CleanOldObjects();

        // PAUSA TEMPORAL DE GENERACION
        if (Time.time < pausedUntil)
        {
            return;
        }

        // ESPERAR PROXIMA APARICION
        if (Time.time < nextSpawnTime)
        {
            return;
        }

        // LIMITE DE OBJETOS SIMULTANEOS
        if (activeObjects.Count >=
            Mathf.Max(1, maxSimultaneousObjects))
        {
            return;
        }

        SpawnObject();

        ScheduleNextSpawn();
    }

    private void CachePlayerAnimator()
    {
        if (player == null || playerAnimator != null)
        {
            return;
        }

        playerAnimator = player.GetComponent<Animator>();

        if (playerAnimator == null)
        {
            playerAnimator =
                player.GetComponentInChildren<Animator>();
        }
    }

    private bool ReferencesReady()
    {
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }

        return player != null &&
               playerAnimator != null &&
               gameCamera != null &&
               phase3Start != null &&
               phase3Controller != null &&
               purplePrefab != null &&
               redPrefab != null &&
               greenPrefab != null;
    }

    private void ScheduleNextSpawn()
    {
        // Altura recorrida desde el inicio
        // de la Fase 3.
        float playerHeight = Mathf.Max(
            0f,
            player.position.y - phase3Start.position.y
        );

        Vector2 interval;

        if (playerHeight < 20f)
        {
            interval = firstSectionInterval;
        }
        else if (playerHeight < 40f)
        {
            interval = secondSectionInterval;
        }
        else
        {
            // Se mantiene en las Fases 4 y 5
            // hasta que decidamos ajustar
            // su dificultad.
            interval = thirdSectionInterval;
        }

        float minDelay = Mathf.Max(
            0.1f,
            Mathf.Min(interval.x, interval.y)
        );

        float maxDelay = Mathf.Max(
            minDelay,
            Mathf.Max(interval.x, interval.y)
        );

        nextSpawnTime = Time.time +
            Random.Range(minDelay, maxDelay);
    }

    private void SpawnObject()
    {
        GameObject selectedPrefab = ChoosePrefab();

        if (selectedPrefab == null)
        {
            return;
        }

        float halfHeight =
            gameCamera.orthographicSize;

        float halfWidth =
            halfHeight * gameCamera.aspect;

        float cameraLeft =
            gameCamera.transform.position.x - halfWidth;

        float cameraRight =
            gameCamera.transform.position.x + halfWidth;

        float margin =
            halfWidth * 2f * horizontalMargin;

        // Respetar tanto la camara como
        // los limites horizontales del nivel.
        float minX = Mathf.Max(
            cameraLeft + margin,
            Mathf.Min(minSpawnX, maxSpawnX)
        );

        float maxX = Mathf.Min(
            cameraRight - margin,
            Mathf.Max(minSpawnX, maxSpawnX)
        );

        if (minX > maxX)
        {
            return;
        }

        float spawnX;

        // 25 % alineado con el jugador
        // en el instante de aparicion.
        if (Random.value < aimedSpawnChance)
        {
            spawnX = Mathf.Clamp(
                player.position.x,
                minX,
                maxX
            );
        }
        else
        {
            spawnX = Random.Range(
                minX,
                maxX
            );
        }

        // Aparecer por encima de la camara.
        float spawnY =
            gameCamera.transform.position.y +
            halfHeight +
            spawnAboveCamera;

        Vector3 spawnPosition = new Vector3(
            spawnX,
            spawnY,
            0f
        );

        GameObject newObject = Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity
        );

        activeObjects.Add(newObject);
    }

    private GameObject ChoosePrefab()
    {
        float purple =
            Mathf.Max(0f, purpleWeight);

        float red =
            Mathf.Max(0f, redWeight);

        float green =
            Mathf.Max(0f, greenWeight);

        float total = purple + red + green;

        if (total <= 0f)
        {
            return null;
        }

        float randomValue =
            Random.Range(0f, total);

        if (randomValue < purple)
        {
            return purplePrefab;
        }

        if (randomValue < purple + red)
        {
            return redPrefab;
        }

        return greenPrefab;
    }

    private void CleanOldObjects()
    {
        for (int i = activeObjects.Count - 1;
             i >= 0;
             i--)
        {
            GameObject fallingObject =
                activeObjects[i];

            // El objeto ya se destruyo.
            if (fallingObject == null)
            {
                activeObjects.RemoveAt(i);
                continue;
            }

            Vector3 viewportPosition =
                gameCamera.WorldToViewportPoint(
                    fallingObject.transform.position
                );

            // El objeto ya salio por debajo
            // de la pantalla.
            if (viewportPosition.y < -0.2f)
            {
                activeObjects.RemoveAt(i);

                Destroy(fallingObject);
            }
        }
    }

    // PAUSAR GENERACION TEMPORALMENTE
    // Se utilizara al recoger el jetpack.
    public void PauseSpawning(float seconds)
    {
        if (seconds <= 0f)
        {
            return;
        }

        pausedUntil = Mathf.Max(
            pausedUntil,
            Time.time + seconds
        );

        // No acumular apariciones pendientes.
        nextSpawnTime = Mathf.Max(
            nextSpawnTime,
            pausedUntil
        );

        Debug.Log(
            "GENERACION PAUSADA DURANTE " +
            seconds + " SEGUNDOS"
        );
    }

    // DETENER EL GENERADOR AL FINAL DEL JUEGO
    public void StopSpawning()
    {
        generationStopped = true;

        Debug.Log(
            "GENERACION DE OBJETOS FINALIZADA"
        );
    }
}

