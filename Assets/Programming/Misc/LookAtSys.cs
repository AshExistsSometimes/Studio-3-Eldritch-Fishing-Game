using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtSys : MonoBehaviour
{
    public enum LookMode { FaceCamera, FaceTarget, LockAxis }

    [Header("Look Settings")]
    public LookMode lookMode = LookMode.FaceCamera;
    public Transform target;

    [Tooltip("Which axes should remain locked when rotating.")]
    public bool lockX;
    public bool lockY;
    public bool lockZ;

    [Tooltip("If true, will use LateUpdate instead of Update (better for camera facing).")]
    public bool useLateUpdate = true;

    [Header("Look Bounds (degrees)")]
    public float minX = -35f;
    public float maxX = 89f;
    public float minY = -180f;
    public float maxY = 180f;

    [Header("Look Dampening (Non-Random)")]
    [Tooltip("How quickly the object follows its target (for normal look modes). Higher = snappier.")]
    public float lookDampening = 5f;

    [Header("Random Look Settings")]
    public bool randomLook = false;
    [Tooltip("How quickly the object rotates to the RANDOM look direction.")]
    public float randomLookSpeed = 2f;
    public Vector2 lookInterval = new Vector2(2f, 4f);
    public Vector2 lookDuration = new Vector2(1f, 2f);

    [Header("Random Look: Player Focus")]
    public bool randomLookFocusPlayer = false;
    [Range(0f, 100f)] public float randomLookFocusChance = 20f;
    public Vector2 randomLookFocusDuration = new Vector2(1.5f, 3f);

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private float intervalTimer;
    private float durationTimer;
    private Transform player;

    void Start()
    {
        initialRotation = transform.rotation;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ScheduleNextLook();
        targetRotation = initialRotation;
    }

    void Update()
    {
        if (!useLateUpdate)
            HandleLook();
    }

    void LateUpdate()
    {
        if (useLateUpdate)
            HandleLook();
    }

    void HandleLook()
    {
        if (randomLook)
        {
            RandomLookHandler();
            return;
        }

        // --- NON-RANDOM LOOK MODES ---
        switch (lookMode)
        {
            case LookMode.FaceCamera:
                if (Camera.main != null)
                    targetRotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position);
                break;
            case LookMode.FaceTarget:
                if (target != null)
                    targetRotation = Quaternion.LookRotation(target.position - transform.position);
                break;
            case LookMode.LockAxis:
                LockAxisLook();
                return; // LockAxisLook directly applies rotation
        }

        // Apply axis locks
        Vector3 desiredEuler = targetRotation.eulerAngles;
        Vector3 initialEuler = initialRotation.eulerAngles;
        if (lockX) desiredEuler.x = initialEuler.x;
        if (lockY) desiredEuler.y = initialEuler.y;
        if (lockZ) desiredEuler.z = initialEuler.z;
        targetRotation = Quaternion.Euler(desiredEuler);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lookDampening);

        ClampRotation();
    }

    void LockAxisLook()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(direction);

        Vector3 euler = lookRot.eulerAngles;
        if (lockX) euler.x = transform.rotation.eulerAngles.x;
        if (lockY) euler.y = transform.rotation.eulerAngles.y;
        if (lockZ) euler.z = transform.rotation.eulerAngles.z;

        transform.rotation = Quaternion.Euler(euler);
    }

    void RandomLookHandler()
    {
        if (durationTimer > 0)
        {
            durationTimer -= Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * randomLookSpeed);
            return;
        }

        intervalTimer -= Time.deltaTime;
        if (intervalTimer <= 0)
        {
            PickNewRandomLook();
            ScheduleNextLook();
        }
    }

    void PickNewRandomLook()
    {
        bool focusPlayer = randomLookFocusPlayer && player != null &&
                           Random.Range(0f, 100f) <= randomLookFocusChance;

        if (focusPlayer)
        {
            targetRotation = Quaternion.LookRotation(player.position - transform.position);
            durationTimer = Random.Range(randomLookFocusDuration.x, randomLookFocusDuration.y);
        }
        else
        {
            float randomYaw = Random.Range(minY, maxY);
            float randomPitch = Random.Range(minX, maxX);
            targetRotation = initialRotation * Quaternion.Euler(randomPitch, randomYaw, 0);
            durationTimer = Random.Range(lookDuration.x, lookDuration.y);
        }
    }

    void ClampRotation()
    {
        Vector3 euler = transform.localEulerAngles;

        if (euler.x > 180) euler.x -= 360;
        if (euler.y > 180) euler.y -= 360;

        euler.x = Mathf.Clamp(euler.x, minX, maxX);
        euler.y = Mathf.Clamp(euler.y, minY, maxY);

        transform.localEulerAngles = new Vector3(euler.x, euler.y, euler.z);
    }

    void ScheduleNextLook()
    {
        intervalTimer = Random.Range(lookInterval.x, lookInterval.y);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 1f);

        Gizmos.color = new Color(0, 0.5f, 1f, 0.2f);

        Vector3 forward = transform.forward;
        Quaternion leftBound = Quaternion.Euler(0, minY, 0);
        Quaternion rightBound = Quaternion.Euler(0, maxY, 0);
        Quaternion upBound = Quaternion.Euler(minX, 0, 0);
        Quaternion downBound = Quaternion.Euler(maxX, 0, 0);

        Gizmos.DrawRay(transform.position, leftBound * forward * 1.5f);
        Gizmos.DrawRay(transform.position, rightBound * forward * 1.5f);
        Gizmos.DrawRay(transform.position, upBound * forward * 1.5f);
        Gizmos.DrawRay(transform.position, downBound * forward * 1.5f);
    }
}
