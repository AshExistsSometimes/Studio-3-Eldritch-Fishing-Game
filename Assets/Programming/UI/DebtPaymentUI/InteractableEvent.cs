using UnityEngine;
using UnityEngine.Events;

public class InteractableEvent : Interactable
{
    public UnityEvent EventOnInteract;

    public override void OnInteract()
    {
        EventOnInteract.Invoke();
    }
}
