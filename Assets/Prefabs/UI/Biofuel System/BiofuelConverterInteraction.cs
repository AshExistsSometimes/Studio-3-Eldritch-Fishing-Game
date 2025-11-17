using UnityEngine;

public class BiofuelConverterInteraction : Interactable
{
    public Inventory inventory;
    public bool isOpen;

    public override void OnInteract()
    {
        {
            if (inventory.inventoryObject.activeSelf)
            {
                isOpen = false;
                inventory.CloseInventory();
            }
            else
            {
                isOpen = true;
                inventory.OpenInventory(false, false); // Inventory is true
            }
        }
    }
}
