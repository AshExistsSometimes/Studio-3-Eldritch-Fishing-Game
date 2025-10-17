using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountTrigger : MonoBehaviour
{
    public BoatController boat;

    public Transform playerBody;

    public bool isMounted = false;
    public bool canMount = false;

    private void Update()
    {
        if (Input.GetKeyDown(InputManager.GetKeyCode("Interact")) & canMount)// Please put this in a function so that it can be called by OnInteract, so we can have the player interact with a steering wheel to start sailing
        {
            if (!isMounted && boat != null)
            {
                isMounted = true;
                boat.Mount(playerBody);
            }
            else if (isMounted)
            {
                isMounted = false;
                boat.Dismount();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canMount = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
       if (other.CompareTag("Player"))
        {
            canMount = false;
        }
    }
}
