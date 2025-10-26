using UnityEngine;

public class BoatSteeringWheel : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the BoatController controlling mount state.")]
    public BoatController boatController;

    [Header("Rotation Settings")]
    [Tooltip("How quickly the object rotates toward the target angle (degrees per second).")]
    public float rotationSpeed = 90f;

    [Tooltip("Maximum local rotation angle in degrees on the Y-axis.")]
    public float maxRotationAngle = 90f;

    [Tooltip("Input axis name for horizontal turning (e.g., 'Horizontal').")]
    public string inputAxis = "Horizontal";

    // Internal state
    private float targetLocalYRotation = 0f;
    private float currentLocalYRotation = 0f;

    private void Update()
    {
        // Only active if the player is mounted
        if (boatController == null || !boatController.isMounted)
            return;

        // Get horizontal input (-1 for left, 1 for right, 0 for neutral)
        float input = Input.GetAxisRaw(inputAxis);

        // Determine target local Y rotation
        if (input < 0f) // Turning left
            targetLocalYRotation = -maxRotationAngle;
        else if (input > 0f) // Turning right
            targetLocalYRotation = maxRotationAngle;
        else // No input — return to center
            targetLocalYRotation = 0f;

        // Smoothly move current local rotation toward the target
        currentLocalYRotation = Mathf.MoveTowards(
            currentLocalYRotation,
            targetLocalYRotation,
            rotationSpeed * Time.deltaTime
        );

        // Apply as local rotation (maintains local space orientation)
        Vector3 localEuler = transform.localEulerAngles;
        localEuler.y = currentLocalYRotation;
        transform.localEulerAngles = localEuler;
    }
}
