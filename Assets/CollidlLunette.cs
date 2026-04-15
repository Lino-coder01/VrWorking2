using UnityEngine;
using System.Collections.Generic;

public class SphereCollector : MonoBehaviour
{
    [Header("Tag des objets récupérables")]
    public string collectableTag = "Collectable"; //tag for grabbable objects

    [Header("Récupération")]
    public Transform cylindre; //Surface of fishing tip
    public float retrieveDistance = 0.5f; // distance cylindresphere to put

    private List<GameObject> attachedObjects = new List<GameObject>();

    public float waterLevel = 9.8f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(collectableTag)) return;
        if (attachedObjects.Contains(other.gameObject)) return;

        AttachObject(other.gameObject);
    }
 
    void AttachObject(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Parent direct va suivre parfaitement la sphère
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        attachedObjects.Add(obj);

        Debug.Log("Objet attaché : " + obj.name);
    }

    void RetrieveAll()
    {
        foreach (GameObject obj in attachedObjects)
        {

            if (obj == null) continue;

            //Détache les objets connecté à la sphère
            obj.transform.SetParent(null);

            // Spawner  au dessus du cylindre avec un Y garanti sur l'eau
            Vector3 dropPos = new Vector3(
                cylindre.position.x + Random.Range(-0.2f, 0.2f),
                cylindre.position.y + 0.3f, // ← au-dessus du cylindre, pas waterLevel fixe
                cylindre.position.z + Random.Range(-0.2f, 0.2f)
            );
            obj.transform.position = dropPos;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
            }

            foreach (Collider col in obj.GetComponentsInChildren<Collider>())
                col.enabled = true;
        }

        attachedObjects.Clear();
    }

    // For each frame we check if the sphere is enough close of the cylinder and that it transports objects
    void Update()
    {
        if (cylindre == null) return;

        float dist = Vector3.Distance(transform.position, cylindre.position);
        if (dist <= retrieveDistance && attachedObjects.Count > 0)
        {
            RetrieveAll();
        }
    }

    
}