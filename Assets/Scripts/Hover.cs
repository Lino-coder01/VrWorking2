using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OarHighlight : MonoBehaviour
{
    public Renderer oarRenderer;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.cyan;
    public Color grabbedColor = Color.yellow;

    private XRSimpleInteractable interactable;

    void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (interactable == null)
        {
            Debug.LogWarning("XRSimpleInteractable manquant sur " + gameObject.name);
            return; // On arrête là pour éviter le crash
        }

        // Hover
        interactable.hoverEntered.AddListener(OnHover);
        interactable.hoverExited.AddListener(OnHoverExit);

        // Grab
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    void OnHover(HoverEnterEventArgs args)
    {
        oarRenderer.material.color = hoverColor;
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        oarRenderer.material.color = normalColor;
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        oarRenderer.material.color = grabbedColor;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        oarRenderer.material.color = normalColor;
    }
}
