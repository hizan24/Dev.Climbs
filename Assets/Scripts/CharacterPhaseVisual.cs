
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterPhaseVisuals : MonoBehaviour
{
    [Header("Animaciones por fase")]
    public RuntimeAnimatorController phase1Controller;
    public RuntimeAnimatorController phase2Controller;
    public RuntimeAnimatorController phase3Controller;

    private Animator animator;

    private bool phase2Activated = false;
    private bool phase3Activated = false;

    void Awake()
    {
        animator = GetComponent<Animator>();

        // Guardar el Animator original
        if (phase1Controller == null)
        {
            phase1Controller =
                animator.runtimeAnimatorController;
        }
    }

    // ACTIVAR APARIENCIA DE FASE 2
    public void ApplyPhase2Visuals()
    {
        if (phase2Activated || phase3Activated) return;

        if (phase2Controller == null)
        {
            Debug.LogWarning(
                "No se ha asignado el Animator de Fase 2"
            );
            return;
        }

        animator.runtimeAnimatorController =
            phase2Controller;

        phase2Activated = true;

        Debug.Log(
            "EVOLUCION VISUAL: FASE 2 ACTIVADA"
        );
    }

    // ACTIVAR APARIENCIA DE FASE 3
    public void ApplyPhase3Visuals()
    {
        if (phase3Activated) return;

        if (phase3Controller == null)
        {
            Debug.LogWarning(
                "No se ha asignado el Animator de Fase 3"
            );
            return;
        }

        animator.runtimeAnimatorController =
            phase3Controller;

        phase3Activated = true;

        Debug.Log(
            "EVOLUCION VISUAL: FASE 3 ACTIVADA"
        );
    }
}
