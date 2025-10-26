using System.Collections;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    [Header("References")]
    public Transform cam;             // Camera reference for cast direction
    public Transform castPoint;       // Rod tip (hook origin)
    public GameObject hook;           // Hook/bobber object
    public GameObject reelVisual;     // Visual reel object (spins during reeling)
    public LineRenderer lineRenderer; // Renders fishing line between rod and hook

    [Header("Settings")]
    public float Cooldown = 0.5f;     // Time before next cast allowed
    public float MaxCastDistance = 10f;

    [Header("Casting")]
    public float castForce = 10f;
    public float castUpForce = 1.0f;

    [Header("Reeling")]
    public float reelInSpeed = 5.0f;
    public float SpinSpeed = 100f;    // Reel spin speed
    public float ReelTime = 0.5f;     // Default spin duration for short reeling

    [Header("Audio")]
    public AudioClip castClip;
    public AudioClip reelClip;
    public AudioSource audioSource;

    public bool RodCastReady = true;
    public bool ReelSpinning = false;

    private void Awake()
    {
        // Ensure audio source
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Ensure line renderer setup
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }
    }

    private void LateUpdate()
    {
        // --- Casting Input ---
        if (RodCastReady && Input.GetKeyDown(InputManager.GetKeyCode("UseItem")))
        {
            PlayCastSound();
            Cast();
            StartCoroutine(WaitForCooldown(Cooldown));
        }

        // --- Handle reel visual spin if active ---
        if (ReelSpinning)
        {
            if (reelVisual != null)
                reelVisual.transform.Rotate(0f, 0f, SpinSpeed * Time.deltaTime, Space.Self);
        }

        // Clamp hook distance to prevent line stretch
        float distance = Vector3.Distance(castPoint.position, hook.transform.position);
        if (distance > MaxCastDistance)
        {
            Vector3 direction = (hook.transform.position - castPoint.position).normalized;
            hook.transform.position = castPoint.position + direction * MaxCastDistance;
        }

        // Update visual line
        UpdateLineRenderer();
    }

    // Casts the hook forward using physics force.
    public void Cast()
    {
        Debug.Log("cast");
        RodCastReady = false;

        hook.transform.position = castPoint.position;
        hook.SetActive(true);

        Rigidbody hookRb = hook.GetComponent<Rigidbody>();
        if (hookRb != null)
        {
            hookRb.useGravity = true;
            hookRb.mass = 1f;
            hookRb.linearDamping = 0.5f;
            hookRb.angularDamping = 0.4f;

            Vector3 forceToAdd = cam.transform.forward * castForce + transform.up * castUpForce;
            hookRb.AddForce(forceToAdd, ForceMode.Impulse);
        }

        if (lineRenderer != null)
            lineRenderer.enabled = true;
    }

    // Starts spinning the reel visually. Can be called by minigame manager.
    public void StartReelSpin()
    {
        if (!ReelSpinning)
        {
            ReelSpinning = true;
            PlayReelSound();
        }
    }

    // Stops reel spin visual. Can be called by minigame manager.
    public void StopReelSpin()
    {
        ReelSpinning = false;
    }

    // Starts pulling the bobber (hook) back towards the rod tip.
    // Used when minigame ends or fish caught.
    public void PullBobberBackIn()
    {
        StartCoroutine(ReelInHookCoroutine());
    }

    // Coroutine to visually and logically reel in the hook.
    private IEnumerator ReelInHookCoroutine()
    {
        StartReelSpin();
        PlayReelSound();

        Rigidbody hookRb = hook.GetComponent<Rigidbody>();
        if (hookRb != null)
        {
            hookRb.useGravity = false;
            hookRb.linearVelocity = Vector3.zero;
            hookRb.linearDamping = 0.0f;
            hookRb.angularDamping = 0.0f;
        }

        // Smoothly move the hook in
        float elapsed = 0f;
        Vector3 startPos = hook.transform.position;

        while (elapsed < ReelTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / ReelTime;
            hook.transform.position = Vector3.Lerp(startPos, castPoint.position, t);
            UpdateLineRenderer();
            yield return null;
        }

        hook.transform.position = castPoint.position;
        hook.SetActive(false);

        StopReelSpin();
        RodCastReady = true;

        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    // Plays the cast sound.
    public void PlayCastSound()
    {
        if (audioSource && castClip)
            audioSource.PlayOneShot(castClip);
    }

    // Plays the reel sound.
    public void PlayReelSound()
    {
        if (audioSource && reelClip)
            audioSource.PlayOneShot(reelClip);
    }

    // Waits a set duration before allowing another cast.
    private IEnumerator WaitForCooldown(float CDTime)
    {
        yield return new WaitForSeconds(CDTime);
    }

    // Updates the visual fishing line between rod tip and hook.
    private void UpdateLineRenderer()
    {
        if (lineRenderer == null)
            return;

        if (hook == null || !hook.activeSelf)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, castPoint.position);
        lineRenderer.SetPosition(1, hook.transform.position);
    }
}
