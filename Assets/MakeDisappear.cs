using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HideWhenNotHeld : MonoBehaviour
{
    public XRGrabInteractable canapeche;

    private Renderer[] renderers; //All visual components on this object and children
    private Collider[] colliders; //All colliders on this object and children


    /*
     * Collects all Renderers and Colliders in the object hierarchy
     * Hides the object immediately at startup
     * Subscribes to the rod's grab/release events
     */
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

    //Loops through every Renderer and Collider and enables/disables them
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