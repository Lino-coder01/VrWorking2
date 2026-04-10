using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HandleReel : MonoBehaviour
{
    public Transform cylindre;
    public Rigidbody sphereRb;
    public Transform handle;

    public XRSimpleInteractable handleInteractable;

    public float ropeLength = 4f;
    public float reelSpeed = 2f;
    public float followStrength = 80f;

    private float fixedX;
    private float fixedZ;

    private bool isHeld = false;

    private float lastAngle;

    private InputDevice leftDevice;

    void OnEnable()
    {
        handleInteractable.selectEntered.AddListener(OnGrab);
        handleInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        handleInteractable.selectEntered.RemoveListener(OnGrab);
        handleInteractable.selectExited.RemoveListener(OnRelease);
    }

    void Start()
    {
        fixedX = sphereRb.position.x;
        fixedZ = sphereRb.position.z;

        leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        lastAngle = handle.localEulerAngles.y;
    }

    void Update()
    {
        if (!isHeld) return;

        float currentAngle = handle.localEulerAngles.y;
        float delta = Mathf.DeltaAngle(lastAngle, currentAngle);
        lastAngle = currentAngle;

        ropeLength -= delta * 0.02f * reelSpeed;
        ropeLength = Mathf.Clamp(ropeLength, 0.5f, 10f);
    }

    void FixedUpdate()
    {
        if (cylindre == null || sphereRb == null) return;

        float targetY = cylindre.position.y - ropeLength;

        Vector3 targetPos = new Vector3(fixedX, targetY, fixedZ);

        Vector3 force = (targetPos - sphereRb.position) * followStrength;

        sphereRb.AddForce(force, ForceMode.Acceleration);

        sphereRb.position = new Vector3(fixedX, sphereRb.position.y, fixedZ);
        sphereRb.velocity = new Vector3(0f, sphereRb.velocity.y, 0f);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
        lastAngle = handle.localEulerAngles.y;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
    }
}