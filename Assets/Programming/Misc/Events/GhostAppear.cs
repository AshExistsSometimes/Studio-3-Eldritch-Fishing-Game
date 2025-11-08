using UnityEngine;
public class GhostAppear : MonoBehaviour
{
    [Header("Settings")]
    public float LookAngleThreshold = 30f;   // Angle threshold to count as looking at object
    public Light lightSource;

    [Header("Debug")]
    public bool isVisible = false;          // Current visible state
    public bool beingLookedAt = false;      // Shows if object is currently in view cone

    private Renderer renderer;
    private Transform cam;
    private float dotThreshold;

    private bool PlayerHasSeenMe = false;
    private void OnEnable()
    {
        cam = Camera.main.transform;
        renderer = GetComponentInChildren<Renderer>();

        BecomeInvisible();

        dotThreshold = Mathf.Cos(LookAngleThreshold * Mathf.Deg2Rad);

        isVisible = false;

        IsPlayerLooking();

        // Spawn check
        if (!IsPlayerLooking())
        {
            // Reveal immediately if player isnt looking at it
            BecomeVisible();
        }
    }

    private void Update()
    {
        IsPlayerLooking();
        beingLookedAt = IsPlayerLooking();
        FaceCamera();

        // If its not visible and the player stops looking at it, it appears
        if (!IsPlayerLooking() && !isVisible)
        {
            BecomeVisible();
        }

        if (IsPlayerLooking() && isVisible)// Allows player to see the ghost before it can dissapear
        {
            PlayerHasSeenMe = true;
        }


        if (!IsPlayerLooking() && isVisible && PlayerHasSeenMe)// Player looks away while its visible
        {
            BecomeInvisible();
            gameObject.SetActive(false);
        }        
    }

    private void BecomeVisible()
    {
        isVisible = true;
        renderer.enabled = true;
        lightSource.intensity = 2f;
        FaceCamera();
    }

    private void BecomeInvisible()
    {
        isVisible = false;
        renderer.enabled = false;
        lightSource.intensity = 0f;
    }

    public bool IsPlayerLooking()
    {
        Vector3 toObj = (transform.position - cam.position).normalized;

        float dot = Vector3.Dot(cam.forward, toObj);

        return dot >= dotThreshold;
    }

    private void FaceCamera()
    {
        transform.LookAt(cam.position, Vector3.up);
    }
}