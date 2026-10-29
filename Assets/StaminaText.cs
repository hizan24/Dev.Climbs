
using UnityEngine;
using TMPro;

public class StaminaText : MonoBehaviour
{
    public PlayerMovement player;
    public TMP_Text texto;

    void Update()
    {
        if (player == null || texto == null)
            return;

        float porcentaje = player.currentStamina / player.maxStamina * 100f;

        texto.text = "ENERGÍA: " + Mathf.CeilToInt(porcentaje) + "%";
    }
}
