using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FishingLineArc : MonoBehaviour
{
    [Header("Références")]
    public Transform rodTip;
    public XRRayInteractor teleportInteractor;
    public Transform originalRayOrigin;

    private XRGrabInteractable grabInteractable;
    private Transform fakeOrigin;
    private bool isGrabbed = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        fakeOrigin = new GameObject("FishingRayOrigin").transform;
        fakeOrigin.SetParent(rodTip);
        fakeOrigin.localPosition = Vector3.zero;
        //fakeOrigin.localRotation = Quaternion.identity;
        fakeOrigin.localRotation = Quaternion.Euler(0, 180f, 0);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        teleportInteractor.rayOriginTransform = fakeOrigin;
        teleportInteractor.gameObject.SetActive(true);

        // Active tous les composants visuels
        foreach (var component in teleportInteractor.GetComponents<MonoBehaviour>())
            component.enabled = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        teleportInteractor.rayOriginTransform = originalRayOrigin;
        teleportInteractor.gameObject.SetActive(false);
    }
}