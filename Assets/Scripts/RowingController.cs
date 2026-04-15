using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using static UnityEngine.Rendering.DebugUI.Table;

public class RowingController : MonoBehaviour
{
    //The two grabbable oar handles
    public XRSimpleInteractable leftOar;
    public XRSimpleInteractable rightOar;

    public Rigidbody boatRigidbody;
    
    //Rowing and rotation speed
    public float rowingForce = 10f;
    public float steeringForce = 12000f;
    public float rotationMultiplier = 2f;
    
    //The force is calculated by how fast a players moves his hand in the right direction
    public float bothOarsForce = 10f;

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    private bool leftOarGrabbed;
    private bool rightOarGrabbed;

    public Transform playerRig;
    public Transform fishingRod;

    public DynamicMoveProvider moveProvider;

    //Saved positions relative to the boat
    private Vector3 localPlayerOffset;
    private Quaternion localPlayerRotation;

    private Vector3 localFishingRodOffset;
    private Quaternion localFishingRodRotation;

    /*
     * Gets both hand XR devices for velocity tracking
     * Calls RecalculateLocalOffset() to save initial relative positions
     */
    void Start()
    {
        //This gives a 3D vector in metres per second representing how fast the controller is moving in world space
        leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        RecalculateLocalOffset();
    }

    /*
     * Saves the player and fishing rod positions relative to the boat, 
     * so when the boat moves/rotates, everything can be repositioned correctly in world space
     */
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

    //Reads real physical velocity of both controllers every physics frame.
    void FixedUpdate()
    {
        leftDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVel);
        rightDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightVel);

        bool boatMoved = false;

        // Both oars grabbed  boat moves forward
        if (leftOarGrabbed && rightOarGrabbed)
        {
            Vector3 avgVelocity = (leftVel + rightVel) / 2f;
            //negation because the gesture moves towards the player
            float forwardBackInput = -avgVelocity.z; 
            float downwardInput = -avgVelocity.y;

            //Both thresholds must be exceeded simultaneously
            //this ensures it only triggers on a real rowing gesture, not accidental hand movement
            if (forwardBackInput > 0.2f && downwardInput > 0.1f)
            {
                /* intensity of the rowing gesture
                 * P new​ = P old​ + f^ ​× rowInput × F × Δt
                 * Pnew​New boat positionPoldP_{old}
                 * Pold​Current boat positionf^\hat{f}
                 * f^​Normalised forward direction vectorrowInputrowInput
                 * rowInputGesture intensity scalarFF
                 * FbothOarsForce constantΔt\Delta t
                 * ΔtTime.fixedDeltaTime
                 */
                float rowInput = (forwardBackInput + downwardInput) / 2f;
                boatRigidbody.MovePosition(
                    boatRigidbody.position + transform.forward * rowInput * bothOarsForce * Time.fixedDeltaTime
                );
                boatMoved = true;
            }
        }
        //Only right oar grabbed
        if (!leftOarGrabbed && rightOarGrabbed)
        {
            float forwardBackInput = -leftVel.z;
            float downwardInput = -leftVel.y;

            if (forwardBackInput > 0.2f && downwardInput > 0.1f)
            {
                //MovePosition bypasses normal force/acceleration physics
                //it teleports the rigidbody kinematically*
                //θ=rowInput×steeringForce×Δt(degrees/frame)
                float rowInput = (forwardBackInput + downwardInput) / 2f;
                boatRigidbody.MoveRotation(
                    boatRigidbody.rotation * Quaternion.Euler(0, -rowInput * steeringForce * rotationMultiplier * Time.fixedDeltaTime, 0)
                );
                boatMoved = true;
            }
        }
        // Only left oar grabbed
        if (leftOarGrabbed && !rightOarGrabbed)
        {
            float forwardBackInput = -rightVel.z;
            float downwardInput = -rightVel.y;

            if (forwardBackInput > 0.2f && downwardInput > 0.1f)
            {
                float rowInput = (forwardBackInput + downwardInput) / 2f;
                boatRigidbody.MoveRotation(
                    boatRigidbody.rotation * Quaternion.Euler(0, rowInput * steeringForce * rotationMultiplier * Time.fixedDeltaTime, 0)
                );
                boatMoved = true;
            }
        }

        //After movement
        //Disables XR locomotion so it doesn't fight the boat movement
        //Repositions player and fishing rod using the saved local offsets converted back to world space
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
        //Re enables normal XR movement and updates offsets in case the player moved freely.
        else
        {
            moveProvider.enabled = true;
            RecalculateLocalOffset();
        }
    }
}