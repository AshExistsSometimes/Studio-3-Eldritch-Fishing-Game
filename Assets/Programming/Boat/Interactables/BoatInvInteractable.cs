using UnityEngine;

public class BoatInvInteractable : Interactable
{
    public Inventory inventory;
    public PlayerMovement player;

    public override void OnInteract()
    {
        {
            if (inventory.inventoryObject.activeSelf)
            {
                inventory.CloseInventory();
            }
            else
            {
                inventory.OpenInventory(true, false);// Boat inventory is true
            }
        }
    }
}
