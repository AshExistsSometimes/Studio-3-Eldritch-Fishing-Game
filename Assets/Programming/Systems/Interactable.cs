using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Renderer))]
public class Interactable : MonoBehaviour, IInteractable
{
    [Header("Interaction Feedback")]
    [Tooltip("Glow color when the object is hovered over.")]
    public Color GlowColor = Color.yellow;

    [Tooltip("Strength multiplier for the emission glow.")]
    [Range(0f, 5f)] public float GlowIntensity = 1.5f;

    private Renderer _renderer;
    private Material _materialInstance;
    private Color _originalEmissionColor;
    private bool _isGlowing = false;
    private TextMeshProUGUI interactionPrompt;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _materialInstance = _renderer.material; // Creates a unique instance for this object

        if (_materialInstance.HasProperty("_EmissionColor"))
            _originalEmissionColor = _materialInstance.GetColor("_EmissionColor");

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
            Debug.LogWarning("BaseInteractable: No UI element found with tag 'InteractionPopup'.");
        }
    }

    private void OnDestroy()
    {
        if (_materialInstance != null)
            Destroy(_materialInstance);
    }

    // Triggered when the player looks at this object.
    // Applies emission glow and shows prompt.
    public void OnMouseOver()
    {
        if (_isGlowing)
            return;

        _isGlowing = true;
        if (_materialInstance.HasProperty("_EmissionColor"))
        {
            _materialInstance.EnableKeyword("_EMISSION");
            _materialInstance.SetColor("_EmissionColor", GlowColor * GlowIntensity);
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.text = "Press E to Interact";
            interactionPrompt.enabled = true;
        }
    }


    // Triggered when the player looks away from this object.
    // Removes emission glow and hides prompt.
    public void OnMouseOff()
    {
        if (!_isGlowing)
            return;

        _isGlowing = false;
        if (_materialInstance.HasProperty("_EmissionColor"))
            _materialInstance.SetColor("_EmissionColor", _originalEmissionColor);

        if (interactionPrompt != null)
            interactionPrompt.enabled = false;
    }


    // Triggered when the player presses E while looking at this object.
    // Override in subclasses for custom behavior.
    public virtual void OnInteract()
    {
        Debug.Log($"{name} interacted with.");
    }
}