using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(CharacterController))]
public class BoatController : MonoBehaviour
{
    public GameObject player;
    public Camera playerCamera;
    public Transform standingPoint;
    public FishingRod rod;
    public float walkSpeed = 2f;
    public float runSpeed = 2f;
    public float acceleration = 2f;
    public float decceleration = 2f;
    public float turnSpeed = 2f;

    private float currentSpeed = 0;
    public float fuelDepletionTimer = 0;
    [HideInInspector] public bool isMounted = false;

    private Vector3 verticalVelocity = Vector3.zero;

    public float lookSpeed = 2f;
    public float lookXlimit = 80f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    private Transform driver;
    private Vector3 lastPosition;

    private CharacterController characterController;
    private PlayerController playerController;
    private BoatFuelManager fuelManager;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerController = FindFirstObjectByType<PlayerController>();    
        fuelManager = GetComponent<BoatFuelManager>();
        lastPosition = transform.position;
    }

    void Update()
    {
        float yPos = transform.position.y;

        if (!isMounted || driver == null)
        {
            return;
        }

        if (yPos > 2.8)
        {
            yPos = 2.8f;
        }
        Physics.IgnoreLayerCollision(8, 9);
        Physics.IgnoreLayerCollision(8, 10);

        CheckIfMoving();

        if (fuelManager.fuelAmount > 0)
        {
            HandleMovement();
        }

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXlimit, lookXlimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        player.transform.localRotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
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
            transform.Rotate(Vector3.up * (horizontal * turnSpeed) * 60f * Time.deltaTime);
        }

        Vector3 move = transform.forward * currentSpeed;
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

        rod.gameObject.SetActive(false);
        playerController.enabled = false;
    }

    public void Dismount()
    {
        if (!isMounted || driver == null)
        {
            return;
        }

        driver.SetParent(null);

        playerController.enabled = true;

        isMounted = false;
        driver = null;
        currentSpeed = 0f;
        rod.gameObject.SetActive(true);
    }


    // UPGRADES - Ashley

    public void UpgradeSpeed(float UpgradeAmount)
    {
        walkSpeed += UpgradeAmount;
        runSpeed += UpgradeAmount;
    }
    public void UpgradeAcceleration(float UpgradeAmount)
    {
        acceleration += UpgradeAmount;
    }
    public void UpgradeTurnSpeed(float UpgradeAmount)
    {
        turnSpeed += UpgradeAmount;
    }

    private void CheckIfMoving()
    {
        if (transform.position != lastPosition)
        {
            fuelDepletionTimer += Time.deltaTime;
        }

        if (fuelDepletionTimer > 5)
        {
            fuelDepletionTimer = 0;
            fuelManager.DepleteFuel(1);
        }
        
        lastPosition = transform.position;
    }
}
