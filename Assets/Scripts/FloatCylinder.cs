using System.Collections.Generic;
using UnityEngine;

public class FloatAdvanced : MonoBehaviour
{
    [Header("Flottaison")]
    public float waterLevel = 0f;
    public float floatHeight = 0.5f; //height above water

    [Header("Drag")]
    public float underWaterDrag = 3f; //Resistance under water
    public float underWaterAngularDrag = 1f;
    public float defaultDrag = 0f; //resistance outside of water
    public float defaultAngularDrag = 0.05f;

    [HideInInspector] public bool overrideY = false;

    private Rigidbody rb;

    //We select the Rigidbody and freeze akk rotations and Y physic
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    void FixedUpdate()
    {
        float targetY = waterLevel + floatHeight; //Y destination where the object should float

        if (!overrideY)
        {
            // Forcer Y exactement, annuler toute vélocité verticale
            Vector3 pos = rb.position;
            pos.y = targetY;
            rb.MovePosition(pos);
        }

        // Toujours annuler vélocité Y et angulaire
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.angularVelocity = Vector3.zero; // ✅ stop mouvement circulaire

        bool isUnderWater = rb.position.y < waterLevel;
        rb.drag = isUnderWater ? underWaterDrag : defaultDrag;
        rb.angularDrag = isUnderWater ? underWaterAngularDrag : defaultAngularDrag;
    }
}