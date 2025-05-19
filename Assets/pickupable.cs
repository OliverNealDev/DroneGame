using UnityEngine;

public class pickupable : MonoBehaviour
{
    void Awake()
    {
        // Check for a Collider and add a BoxCollider if none exists.
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = false;
        }
        
        // Check for a Rigidbody and add one if missing.
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
        }
        
        // Set the object to the Pickupable layer.
        int layer = LayerMask.NameToLayer("Pickupable");
        if (layer != -1)
        {
            gameObject.layer = layer;
        }
        else
        {
            Debug.LogWarning("Pickupable layer not found. Please create a layer named Pickupable.");
        }
    }
}