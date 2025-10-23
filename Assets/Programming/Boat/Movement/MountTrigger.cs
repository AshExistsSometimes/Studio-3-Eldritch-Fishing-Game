using UnityEngine;

public class MountTrigger : Interactable, IInteractable
{
    public BoatController boat;

    public Transform playerBody;

    public bool isMounted = false;

    public override void OnInteract()
    {
        TriggerMount();
    }

    public void TriggerMount()
    {
        // Controls mounting and dismounting the boat
        if (Input.GetKeyDown(InputManager.GetKeyCode("Interact")) & !isMounted)
        {
            if (!isMounted && boat != null)
            {
                isMounted = true;
                boat.Mount(playerBody);
            }
        }
        else if (Input.GetKeyDown(InputManager.GetKeyCode("Interact")) & isMounted)
        {
            isMounted = false;
            boat.Dismount();
        }
    }
}
