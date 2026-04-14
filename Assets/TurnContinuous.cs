using UnityEngine;
using UnityEngine.InputSystem;

public class SmoothTurnJoystick : MonoBehaviour
{
    public InputActionProperty turnAction;
    public float rotationSpeed = 90f;
    public float deadZone = 0.2f;

    void OnEnable()
    {
        turnAction.action.Enable();
    }

    void OnDisable()
    {
        turnAction.action.Disable();
    }

    void Update()
    {
        Vector2 input = turnAction.action.ReadValue<Vector2>();

        float turnInput = input.x;

        if (Mathf.Abs(turnInput) < deadZone)
            return;

        float rotation = turnInput * rotationSpeed * Time.deltaTime;

        transform.Rotate(0f, rotation, 0f);
    }
}