using System.Collections;
using UnityEngine;

public class CrossbowWeapon : Interactable
{
    [Header("Mounting")]
    public Transform MountPosition;
    public Transform DismountPos;
    public PlayerMovement playerMovement;
    public GameObject playerObject;
    public FishingRod rod;

    [Header("Rotation Pivots")]
    public Transform LR_RotPoint;
    public Transform UD_RotPoint;

    [Header("Rotation Settings")]
    public Vector2 YRotConstraints = new Vector2(-90f, 90f);
    public Vector2 XRotConstraints = new Vector2(-45f, 45f);
    public float yawSpeed = 60f;
    public float pitchSpeed = 40f;

    [Header("Firing")]
    public GameObject HarpoonPrefab;
    public Transform HarpoonSpawnPoint;
    public float FireRate = 1.0f;
    public float FirePower = 20f;
    public string BoatTag = "Boat";

    [Header("Visual")]
    public GameObject HarpoonVisual;

    private bool isMounted = false;
    private float lastFireTime = -999f;

    private float currentYaw = 0f;
    private float currentPitch = 0f;

    private void Start()
    {
        if (LR_RotPoint != null) currentYaw = NormalizeAngle(LR_RotPoint.localEulerAngles.y);
        if (UD_RotPoint != null) currentPitch = NormalizeAngle(UD_RotPoint.localEulerAngles.x);
    }

    public override void OnInteract()
    {
        if (!isMounted)
            Mount();
    }

    // Mount player onto crossbow, disable movement, align rotation.</summary>
    public void Mount()
    {
        if (!playerObject || !playerMovement || !MountPosition) return;

        // Reset the crossbow pivots first
        if (LR_RotPoint != null) LR_RotPoint.localRotation = Quaternion.identity;
        if (UD_RotPoint != null) UD_RotPoint.localRotation = Quaternion.identity;

        // Disable player movement and headbob
        playerMovement.canMove = false;
        playerMovement.enableHeadbob = false;

        // Stop CharacterController drift
        CharacterController cc = playerObject.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        // Parent and snap the player to mount
        playerObject.transform.SetParent(MountPosition);
        playerObject.transform.localPosition = Vector3.zero;
        playerObject.transform.localRotation = Quaternion.identity;

        // Align camera along crossbow forward
        if (playerMovement.playerCamera != null)
            playerMovement.playerCamera.transform.rotation = Quaternion.LookRotation(HarpoonSpawnPoint.forward);

        isMounted = true;
        rod.gameObject.SetActive(false);
    }


    /// <summary>Dismount player, restore movement, place at DismountPos.</summary>
    public void Dismount()
    {
        if (!playerObject || !playerMovement) return;

        isMounted = false;

        playerObject.transform.SetParent(null);

        if (DismountPos)
        {
            playerObject.transform.position = DismountPos.position;

            // Keep Y rotation aligned, reset pitch
            Vector3 rot = playerObject.transform.eulerAngles;
            rot.x = 0f;
            rot.z = 0f;
            playerObject.transform.eulerAngles = rot;
        }

        // Re-enable CharacterController after teleport
        CharacterController cc = playerObject.GetComponent<CharacterController>();
        if (cc) cc.enabled = true;

        playerMovement.canMove = true;
        playerMovement.enableHeadbob = true;
        rod.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isMounted) return;

        // Handle crossbow rotation from input
        HandleRotationInput();

        // Puts a harpoon in the crossbow when ready to fire
        UpdateHarpoonVisual();

        if (playerMovement != null && playerMovement.playerCamera != null && UD_RotPoint != null)
        {
            // Get vertical input for pitch only
            float vertical = -Input.GetAxis("Vertical"); // invert if needed
            float pitch = vertical * pitchSpeed * Time.deltaTime;

            // Current camera rotation
            Vector3 camEuler = playerMovement.playerCamera.transform.localEulerAngles;

            // Convert to -180..180 range to clamp properly
            if (camEuler.x > 180f) camEuler.x -= 360f;

            // Apply pitch and clamp
            camEuler.x = Mathf.Clamp(camEuler.x + pitch, XRotConstraints.x, XRotConstraints.y);

            // Apply new rotation (keep Y and Z as they are)
            playerMovement.playerCamera.transform.localEulerAngles = new Vector3(camEuler.x, 0f, 0f);
        }

        // Dismount
        if (Input.GetKeyDown(KeyCode.Escape))
            Dismount();

        // Fire harpoon
        if (Input.GetKey(InputManager.GetKeyCode("UseItem")))
            FireHarpoon();
    }

    /// <summary>Rotate L/R and U/D pivots within constraints using input.</summary>
    private void HandleRotationInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (LR_RotPoint)
        {
            currentYaw += h * yawSpeed * Time.deltaTime;
            currentYaw = Mathf.Clamp(currentYaw, YRotConstraints.x, YRotConstraints.y);
            Vector3 e = LR_RotPoint.localEulerAngles;
            e.y = currentYaw;
            LR_RotPoint.localEulerAngles = e;
        }

        if (UD_RotPoint)
        {
            currentPitch += -v * pitchSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, XRotConstraints.x, XRotConstraints.y);
            Vector3 e = UD_RotPoint.localEulerAngles;
            e.x = currentPitch;
            UD_RotPoint.localEulerAngles = e;
        }
    }

    /// <summary>Fire harpoon from UD_RotPoint�s -Z direction if off cooldown.</summary>
    public void FireHarpoon()
    {
        if (!isLoaded()) return;

        if (!HarpoonPrefab || !HarpoonSpawnPoint || !UD_RotPoint) return;

        GameObject go = Instantiate(HarpoonPrefab, HarpoonSpawnPoint.position, HarpoonSpawnPoint.rotation);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb)
        {
            Vector3 dir = -UD_RotPoint.forward.normalized;
            rb.linearVelocity = dir * FirePower;
        }

        Harpoon h = go.GetComponent<Harpoon>();
        if (h) h.IgnoredTag = BoatTag;

        lastFireTime = Time.time;
    }
    private void UpdateHarpoonVisual()
    {
        HarpoonVisual.gameObject.SetActive(isLoaded());
    }

    private bool isLoaded()
    {
        if (Time.time < lastFireTime + FireRate)
            return false;
        else
            return true;
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}


