using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using static Unity.VisualScripting.Member;

public class RowingController : MonoBehaviour
{
    public XRSimpleInteractable leftOar;
    public XRSimpleInteractable rightOar;

    public Rigidbody boatRigidbody;

    public float rowingForce = 10f;
    public float steeringForce = 40f;
    public float bothOarsForce = 10f;

    public Transform playerRig;

    private IXRSelectInteractor leftInteractor;
    private IXRSelectInteractor rightInteractor;

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    private Vector3 lastBoatPosition;
    private Quaternion lastBoatRotation;

    private bool leftOarGrabbed;
    private bool rightOarGrabbed;


    public DynamicMoveProvider moveProvider;

    void Start()
    {
        leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        lastBoatPosition = boatRigidbody.position;
        lastBoatRotation = boatRigidbody.rotation;
    }

    void OnEnable()
    {
        leftOar.selectEntered.AddListener(e => leftOarGrabbed = true);
        leftOar.selectExited.AddListener(e => leftOarGrabbed = false);
        rightOar.selectEntered.AddListener(e => rightOarGrabbed = true);
        rightOar.selectExited.AddListener(e => rightOarGrabbed = false);
    }

    void OnDisable()
    {
        leftOar.selectEntered.RemoveAllListeners();
        leftOar.selectExited.RemoveAllListeners();
        rightOar.selectEntered.RemoveAllListeners();
        rightOar.selectExited.RemoveAllListeners();
    }

    void FixedUpdate()
    {
        Vector3 leftVel = Vector3.zero;
        Vector3 rightVel = Vector3.zero;
        Quaternion leftRot = Quaternion.identity;
        Quaternion rightRot = Quaternion.identity;

        leftDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out leftVel);
        rightDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out rightVel);
        leftDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out leftRot);
        rightDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out rightRot);

        // Avancer — les deux rames saisies
        if (leftOarGrabbed && rightOarGrabbed)
        {
            Vector3 avgVelocity = (leftVel + rightVel) / 2f;

            float forwardBackInput = -avgVelocity.z; // positif = mouvement vers l'arrière
            float downwardInput = -avgVelocity.y;    // positif = mouvement vers le bas

            bool isRowingMotion = forwardBackInput > 0.2f && downwardInput > 0.1f;

            if (isRowingMotion)
            {
                // On combine les deux axes pour avoir l'intensité du coup de rame
                float rowInput = (forwardBackInput + downwardInput) / 2f;
                Vector3 newPosition = boatRigidbody.position + transform.forward * rowInput * bothOarsForce * Time.fixedDeltaTime;
                boatRigidbody.MovePosition(newPosition);
            }
        }

        // Tourner à droite — main gauche seulement + mouvement de rame détecté
        if (!leftOarGrabbed && rightOarGrabbed)
        {
            float forwardBackInput = -leftVel.z;
            float downwardInput = -leftVel.y;

            bool isRowingMotion = forwardBackInput > 0.2f && downwardInput > 0.1f;

            if (isRowingMotion)
            {
                float rowInput = (forwardBackInput + downwardInput) / 2f;
                boatRigidbody.MoveRotation(boatRigidbody.rotation * Quaternion.Euler(0, -rowInput * steeringForce * Time.fixedDeltaTime, 0));
            }
        }

        // Tourner à droite — main droite seulement + mouvement détecté
        if (leftOarGrabbed && !rightOarGrabbed)
        {
            float forwardBackInput = -rightVel.z;
            float downwardInput = -rightVel.y;

            bool isRowingMotion = forwardBackInput > 0.2f && downwardInput > 0.1f;

            if (isRowingMotion)               
            {
                float rowInput = (forwardBackInput + downwardInput) / 2f;
                boatRigidbody.MoveRotation(boatRigidbody.rotation * Quaternion.Euler(0, rowInput * steeringForce * Time.fixedDeltaTime, 0));
            }
        }

        Vector3 deltaPosition = boatRigidbody.position - lastBoatPosition;
        Quaternion deltaRotation = boatRigidbody.rotation * Quaternion.Inverse(lastBoatRotation);

        if (deltaPosition != Vector3.zero || deltaRotation != Quaternion.identity)
        {
            // Temporarily disable the move provider so it doesn't fight us
            moveProvider.enabled = false;

            playerRig.SetPositionAndRotation(
                playerRig.position + deltaPosition,
                deltaRotation * playerRig.rotation
            );
        }
        else
        {
            // No boat movement — give control back to DynamicMoveProvider
            moveProvider.enabled = true;
        }

        lastBoatPosition = boatRigidbody.position;
        lastBoatRotation = boatRigidbody.rotation;
    }
}