using UnityEngine;

public class BuySellInvInteractable : Interactable
{
    public BuySellSystem buySell;
    public PlayerMovement player;

    private void Update()
    {
        if (Input.GetKeyDown(InputManager.GetKeyCode("CloseMenu")))
        {
            buySell.CloseInventory();
        }
    }

    public override void OnInteract()
    {
        if (buySell.inventory.inventoryObject.activeSelf)
        {
            buySell.CloseInventory();
        }
        else
        {
            buySell.OpenInventory();
        }
    }
}
