using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float verticalSpeed = 3f;
    [SerializeField] private float acceleration = 2f;

    // New camera reference.
    public Transform cameraTransform;

    private Vector2 moveInput;
    private float verticalInput;

    [Header("Tilting Settings")]
    [SerializeField] private float maxTiltAngle = 20f;
    [SerializeField] private float tiltSpeed = 5f;

    [Header("Propeller Settings")]
    [SerializeField] private List<GameObject> propellers = new List<GameObject>();
    [SerializeField] private float propellerAnimSpeed = 3000f;
    [SerializeField] private float propellerAnimAcceleration = 100f;
    private float rotateAmount;

    private Quaternion targetRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.sleepThreshold = 0.0f;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        // Get camera relative horizontal movement.
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            movement += camForward;
        if (Input.GetKey(KeyCode.S))
            movement -= camForward;
        if (Input.GetKey(KeyCode.A))
            movement -= camRight;
        if (Input.GetKey(KeyCode.D))
            movement += camRight;

        if (movement != Vector3.zero)
            movement.Normalize();

        // The horizontal input is set in the xz plane.
        moveInput = new Vector2(movement.x, movement.z);

        // Vertical input.
        verticalInput = 0f;
        if (Input.GetKey(KeyCode.Space))
        {
            verticalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            verticalInput = -1f;
        }

        // Calculate tilting.
        float targetPitch = moveInput.y * maxTiltAngle;
        float targetRoll = -moveInput.x * maxTiltAngle;
        float currentYaw = 0;
        targetRotation = Quaternion.Euler(targetPitch, currentYaw, targetRoll);

        // Determine base propeller speed.
        float targetPropellerSpeed = propellerAnimSpeed;
        // If idle, reduce speed to 25%.
        if (verticalInput > 0)
        {
            targetPropellerSpeed *= 1.0f;
        }
        else if (verticalInput < 0)
        {
            targetPropellerSpeed *= 0.25f;
        }
        else
        {
            targetPropellerSpeed *= 0.5f;
        }
        
        float currentRotateAmount = rotateAmount;
        rotateAmount = Mathf.Lerp(currentRotateAmount, targetPropellerSpeed, propellerAnimAcceleration * Time.deltaTime);

        foreach (GameObject propeller in propellers)
        {
            propeller.transform.Rotate(0f, rotateAmount, 0f, Space.Self);
        }
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

        float currentVerticalVelocity = Mathf.Lerp(
            rb.linearVelocity.y,
            targetVerticalVelocity,
            Time.fixedDeltaTime * acceleration
        );

        rb.linearVelocity = new Vector3(currentHorizontalVelocity.x, currentVerticalVelocity, currentHorizontalVelocity.z);

        Quaternion newRotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            Time.fixedDeltaTime * tiltSpeed
        );

        rb.MoveRotation(newRotation);
    }
}