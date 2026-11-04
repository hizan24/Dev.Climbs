
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterPhaseVisuals : MonoBehaviour
{
    [Header("Animaciones por fase")]
    public RuntimeAnimatorController phase1Controller;
    public RuntimeAnimatorController phase2Controller;

    private Animator animator;
    private bool phase2Activated = false;

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

    public void ApplyPhase2Visuals()
    {
        if (phase2Activated) return;

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
}
