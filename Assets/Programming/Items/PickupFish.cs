using UnityEngine;
public class PickupFish : Interactable
{
    public FishSO fish;

    public override void OnInteract()
    {
        InvItemSO itemPickup = fish.InventoryItem;
        Inventory.Instance.AttemptAddItemToInventory(itemPickup);
        Destroy(gameObject);
    }
}
