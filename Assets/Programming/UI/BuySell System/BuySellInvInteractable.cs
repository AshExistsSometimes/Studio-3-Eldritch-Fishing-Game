using UnityEngine;

public class BuySellInvInteractable : Interactable
{
    public Inventory inventory;
    public PlayerMovement player;

    private void Update()
    {
        if (Input.GetKeyDown(InputManager.GetKeyCode("CloseMenu")))
        {
            inventory.CloseInventory();
        }
    }

    public override void OnInteract()
    {
        if (inventory.inventoryObject.activeSelf)
        {
            inventory.CloseInventory();
        }
        else
        {
            inventory.OpenInventory(false, true);
        }
    }
}
