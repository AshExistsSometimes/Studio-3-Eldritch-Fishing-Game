using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlot;
    private bool menuActivated;
    public GameObject InventoryMenu;

    public ItemSO[] ItemSOs;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && menuActivated)
        {
            InventoryMenu.SetActive(false);
            menuActivated = false;
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && !menuActivated)
        {
            InventoryMenu.SetActive(true);
            menuActivated = true;
        }
    }
    public void addItem(string itemName, int itemIndex, Sprite itemSprite)
    {
        for (int i = 0; i < inventorySlot.Length; i++)
        {
            if (inventorySlot[i].isFull == false)
            {
                inventorySlot[i].addItem(itemName, i, itemSprite);
                return;
            }
        }
    }

    public void sellItem(string itemName, int itemIndex, Sprite itemSprite)
    {
        for (int i = 0; i < ItemSOs.Length; i++)
        {
            if (ItemSOs[i].itemName == itemName)
            {
                ItemSOs[i].SellItem();
                return;
            }
        }
    }
}
