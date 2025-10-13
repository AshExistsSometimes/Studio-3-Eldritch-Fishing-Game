using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    // Item Attributes
    public string itemName; // Name
    public bool isFull; // spacechecker is full
    public int itemIndex; // spot in the inventory


    [SerializeField]
    private Image itemImage;
    public void addItem(string itemName, int itemIndex, Sprite itemSprite)
    {
        this.itemName = itemName;
        this.itemIndex = itemIndex;
        isFull = true;

        itemImage.sprite = itemSprite;
    }
}
