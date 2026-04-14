using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using InputDevice = UnityEngine.XR.InputDevice;
using CommonUsages = UnityEngine.XR.CommonUsages;

public class CylinderController : MonoBehaviour
{
    [Header("Références")]
    public XRSimpleInteractable reelInteractable;
    public XRGrabInteractable canneAPeche;
    public Transform balle;
    public FloatAdvanced floatAdvanced; // ✅ AJOUTE ÇA

    [Header("Mouvement cylindre XZ")]
    public float moveSpeed = 2f;
    public float cylinderFixedY = 0.5f;

    [Header("Mouvement balle Y")]
    public float reelSensitivity = 1f;
    public float minDepth = 0.5f;
    public float maxDepth = 5f;
    public float groundY = -4f;

    private InputDevice rightDevice;
    private bool reelGrabbed = false;
    private Rigidbody balleRb;
    private Rigidbody cylinderRb;
    private float currentDepth = 2f;

    private Vector3 lastCannePos;

    void Start()
    {
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        balleRb = balle.GetComponent<Rigidbody>();
        cylinderRb = GetComponent<Rigidbody>();
        currentDepth = Mathf.Clamp(
            transform.position.y - balle.position.y,
            minDepth, maxDepth
        );
        lastCannePos = canneAPeche.transform.position;
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
        bool handleMode = reelGrabbed && canneHeld;
        bool joystickActif = joystick.magnitude >= 0.2f;
        bool movingWhileHoldingCanne = canneHeld && !handleMode && joystickActif;

        //Baisser/monter la sphere
        if (handleMode)
        {
            if (floatAdvanced != null) floatAdvanced.overrideY = true;

            if (Mathf.Abs(joystick.y) >= 0.2f)
            {
                currentDepth += joystick.y * reelSensitivity * Time.fixedDeltaTime;
                currentDepth = Mathf.Clamp(currentDepth, minDepth, maxDepth);
            }

            cylinderRb.constraints = RigidbodyConstraints.FreezePosition
                                   | RigidbodyConstraints.FreezeRotation;
        }
        else if (movingWhileHoldingCanne)
        {
            if (floatAdvanced != null) floatAdvanced.overrideY = false;

            // Bouge XZ en suivant le joystick
            cylinderRb.constraints = RigidbodyConstraints.FreezeRotation
                                   | RigidbodyConstraints.FreezePositionY;
           

            // APRÈS
            Camera cam = Camera.main;
            Vector3 camForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
            Vector3 move = (camForward * joystick.y + camRight * joystick.x) * moveSpeed * Time.fixedDeltaTime;
            Vector3 newPos = cylinderRb.position + move;
            newPos.y = cylinderFixedY;
            cylinderRb.MovePosition(newPos);
        }
        else if (canneHeld && !joystickActif)
        {
            if (floatAdvanced != null) floatAdvanced.overrideY = false;
            cylinderRb.constraints = RigidbodyConstraints.FreezeRotation
                                   | RigidbodyConstraints.FreezePositionY;

            Vector3 delta = canneAPeche.transform.position - lastCannePos;
            delta.y = 0f;
            Vector3 newPos = cylinderRb.position + delta;
            newPos.y = cylinderFixedY;
            cylinderRb.MovePosition(newPos);

            cylinderRb.velocity = Vector3.zero;
            cylinderRb.angularVelocity = Vector3.zero;
        }
        else
        {
            // Rien en main
            if (floatAdvanced != null) floatAdvanced.overrideY = false;

            if (joystickActif)
            {
                cylinderRb.constraints = RigidbodyConstraints.FreezeRotation
                                       | RigidbodyConstraints.FreezePositionY;
                
                // APRÈS
                Camera cam = Camera.main;
                Vector3 camForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
                Vector3 camRight = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
                Vector3 move = (camForward * joystick.y + camRight * joystick.x) * moveSpeed * Time.fixedDeltaTime;
                Vector3 newPos = cylinderRb.position + move;
                newPos.y = cylinderFixedY;
                cylinderRb.MovePosition(newPos);
            }
            else
            {
                cylinderRb.constraints = RigidbodyConstraints.FreezePosition
                                       | RigidbodyConstraints.FreezeRotation;

                // ✅ Annuler toute vélocité résiduelle
                cylinderRb.velocity = Vector3.zero;
                cylinderRb.angularVelocity = Vector3.zero;
            }
        }

        // Balle toujours sous le cylindre
        float targetBalleY = Mathf.Max(cylinderRb.position.y - currentDepth, groundY);
        balleRb.MovePosition(new Vector3(
            cylinderRb.position.x,
            targetBalleY,
            cylinderRb.position.z
        ));
        lastCannePos = canneAPeche.transform.position;
    }
}