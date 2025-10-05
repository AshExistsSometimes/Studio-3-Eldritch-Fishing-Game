using System.Collections;
using System.Collections.Generic;
using JetBrains.Rider.Unity.Editor;
using UnityEditor.Profiling;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BoatController : MonoBehaviour
{
    public Transform standingPoint;
    public float walkSpeed = 2f;
    public float runSpeed = 2f;
    public float acceleration = 2f;
    public float decceleration = 2f;

    private float currentSpeed = 0;
    private bool isMounted = false;

    private Vector3 verticalVelocity = Vector3.zero;

    private Transform driver;
    private CharacterController characterController;

    public PlayerMovement playerMovement { get; private set; }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();    
    }

    void Update()
    {
        if (!isMounted || driver == null)
        {
            return;
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        float vertical = Input.GetAxis("Vertical");

        float targetSpeed = 0f;
        
        if (vertical > 0.1f)
        {
            targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed,
            (currentSpeed < targetSpeed ? acceleration : decceleration) * Time.deltaTime);

        float horizontal = Input.GetAxis("Horizontal");

        if (Mathf.Abs(horizontal) > 0.1f)
        {
            transform.Rotate(Vector3.up * horizontal * 60f * Time.deltaTime);
        }

        Vector3 move = transform.forward * currentSpeed + verticalVelocity;
        characterController.Move(move * Time.deltaTime);
    }

    public void Mount(Transform player)
    {
        if (isMounted)
        {
            return;
        }

        isMounted = true;
        driver = player;

        driver.SetParent(standingPoint);
        driver.localPosition = Vector3.zero;
        driver.localRotation = Quaternion.identity;

        playerMovement.enabled = false;
    }

    public void Dismount()
    {
        if (!isMounted || driver == null)
        {
            return;
        }

        driver.SetParent(null);
        driver.position = transform.position - transform.right * 3f + Vector3.up * 1f;

        playerMovement.enabled = true;

        isMounted = false;
        driver = null;
        currentSpeed = 0f;
    }
}
