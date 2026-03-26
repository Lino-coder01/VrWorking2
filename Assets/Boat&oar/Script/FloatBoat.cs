using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatBoat : MonoBehaviour
{
    public Transform waterReference; // objet qui donne la hauteur de l'eau
    public float floatHeight = 0.5f;
    public float liftStrength = 10f;
    public float damping = 2f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (waterReference == null) return;

        float targetY = waterReference.position.y + floatHeight;
        float depth = targetY - transform.position.y;
        float force = depth * liftStrength - rb.velocity.y * damping;
        rb.AddForce(Vector3.up * force, ForceMode.Acceleration);
    }
}
