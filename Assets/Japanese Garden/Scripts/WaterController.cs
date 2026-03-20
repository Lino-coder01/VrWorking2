using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class WaterController : MonoBehaviour
{
    public DynamicMoveProvider moveProvider;
    public float normalSpeed = 3f;
    public float waterSpeed = 1f;
    public float waterLevel = 0f;

    private bool playerInWater = false;
    private Transform playerTransform;

    void Start()
    {
        normalSpeed = moveProvider.moveSpeed;
    }

    void Update()
    {
        if (playerInWater && playerTransform != null)
        {
            Vector3 pos = playerTransform.position;
            if (pos.y < waterLevel)
            {
                pos.y = waterLevel;
                playerTransform.position = pos;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") ||
            other.gameObject.name.Contains("XR Origin"))
        {
            playerInWater = true;
            playerTransform = other.transform;
            moveProvider.moveSpeed = waterSpeed;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") ||
            other.gameObject.name.Contains("XR Origin"))
        {
            playerInWater = false;
            playerTransform = null;
            moveProvider.moveSpeed = normalSpeed;
        }
    }
}