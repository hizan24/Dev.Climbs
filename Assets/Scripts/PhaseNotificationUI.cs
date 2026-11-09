using UnityEngine;

public class PhaseNotificationUI : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelAvisoFase2;
    public GameObject panelAvisoFase3;
    public GameObject panelAvisoCheckpoint;
    public GameObject panelAvisoJetpack;

    private void Start()
    {
        if (panelAvisoFase2 != null)
            panelAvisoFase2.SetActive(false);

        if (panelAvisoFase3 != null)
            panelAvisoFase3.SetActive(false);

        if (panelAvisoCheckpoint != null)
            panelAvisoCheckpoint.SetActive(false);

        if (panelAvisoJetpack != null)
            panelAvisoJetpack.SetActive(false);
    }

    public void MostrarAvisoFase2()
    {
        if (panelAvisoFase2 == null) return;

        panelAvisoFase2.SetActive(true);
        CancelInvoke(nameof(OcultarAvisoFase2));
        Invoke(nameof(OcultarAvisoFase2), 4f);
    }

    public void MostrarAvisoFase3()
    {
        if (panelAvisoFase3 == null) return;

        panelAvisoFase3.SetActive(true);
        CancelInvoke(nameof(OcultarAvisoFase3));
        Invoke(nameof(OcultarAvisoFase3), 4f);
    }

    public void MostrarAvisoCheckpoint()
    {
        if (panelAvisoCheckpoint == null) return;

        panelAvisoCheckpoint.SetActive(true);
        CancelInvoke(nameof(OcultarAvisoCheckpoint));
        Invoke(nameof(OcultarAvisoCheckpoint), 3f);
    }

    public void MostrarAvisoJetpack()
    {
        if (panelAvisoJetpack == null) return;

        panelAvisoJetpack.SetActive(true);
        CancelInvoke(nameof(OcultarAvisoJetpack));
        Invoke(nameof(OcultarAvisoJetpack), 4f);
    }

    private void OcultarAvisoFase2()
    {
        if (panelAvisoFase2 != null)
            panelAvisoFase2.SetActive(false);
    }

    private void OcultarAvisoFase3()
    {
        if (panelAvisoFase3 != null)
            panelAvisoFase3.SetActive(false);
    }

    private void OcultarAvisoCheckpoint()
    {
        if (panelAvisoCheckpoint != null)
            panelAvisoCheckpoint.SetActive(false);
    }

    private void OcultarAvisoJetpack()
    {
        if (panelAvisoJetpack != null)
            panelAvisoJetpack.SetActive(false);
    }
}