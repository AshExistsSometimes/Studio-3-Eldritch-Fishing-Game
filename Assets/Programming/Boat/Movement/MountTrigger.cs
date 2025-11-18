using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class MountTrigger : Interactable, IInteractable
{
    public BoatController boat;

    public Transform playerBody;

    public bool isMounted = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) & isMounted)
        {
            isMounted = false;
            boat.Dismount();
        }
    }
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
        else if ((Input.GetKeyDown(InputManager.GetKeyCode("Interact")) & isMounted))
        {
            isMounted = false;
            boat.Dismount();
        }
    }
}
