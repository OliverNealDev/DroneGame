using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DronePickupController : MonoBehaviour
{
    private List<GameObject> pickupables = new List<GameObject>();
    private List<GameObject> pickups = new List<GameObject>();

    [SerializeField] private float posLerpStrength;
    [SerializeField] private float rotSlerpStrength;
    
    private Rigidbody rb;

    void Start()
    {
        pickupables.Clear();

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && pickupables.Count > 0 && pickups.Count == 0)
        {
            Pickup(pickupables[0]);
            Debug.Log(pickupables.Count);
        }
        else if (Input.GetKeyDown(KeyCode.E) && pickups.Count > 0)
        {
            Drop(pickups[0]);
            Debug.Log(pickups.Count);
        }

        if (pickups.Count > 0)
        {
            foreach (GameObject pickup in pickups)
            {
                // Update the position of the pickup to follow the drone
                //pickup.transform.position = transform.position + -transform.up * 2f;
                //pickup.transform.rotation = Quaternion.LookRotation(transform.forward);
                pickup.transform.position = Vector3.Lerp(pickup.transform.position, transform.position + (Vector3.down / 2), posLerpStrength * Time.deltaTime);
                pickup.transform.rotation = Quaternion.Slerp(pickup.transform.rotation, transform.rotation, rotSlerpStrength * Time.deltaTime);
                
                
                Debug.Log(pickup.transform.position);
            }
        }
    }

    void Pickup(GameObject pickupable)
    {
        pickups.Add(pickupable);
        pickupables.Remove(pickupable);
        //pickupable.transform.SetParent(transform);
        pickupable.GetComponent<Rigidbody>().useGravity = false;
        pickupable.GetComponent<Collider>().enabled = false;
    }
    
    void Drop(GameObject pickup)
    {
        pickups.Remove(pickup);
        //pickup.transform.SetParent(null);
        pickup.GetComponent<Rigidbody>().useGravity = true;
        pickup.GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pickupable"))
        {
            pickupables.Add(other.gameObject);
            Debug.Log("pickupable");
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pickupable"))
        {
            pickupables.Remove(other.gameObject);
            Debug.Log("pickupable removed");
        }
    }
}
