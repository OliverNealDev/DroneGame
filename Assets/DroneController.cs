using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float verticalSpeed = 3f;
    [SerializeField] private float acceleration = 2f;

    private Vector2 moveInput;
    private float verticalInput;

    [Header("Tilting Settings")]
    [SerializeField] private float maxTiltAngle = 20f;
    [SerializeField] private float tiltSpeed = 5f;

    private Quaternion targetRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.sleepThreshold = 0.0f;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        verticalInput = 0f;
        if (Input.GetKey(KeyCode.Space))
        {
            verticalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            verticalInput = -1f;
        }

        float targetPitch = moveInput.y * maxTiltAngle;
        float targetRoll = -moveInput.x * maxTiltAngle;
        //float currentYaw = transform.eulerAngles.y;
        float currentYaw = 0;

        targetRotation = Quaternion.Euler(targetPitch, currentYaw, targetRoll);
    }

    void FixedUpdate()
    {
        Vector3 targetHorizontalVelocity = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        float targetVerticalVelocity = verticalInput * verticalSpeed;

        Vector3 currentHorizontalVelocity = Vector3.Lerp(
            new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z),
            targetHorizontalVelocity,
            Time.fixedDeltaTime * acceleration
        );

        float currentVerticalVelocity;
        if (rb.useGravity)
        {
             currentVerticalVelocity = Mathf.Lerp(
                 rb.linearVelocity.y,
                 targetVerticalVelocity,
                 Time.fixedDeltaTime * acceleration
             );
        }
        else
        {
            currentVerticalVelocity = Mathf.Lerp(
                rb.linearVelocity.y,
                targetVerticalVelocity,
                Time.fixedDeltaTime * acceleration
            );
        }

        rb.linearVelocity = new Vector3(currentHorizontalVelocity.x, currentVerticalVelocity, currentHorizontalVelocity.z);

        Quaternion newRotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            Time.fixedDeltaTime * tiltSpeed
        );

        rb.MoveRotation(newRotation);
    }
}
