using UnityEngine;

public interface IInteractable
{
    // Called when cursor hovers over the object.
    // Should trigger a visual highlight and interaction prompt.
    void OnMouseOver();

    // Called when cursor stops hovering over the object.
    // Should remove highlight and hide interaction prompt.
    void OnMouseOff();

    // Called when the player presses the interact key (E) while hovering over the object.
    // The actual interaction logic is defined by the implementing class.
    void OnInteract();
}

