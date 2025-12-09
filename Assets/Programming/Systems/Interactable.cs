using TMPro;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    public string interactionText = "Press 'E' to interact";

    protected TextMeshProUGUI interactionPrompt;

    protected virtual void Awake()
    {
        // Find TextMeshPro prompt tagged "InteractionPopup"
        GameObject popupObj = GameObject.FindGameObjectWithTag("InteractionPopup");
        if (popupObj != null)
        {
            interactionPrompt = popupObj.GetComponent<TextMeshProUGUI>();
            if (interactionPrompt != null)
                interactionPrompt.enabled = false;
        }
        else
        {
            Debug.LogWarning("Interactable: No UI element found with tag 'InteractionPopup'.");
        }
    }

    // Triggered when the player looks at this object.
    // Shows interaction prompt.
    public virtual void OnMouseHover()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.text = interactionText;
            interactionPrompt.enabled = true;
        }
    }

    // Triggered when player looks away.
    // Hides prompt.
    public virtual void OnMouseOff()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.enabled = false;
        }
    }

    // Triggered when the player presses E while looking at this object.
    public virtual void OnInteract()
    {
        Debug.Log($"{name} interacted with.");
    }
}
