
using UnityEngine;
using UnityEngine.UI;

public class JetpackFuelUI : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerMovement player;
    public GameObject fuelBarRoot;
    public Image fuelFill;

    [Header("Colores")]
    public Color normalFuelColor =
        new Color(0.13f, 0.83f, 0.93f, 1f);

    public Color lowFuelColor =
        new Color(1f, 0.25f, 0.25f, 1f);

    [Range(0f, 1f)]
    public float lowFuelThreshold = 0.25f;

    private void Awake()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerMovement>();
        }

        // Ocultar la barra al comenzar.
        // Se mostrara cuando se desbloquee
        // el jetpack.
        if (fuelBarRoot != null)
        {
            fuelBarRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerMovement>();

            if (player == null)
            {
                return;
            }
        }

        bool unlocked = player.HasJetpackUnlocked();

        // Mostrar la barra solamente cuando
        // se haya recogido el jetpack.
        if (fuelBarRoot != null &&
            fuelBarRoot.activeSelf != unlocked)
        {
            fuelBarRoot.SetActive(unlocked);
        }

        if (!unlocked || fuelFill == null)
        {
            return;
        }

        // Obtener combustible entre 0 y 1.
        float fuelAmount =
            player.GetJetpackFuelNormalized();

        // Actualizar el relleno visual.
        fuelFill.fillAmount = fuelAmount;

        // Cambiar a rojo cuando quede
        // poco combustible.
        if (fuelAmount <= lowFuelThreshold)
        {
            fuelFill.color = lowFuelColor;
        }
        else
        {
            fuelFill.color = normalFuelColor;
        }
    }
}
