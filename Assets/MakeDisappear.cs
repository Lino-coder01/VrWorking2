using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HideWhenNotHeld : MonoBehaviour
{
    public XRGrabInteractable canapeche; // drag ta canapeche ici

    private Renderer[] renderers;
    private Collider[] colliders;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();

        // Cache au départ
        SetVisible(false);

        // Écoute les events de la canapeche
        canapeche.selectEntered.AddListener(OnGrab);
        canapeche.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        SetVisible(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        foreach (var r in renderers)
            r.enabled = visible;

        foreach (var c in colliders)
            c.enabled = visible;
    }

    void OnDestroy()
    {
        canapeche.selectEntered.RemoveListener(OnGrab);
        canapeche.selectExited.RemoveListener(OnRelease);
    }
}