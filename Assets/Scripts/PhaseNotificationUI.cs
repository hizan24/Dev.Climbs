
using UnityEngine;

public class PhaseNotificationUI : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelAvisoFase2;
    public GameObject panelAvisoCheckpoint;

    private void Start()
    {
        panelAvisoFase2.SetActive(false);
        panelAvisoCheckpoint.SetActive(false);
    }

    public void MostrarAvisoFase2()
    {
        panelAvisoFase2.SetActive(true);
        CancelInvoke(nameof(OcultarAvisoFase2));
        Invoke(nameof(OcultarAvisoFase2), 4f);
    }

    public void MostrarAvisoCheckpoint()
    {
        panelAvisoCheckpoint.SetActive(true);
        CancelInvoke(nameof(OcultarAvisoCheckpoint));
        Invoke(nameof(OcultarAvisoCheckpoint), 3f);
    }

    private void OcultarAvisoFase2()
    {
        panelAvisoFase2.SetActive(false);
    }

    private void OcultarAvisoCheckpoint()
    {
        panelAvisoCheckpoint.SetActive(false);
    }
}
