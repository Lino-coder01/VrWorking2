using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    [Header("Son")]
    public AudioClip footstepClip;
    public float volume = 1f;
    public float timeBetweenSteps = 0.5f;

    [Header("Filtre")]
    public string playerTag = "Player"; // tag sur ton XR Rig

    private AudioSource audioSource;
    private float timer = 0f;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D car le son vient du sol
        audioSource.volume = volume;
    }

    void Update()
    {
        timer += Time.deltaTime;
    }

    void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag(playerTag)) return;
        if (footstepClip == null) return;
        if (timer < timeBetweenSteps) return;

        timer = 0f;
        audioSource.PlayOneShot(footstepClip, volume);
    }
}