using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using InputDevice = UnityEngine.XR.InputDevice;
using CommonUsages = UnityEngine.XR.CommonUsages;

public class ReelController : MonoBehaviour
{
    [Header("Références")]
    public XRSimpleInteractable reelInteractable;
    public Transform balle;
    public Transform Cylinder;
    public FollowDistance followDistance;
    public XRGrabInteractable canneAPeche;

    [Header("Limites")]
    public float minRopeLength = 0.5f;
    public float maxRopeLength = 5f;
    public float reelSensitivity = 0.5f;

    [Header("Debug UI")]
    public TMP_Text debugText;

    private InputDevice rightDevice;
    private bool reelGrabbed = false;

    private Rigidbody balleRb;

    void Start()
    {
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        balleRb = balle.GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        reelInteractable.selectEntered.AddListener(e => reelGrabbed = true);
        reelInteractable.selectExited.AddListener(e => reelGrabbed = false);
    }

    void OnDisable()
    {
        reelInteractable.selectEntered.RemoveAllListeners();
        reelInteractable.selectExited.RemoveAllListeners();
    }

    void FixedUpdate()
    {
        Vector2 joystick = Vector2.zero;
        rightDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out joystick);

        bool canneHeld = canneAPeche != null && canneAPeche.isSelected;
        bool joystickActif = reelGrabbed && canneHeld && Mathf.Abs(joystick.y) >= 0.2f;

        if (followDistance != null)
            followDistance.reelActive = joystickActif;

        if (debugText != null)
        {
            Vector3 relPos = Cylinder != null
                ? balle.position - Cylinder.position
                : balle.position;

            debugText.text =
                "rel X      : " + relPos.x.ToString("F3") + "\n" +
                "rel Y      : " + relPos.y.ToString("F3") + "\n" +
                "rel Z      : " + relPos.z.ToString("F3") + "\n" +
                "Joystick Y : " + joystick.y.ToString("F3") + "\n" +
                "Grabbed    : " + reelGrabbed + "\n" +
                "Canne held : " + canneHeld;
        }

        if (!joystickActif) return;

        // Clamp basé sur la position Y du cylindre
        float cylindreY = Cylinder != null ? Cylinder.position.y : 0f;
        float newY = balle.position.y + joystick.y * reelSensitivity * Time.fixedDeltaTime;
        newY = Mathf.Clamp(newY, cylindreY - maxRopeLength, cylindreY - minRopeLength);

        balleRb.MovePosition(new Vector3(balle.position.x, newY, balle.position.z));
    }
}