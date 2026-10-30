
using UnityEngine;

public class StaminaBar : MonoBehaviour
{
    public PlayerMovement player;
    public RectTransform fill;

    private float anchoMaximo;

    void Start()
    {
        anchoMaximo = fill.rect.width;
    }

    void Update()
    {
        if (player == null || fill == null)
            return;

        float porcentaje = player.currentStamina / player.maxStamina;
        porcentaje = Mathf.Clamp01(porcentaje);

        fill.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            anchoMaximo * porcentaje
        );
    }
}
