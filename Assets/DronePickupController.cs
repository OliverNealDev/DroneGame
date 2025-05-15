using System.Collections.Generic;
using UnityEngine;

public class DronePickupController : MonoBehaviour
{
    private List<GameObject> pickupables = new List<GameObject>();
    private List<GameObject> pickups = new List<GameObject>();

    void Start()
    {
        pickupables.Clear();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && pickupables.Count > 0 && pickups.Count == 0)
        {
            Pickup(pickupables[0]);
        }
        else if (Input.GetKeyDown(KeyCode.E) && pickups.Count > 0)
        {
            Drop(pickups[0]);
        }

        if (pickups.Count > 0)
        {
            foreach (GameObject pickup in pickups)
            {
                // Update the position of the pickup to follow the drone
                pickup.transform.position = transform.position + -transform.up * 2f;
                pickup.transform.rotation = Quaternion.LookRotation(transform.forward);
            }
        }
    }

    void Pickup(GameObject pickupable)
    {
        pickups.Add(pickupable);
    }
    
    void Drop(GameObject pickup)
    {
        pickups.Remove(pickup);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickupable"))
        {
            pickupables.Add(other.gameObject);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickupable"))
        {
            pickupables.Remove(other.gameObject);
        }
    }
}
