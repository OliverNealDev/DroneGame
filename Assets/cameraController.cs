using UnityEngine;

public class cameraController : MonoBehaviour
{
    public Transform target;                       // The drone's transform.
    public float distance = 10f;                     // Default distance from the target.
    public float xSpeed = 120f;                      // Mouse sensitivity for horizontal movement.
    public float ySpeed = 80f;                       // Mouse sensitivity for vertical movement.
    public float yMinLimit = 10f;                    // Minimum vertical angle.
    public float yMaxLimit = 80f;                    // Maximum vertical angle.

    // Zoom parameters.
    public float zoomSpeed = 5f;
    public float minDistance = 5f;
    public float maxDistance = 20f;

    private float yaw = 0f;
    private float pitch = 45f;                       // Default pitch is set to look slightly down from above.

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera target not set.");
            return;
        }

        // Lock the cursor and hide it.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize angles based on starting offset.
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = Mathf.Clamp(pitch, yMinLimit, yMaxLimit);
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Handle zoom input using the scroll wheel.
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        distance -= scrollInput * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Always use mouse movement for rotation.
        yaw += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
        pitch += Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.position = position;
        // Use a constant up vector to avoid inversion.
        transform.LookAt(target.position, Vector3.up);
    }
}