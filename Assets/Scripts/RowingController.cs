using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class RowingController : MonoBehaviour
{
    public XRSimpleInteractable leftOar;
    public XRSimpleInteractable rightOar;
    public Rigidbody boatRigidbody;
    public float rowingForce = 10f;
    public float steeringForce = 12000f;
    public float rotationMultiplier = 2f;
    public float bothOarsForce = 10f;
    public Transform playerRig;

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    private bool leftOarGrabbed;
    private bool rightOarGrabbed;

    public Transform fishingRod;
    public DynamicMoveProvider moveProvider;

    private Vector3 localPlayerOffset;
    private Quaternion localPlayerRotation;

    private Vector3 localFishingRodOffset;
    private Quaternion localFishingRodRotation;

    void Start()
    {
        leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        RecalculateLocalOffset();
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

    void RecalculateLocalOffset()
    {
        localPlayerOffset = boatRigidbody.transform.InverseTransformPoint(playerRig.position);
        localPlayerRotation = Quaternion.Inverse(boatRigidbody.rotation) * playerRig.rotation;

        if (fishingRod != null)
        {
            localFishingRodOffset = boatRigidbody.transform.InverseTransformPoint(fishingRod.position);
            localFishingRodRotation = Quaternion.Inverse(boatRigidbody.rotation) * fishingRod.rotation;
        }
    }

    void FixedUpdate()
    {
        leftDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVel);
        rightDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightVel);

        bool boatMoved = false;

        if (leftOarGrabbed && rightOarGrabbed)
        {
            Vector3 avgVelocity = (leftVel + rightVel) / 2f;
            float forwardBackInput = -avgVelocity.z;
            float downwardInput = -avgVelocity.y;

            if (forwardBackInput > 0.2f && downwardInput > 0.1f)
            {
                float rowInput = (forwardBackInput + downwardInput) / 2f;
                boatRigidbody.MovePosition(
                    boatRigidbody.position + transform.forward * rowInput * bothOarsForce * Time.fixedDeltaTime
                );
                boatMoved = true;
            }
        }

        if (!leftOarGrabbed && rightOarGrabbed)
        {
            float forwardBackInput = -leftVel.z;
            float downwardInput = -leftVel.y;

            if (forwardBackInput > 0.2f && downwardInput > 0.1f)
            {
                float rowInput = (forwardBackInput + downwardInput) / 2f;
                boatRigidbody.MoveRotation(
                    boatRigidbody.rotation * Quaternion.Euler(0, -rowInput * steeringForce * rotationMultiplier * Time.fixedDeltaTime, 0)
                );
                boatMoved = true;
            }
        }

        if (leftOarGrabbed && !rightOarGrabbed)
        {
            float forwardBackInput = -rightVel.z;
            float downwardInput = -rightVel.y;

            if (forwardBackInput > 0.2f && downwardInput > 0.1f)
            {
                float rowInput = (forwardBackInput + downwardInput) / 2f;
                boatRigidbody.MoveRotation(
                    boatRigidbody.rotation * Quaternion.Euler(0, rowInput * steeringForce * Time.fixedDeltaTime, 0)
                );
                boatMoved = true;
            }
        }

        if (boatMoved)
        {
            moveProvider.enabled = false;

            Vector3 worldPlayerPos = boatRigidbody.transform.TransformPoint(localPlayerOffset);
            Quaternion worldPlayerRot = boatRigidbody.rotation * localPlayerRotation;
            playerRig.SetPositionAndRotation(worldPlayerPos, worldPlayerRot);

            if (fishingRod != null)
            {
                fishingRod.position = boatRigidbody.transform.TransformPoint(localFishingRodOffset);
                fishingRod.rotation = boatRigidbody.rotation * localFishingRodRotation;
            }
        }
        else
        {
            moveProvider.enabled = true;
            RecalculateLocalOffset();
        }
    }
}