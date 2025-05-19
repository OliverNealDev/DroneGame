using UnityEngine;

public class DroneCameraController : MonoBehaviour
{
    // The drone's transform to follow.
    public Transform target;

    // Distance to keep from the target.
    public float distance = 5.0f;
    // Minimum and maximum distance from the target.
    public float minDistance = 2.0f;
    public float maxDistance = 15.0f;

    // Mouse sensitivity for X and Y axes.
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    // Limits for vertical angle (Y-axis).
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    // Smoothing for zoom.
    public float zoomSmoothTime = 0.1f; // Smoothing only for zoom

    // Current rotation angles.
    private float x = 0.0f;
    private float y = 0.0f;

    // Current distance (for zooming).
    private float currentDistance;
    private float targetDistance;

    // Velocity for zoom smoothing.
    private float distanceVelocity;

    // Initialization
    void Start()
    {
        // Get the initial Euler angles of the camera.
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Set initial distances.
        currentDistance = distance;
        targetDistance = distance;

        // Lock the cursor to the center of the game window and make it invisible.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Make sure there is a target.
        if (!target)
        {
            Debug.LogWarning("Camera Controller: No target assigned. Please assign a target in the Inspector.");
            // Optionally, create a dummy target to avoid null reference errors.
            GameObject dummyTarget = new GameObject("CameraTarget_Dummy");
            target = dummyTarget.transform;
        }
    }

    // Called after all Update functions have been called.
    // This is good for camera updates, as it ensures the target has moved before the camera updates.
    void LateUpdate()
    {
        // If no target, do nothing.
        if (!target)
            return;

        // Get mouse input for rotation.
        // The '-' sign for y is because mouse Y movement is often inverted for camera controls.
        x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

        // Clamp the vertical angle.
        y = ClampAngle(y, yMinLimit, yMaxLimit);

        // Calculate the desired rotation based on mouse input.
        Quaternion rotation = Quaternion.Euler(y, x, 0);

        // Handle zooming with the mouse scroll wheel.
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetDistance -= scroll * 5; // Adjust scroll speed as needed
        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);

        // Smoothly adjust the current distance towards the target distance.
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceVelocity, zoomSmoothTime);

        // Calculate the desired camera position.
        // The camera is positioned 'currentDistance' units behind the target, rotated by 'rotation'.
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -currentDistance);
        Vector3 desiredPosition = rotation * negDistance + target.position;

        // Directly set the camera's position and rotation for immediate feedback.
        transform.rotation = rotation;
        transform.position = desiredPosition;

        // Make the camera look at the target (this is now implicitly handled by setting rotation and position based on the target).
        // However, if you want to ensure it *always* looks directly at the target's center, even if the pivot is slightly off,
        // you can uncomment the line below. But with the current setup, it should align correctly.
        // transform.LookAt(target);


        // Allow cursor unlocking with the Escape key (optional).
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        // Allow cursor re-locking by clicking the game window (optional).
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
             Cursor.lockState = CursorLockMode.Locked;
             Cursor.visible = false;
        }
    }

    // Helper function to clamp an angle between a min and max value.
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
