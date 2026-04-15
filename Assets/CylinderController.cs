using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using InputDevice = UnityEngine.XR.InputDevice;
using CommonUsages = UnityEngine.XR.CommonUsages;

//Controls a fishing rod mechanic : a cylinder (reel) on the surface moves in XZ,
// and a ball (hook) moves vertically below it.
public class CylinderController : MonoBehaviour
{
    [Header("Références")]
    public XRSimpleInteractable reelInteractable;
    public XRGrabInteractable canneAPeche;
    public Transform balle;

    // controls whether the ball floats or sink
    public FloatAdvanced floatAdvanced; 

    [Header("Mouvement cylindre XZ")]
    // How ast the cylinder moves
    public float moveSpeed = 2f;
    //The Y the cylinder is always locked to
    public float cylinderFixedY = 0.5f; 

    [Header("Mouvement balle Y")]
    // How fast the joystick raises/lowers the ball
    public float reelSensitivity = 1f; 
    
    //Min/max distance the ball can be below the cylinder
    public float minDepth = 0.5f;
    public float maxDepth = 5f;
    
    //floor 
    public float groundY = -4f;

    private InputDevice rightDevice;
    private bool reelGrabbed = false;
    private Rigidbody balleRb;
    private Rigidbody cylinderRb;
    // current distance btw cylinder and ball
    private float currentDepth = 2f; 

    // Previous frame position of the rod, used to calculate movement delta
    private Vector3 lastCannePos; 
    /* 
     * We get the right hand XR device for joystick input, the rigidbody reference
     * Calculates initial depth based on actual distance between cylinder and ball
     */
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

    //Subscribes/unsubscribes to the reel grab events to track reelGrabbed state
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

        //player holding the rod
        bool canneHeld = canneAPeche != null && canneAPeche.isSelected; 
        //holding rod and reel 
        bool handleMode = reelGrabbed && canneHeld;
        //joystick used
        bool joystickActif = joystick.magnitude >= 0.2f;
        //canne held, no handle and joystick active
        bool movingWhileHoldingCanne = canneHeld && !handleMode && joystickActif;

        //Higher/Lower the sphere
        //Cylinder freezed, ball only moves vertically
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
        //Movement relative to the camera direction
        //balls floats normally,
        else if (movingWhileHoldingCanne)
        {
            if (floatAdvanced != null) floatAdvanced.overrideY = false;

            // Move XZ depending on joystick, only right and left 
            cylinderRb.constraints = RigidbodyConstraints.FreezeRotation
                                   | RigidbodyConstraints.FreezePositionY;
           
            Camera cam = Camera.main;
            Vector3 camForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
            Vector3 move = (camForward * joystick.y + camRight * joystick.x) * moveSpeed * Time.fixedDeltaTime;
            Vector3 newPos = cylinderRb.position + move;
            newPos.y = cylinderFixedY;
            cylinderRb.MovePosition(newPos);
        }
        //Cylinder follows the rod in XZ
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
            //Nothing in hand
            if (floatAdvanced != null) floatAdvanced.overrideY = false;

            //moves cylinder freely in XZ 
            if (joystickActif)
            {
                cylinderRb.constraints = RigidbodyConstraints.FreezeRotation
                                       | RigidbodyConstraints.FreezePositionY;
                
                Camera cam = Camera.main;
                Vector3 camForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
                Vector3 camRight = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
                Vector3 move = (camForward * joystick.y + camRight * joystick.x) * moveSpeed * Time.fixedDeltaTime;
                Vector3 newPos = cylinderRb.position + move;
                newPos.y = cylinderFixedY;
                cylinderRb.MovePosition(newPos);
            }
            //If joystick idle full freeze and zero velocity to stop any drift
            else
            {
                cylinderRb.constraints = RigidbodyConstraints.FreezePosition
                                       | RigidbodyConstraints.FreezeRotation;

                cylinderRb.velocity = Vector3.zero;
                cylinderRb.angularVelocity = Vector3.zero;
            }
        }

        // Ball always under cylinder
        float targetBalleY = Mathf.Max(cylinderRb.position.y - currentDepth, groundY);
        balleRb.MovePosition(new Vector3(
            cylinderRb.position.x,
            targetBalleY,
            cylinderRb.position.z
        ));
        lastCannePos = canneAPeche.transform.position;
    }
}