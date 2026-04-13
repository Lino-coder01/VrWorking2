using UnityEngine;
using System.Collections.Generic;

public class SphereCollector : MonoBehaviour
{
    [Header("Tag des objets récupérables")]
    public string collectableTag = "Collectable";

    [Header("Récupération")]
    public Transform cylindre;
    public float retrieveDistance = 0.5f; // distance cylindre-sphere pour déposer

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
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        foreach (Collider col in obj.GetComponentsInChildren<Collider>())
            col.enabled = false;

        // ✅ Garder la position world space au moment de l'attachement
        Vector3 worldPos = obj.transform.position;
        Quaternion worldRot = obj.transform.rotation;

        obj.transform.SetParent(transform);

        // ✅ Remettre la position world space pour que l'objet reste visible
        obj.transform.position = worldPos;
        obj.transform.rotation = worldRot;

        attachedObjects.Add(obj);
        Debug.Log("Attaché : " + obj.name + " à pos " + worldPos);
    }

    void RetrieveAll()
    {
        foreach (GameObject obj in attachedObjects)
        {
            if (obj == null) continue;

            obj.transform.SetParent(null);

            // ✅ Spawner AU-DESSUS du cylindre avec un Y garanti sur l'eau
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

    void Update()
    {
        // Quand la sphère remonte près du cylindre → déposer les objets
        if (cylindre == null) return;

        float dist = Vector3.Distance(transform.position, cylindre.position);
        if (dist <= retrieveDistance && attachedObjects.Count > 0)
        {
            RetrieveAll();
        }
    }

    
}