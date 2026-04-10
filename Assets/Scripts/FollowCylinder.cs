using UnityEngine;

public class FollowDistance : MonoBehaviour
{
    public Transform cylindre;
    public float ropeLength = 4f;
    public float followSpeed = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (cylindre == null) return;

        // 🔥 direction uniquement sur XZ
        Vector3 direction = transform.position - cylindre.position;
        direction.y = 0f;
        direction.Normalize();

        Vector3 targetPos = cylindre.position + direction * ropeLength;
        targetPos.y = transform.position.y; // ou cylindre.position.y si tu veux coller

        Vector3 force = (targetPos - transform.position) * followSpeed;

        force.y = 0f; // 🔥 on bloque totalement Y

        rb.MovePosition(Vector3.Lerp(transform.position, targetPos, followSpeed * Time.fixedDeltaTime));
    }

    public void SetRopeLength(float newLength)
    {
        ropeLength = Mathf.Max(0.1f, newLength);
    }
}