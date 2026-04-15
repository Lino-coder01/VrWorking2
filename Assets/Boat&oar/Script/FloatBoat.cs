using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  
 *  The boat continuously tries to reach the height waterReference.y + floatHeight
 *  If it goes too low, the script pushes it up; if it goes too high, 
 *  the negative depth pulls it back down, while damping smooths the motion
*/
public class FloatBoat : MonoBehaviour
{
    public Transform waterReference; // Water Height
    public float floatHeight = 0.5f; // distance which the boat will sit above the waterReference
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
        //Calculate the boat's depth
        float depth = targetY - transform.position.y;
        //Calculate the upward force and we add damping to reduce the bouncing effect
        float force = depth * liftStrength - rb.velocity.y * damping; 
        rb.AddForce(Vector3.up * force, ForceMode.Acceleration);
    }
}
