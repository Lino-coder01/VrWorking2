using UnityEngine;

public class IgnoreWaterCollision : MonoBehaviour
{
    public Collider waterCollider; // drag ton water ici dans l'Inspector

    void Start()
    {
        Collider myCollider = GetComponent<Collider>();
        if (waterCollider != null && myCollider != null)
        {
            Physics.IgnoreCollision(myCollider, waterCollider, true);
        }
    }
}