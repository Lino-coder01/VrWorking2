using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FishingRodAim : MonoBehaviour
{
    public XRGrabInteractable grabInteractable;
    public XRRayInteractor rayInteractor;
    public GameObject groundMarker;

    private void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        rayInteractor.gameObject.SetActive(false);
        groundMarker.SetActive(false);
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        rayInteractor.gameObject.SetActive(true);
        groundMarker.SetActive(true);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        rayInteractor.gameObject.SetActive(false);
        groundMarker.SetActive(false);
    }

    private void Update()
    {
        if (!rayInteractor.gameObject.activeSelf)
            return;

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            groundMarker.SetActive(true);
            groundMarker.transform.position = hit.point + Vector3.up * 0.05f;
            groundMarker.transform.rotation = Quaternion.identity;
        }
        else
        {
            groundMarker.SetActive(false);
        }
    }
}