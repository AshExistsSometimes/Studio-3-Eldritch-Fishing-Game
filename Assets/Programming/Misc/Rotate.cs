using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Header("Rotation Axes")]
    [Tooltip("Rotate around the local X axis")]
    public bool rotateX = false;

    [Tooltip("Rotate around the local Y axis")]
    public bool rotateY = false;

    [Tooltip("Rotate around the local Z axis")]
    public bool rotateZ = false;

    [Header("Rotation Settings")]
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 45f;

    void Update()
    {
        // Determine the local rotation vector based on toggled axes
        Vector3 rotationVector = new Vector3(
            rotateX ? 1f : 0f,
            rotateY ? 1f : 0f,
            rotateZ ? 1f : 0f
        );

        // Apply local-space rotation
        transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
