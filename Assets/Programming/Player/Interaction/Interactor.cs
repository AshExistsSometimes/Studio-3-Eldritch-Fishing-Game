using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Tooltip("Camera or transform from which the interaction ray originates.")]
    public Transform RayOrigin;

    [Tooltip("Maximum distance for interaction ray.")]
    public float InteractRange = 3f;

    private IInteractable currentTarget;

    private void Update()
    {
        HandleRaycast();
        HandleInteractionInput();
    }

    // Performs a raycast each frame to find interactable objects.
    private void HandleRaycast()
    {
        if (RayOrigin == null)
            return;

        Ray ray = new Ray(RayOrigin.position, RayOrigin.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, InteractRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // If we look at a new object, call OnMouseOver()
                if (interactable != currentTarget)
                {
                    ClearCurrentTarget();
                    currentTarget = interactable;
                    currentTarget.OnMouseOver();
                }
                return;
            }
        }

        // No valid hit or lost focus
        ClearCurrentTarget();
    }

    // Handles pressing the interact key when looking at an interactable.
    private void HandleInteractionInput()
    {
        if (currentTarget != null && Input.GetKeyDown(InputManager.GetKeyCode("Interact")))
        {
            currentTarget.OnInteract();
        }
    }

    // Clears the current interact target when no longer looking at it.
    private void ClearCurrentTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.OnMouseOff();
            currentTarget = null;
        }
    }
}
