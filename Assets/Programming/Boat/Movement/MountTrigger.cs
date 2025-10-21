using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountTrigger : Interactable, IInteractable
{
    public BoatController boat;

    public Transform playerBody;

    public bool isMounted = false;
    public bool canMount = false;

    private void Update()
    {
        if (Input.GetKeyDown(InputManager.GetKeyCode("Interact")) & !canMount)
        {
            if (isMounted)
            {
                isMounted = false;
                canMount = false;
                boat.Dismount();
            }
            else
            {
                canMount = true;
            }

        }
    }

    public override void OnInteract()
    {
        TriggerMount();
    }

    public void TriggerMount()
    {
        if (Input.GetKeyDown(InputManager.GetKeyCode("Interact")) & canMount)// Please put this in a function so that it can be called by OnInteract, so we can have the player interact with a steering wheel to start sailing
        {
            Debug.Log("trying to mount");
            if (!isMounted && boat != null)
            {
                isMounted = true;
                canMount = false;
                boat.Mount(playerBody);
            }
            else if (isMounted)
            {
                isMounted = false;
                boat.Dismount();
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        canMount = true;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //   if (other.CompareTag("Player"))
    //    {
    //        canMount = false;
    //    }
    //}
}
