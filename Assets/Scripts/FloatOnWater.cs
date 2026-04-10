using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    public float waterLevel = 0f;      // Y position of your water surface
    public float buoyancyForce = 10f;  // how strong the float is
    public float damping = 0.5f;       // reduces bouncing

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // If boat is below water level → push it up
        if (transform.position.y < waterLevel)
        {
            float difference = waterLevel - transform.position.y;
            rb.AddForce(Vector3.up * buoyancyForce * difference, ForceMode.Force);

            // Damping to stop bouncing
            rb.velocity *= (1f - damping * Time.fixedDeltaTime);
        }
    }
}