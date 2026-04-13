using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FollowDistance : MonoBehaviour
{
    public Transform cylindre;
    public float ropeLength = 4f;
    public float followSpeed = 10f;
    [HideInInspector] public bool reelActive = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (cylindre == null) return;
        if (reelActive) return; // ReelController gère la pause

        Vector3 direction = transform.position - cylindre.position;
        direction.y = 0f;
        direction.Normalize();

        Vector3 targetPos = cylindre.position + direction * ropeLength;
        targetPos.y = transform.position.y;

        rb.MovePosition(Vector3.Lerp(transform.position, targetPos, followSpeed * Time.fixedDeltaTime));
    }
}