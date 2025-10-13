using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    // Item Attributes
    public ItemSO item;
    public bool isFull; // spacechecker is full
    public int itemIndex; // spot in the inventory


    [SerializeField]
    public void addItem(string itemName, int itemIndex)
    {
        item.itemName = itemName;
        this.itemIndex = itemIndex;
        isFull = true;
    }
}
