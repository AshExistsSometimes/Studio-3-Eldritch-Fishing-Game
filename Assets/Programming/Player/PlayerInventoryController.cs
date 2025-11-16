using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    public Inventory inventory;
    public PlayerController player;

    void Update()
    {
        player.canMove = !inventory.inventoryOpen;

        if (Input.GetKeyDown(InputManager.GetKeyCode("OpenInventory")))
        {
            if (inventory.inventoryObject.activeSelf)
            {
                inventory.CloseInventory();
            }
            else
            {
                inventory.OpenInventory(false, false);
            }
        }

        // close the info panel if open otherwise close the inventory.
        if (Input.GetKeyDown(InputManager.GetKeyCode("CloseMenu")))
        {
            inventory.CloseInventory();
        }
    }
}
