using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FishingLineArc : MonoBehaviour
{
    [Header("Références")]
    public Transform rodTip; // glisse Cylinder.007 ici

    [Header("Ligne de pêche")]
    public int arcPoints = 30;
    public float lineLength = 4f;
    public float gravity = 2f;
    public Color lineColor = Color.white;
    public float lineWidth = 0.015f;

    private XRGrabInteractable grabInteractable;
    private LineRenderer fishingLine;
    private bool isGrabbed = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        fishingLine = gameObject.AddComponent<LineRenderer>();
        fishingLine.positionCount = arcPoints;
        fishingLine.startWidth = lineWidth;
        fishingLine.endWidth = lineWidth * 0.5f;
        fishingLine.material = new Material(Shader.Find("Sprites/Default"));
        fishingLine.startColor = lineColor;
        fishingLine.endColor = lineColor;
        fishingLine.useWorldSpace = true;
        fishingLine.enabled = false;
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        fishingLine.enabled = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        fishingLine.enabled = false;
    }

    void Update()
    {
        if (!isGrabbed || rodTip == null) return;

        for (int i = 0; i < arcPoints; i++)
        {
            float t = i / (float)(arcPoints - 1);

            Vector3 point = rodTip.position
                + rodTip.forward * (t * lineLength);

            // Gravité simulée — tombe vers le bas
            point.y -= gravity * t * t;

            fishingLine.SetPosition(i, point);
        }
    }
}