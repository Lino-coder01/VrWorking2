using UnityEngine;

public class FishingLine : MonoBehaviour
{
    public Transform rodTip;
    public float moveSpeed = 2f;

    private Vector3 positionOffset = Vector3.zero;
    private Vector3 initialPosition;
    private Vector3 lastRodTipPosition;

    void Start()
    {
        initialPosition = transform.position;
        if (rodTip != null)
            lastRodTipPosition = rodTip.position;
    }

    void Update()
    {
        if (rodTip != null)
        {
            Vector3 rodDelta = rodTip.position - lastRodTipPosition;
            // Seulement X et Z du delta de la canne
            positionOffset.x += rodDelta.x;
            positionOffset.z += rodDelta.z;
            lastRodTipPosition = rodTip.position;
        }

        if (Input.GetKey(KeyCode.RightArrow)) positionOffset.x += moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow)) positionOffset.x -= moveSpeed * Time.deltaTime;

        // On applique SEULEMENT X et Z, le Y reste géré par Buoyancy
        transform.position = new Vector3(
            initialPosition.x + positionOffset.x,
            transform.position.y, // Y intouché
            initialPosition.z + positionOffset.z
        );
    }
}