using UnityEngine;

public class IgnoreBoatCollision : MonoBehaviour
{
    public Collider boatCollider; // 👉 assigné manuellement
    private Collider sphereCollider;

    void Start()
    {
        sphereCollider = GetComponent<Collider>();

        if (boatCollider != null && sphereCollider != null)
        {
            Physics.IgnoreCollision(sphereCollider, boatCollider);
        }
        else
        {
            Debug.LogWarning("Boat or Sphere collider missing!");
        }
    }
}