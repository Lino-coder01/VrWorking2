using System.Collections.Generic;
using UnityEngine;

public class FloatAdvanced : MonoBehaviour
{
    [Header("Flottaison")]
    public float waterLevel = 0f; // hauteur Y de ta mesh eau
    public float floatHeight = 0.5f;
    public float liftStrength = 10f;
    public float damping = 2f;

    [Header("Floaters")]
    public List<Transform> floaterPoints = new List<Transform>();
    public float floatingPower = 20f;

    [Header("Drag")]
    public float underWaterDrag = 3f;
    public float underWaterAngularDrag = 1f;
    public float defaultDrag = 0f;
    public float defaultAngularDrag = 0.05f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float targetY = waterLevel + floatHeight;
        float depth = targetY - transform.position.y;
        float force = depth * liftStrength - rb.velocity.y * damping;
        rb.AddForce(Vector3.up * force, ForceMode.Acceleration);

        bool isUnderWater = false;

        foreach (var point in floaterPoints)
        {
            float diff = point.position.y - waterLevel;
            if (diff < 0)
            {
                rb.AddForceAtPosition(
                    Vector3.up * floatingPower * Mathf.Abs(diff),
                    point.position,
                    ForceMode.Force
                );
                isUnderWater = true;
            }
        }

        rb.drag = isUnderWater ? underWaterDrag : defaultDrag;
        rb.angularDrag = isUnderWater ? underWaterAngularDrag : defaultAngularDrag;
    }
}