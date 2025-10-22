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
                inventory.OpenInventory(true);// Boat inventory is true
            }
        }
    }
}
