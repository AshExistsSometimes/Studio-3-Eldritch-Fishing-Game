using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    public bool canMove = true;

    [Header ("Head Bob Settings")]
    public Transform headTransform;
    public bool enableHeadbob = true;
    public float headbobSpeed = 6f;
    public float headbobAmount = 0.05f;
    [HideInInspector]
    public float defaultheadbobSpeed;
    private float headbobTimer = 0f;
    private Vector3 defaultHeadPos;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        defaultheadbobSpeed = headbobSpeed;
        if (headTransform != null)
        {
            defaultHeadPos = headTransform.localPosition;
        }
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(InputManager.GetKeyCode("Sprint"));
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetKeyDown(InputManager.GetKeyCode("Jump")) && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            HandleHeadbob();
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }





    void HandleHeadbob()
    {
        if (headTransform == null) return;

        // Check if player is moving (using WASD keys)
        bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;

        if (isMoving)
        {
            headbobTimer += Time.deltaTime * headbobSpeed;
            float bobOffset = Mathf.Sin(headbobTimer) * headbobAmount;
            headTransform.localPosition = defaultHeadPos + new Vector3(0f, bobOffset, 0f);
        }
        else
        {
            // Reset headbob
            headbobTimer = 0f;
            headTransform.localPosition = Vector3.Lerp(headTransform.localPosition, defaultHeadPos, Time.deltaTime * headbobSpeed);
        }
    }
}