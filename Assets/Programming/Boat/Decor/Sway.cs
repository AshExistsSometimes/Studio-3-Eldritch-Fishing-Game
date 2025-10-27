using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    public float swaySpeed = 0.5f;
    public float swayAmplitude = 10f;
    public Vector3 swayAxis = Vector3.right;

    private Vector3 originalRotation;

    void Start()
    {
        originalRotation = transform.localEulerAngles;
    }

    void Update()
    {
        // Calculate the sway angle
        float swayAngle = Mathf.Sin(Time.time * swaySpeed) * swayAmplitude;

        // Rotate the object
        transform.Rotate(swayAxis, swayAngle * Time.deltaTime);

        // Reset the rotation to its original value
        // transform.localEulerAngles = originalRotation;
    }
}

