using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlot;
    private bool menuActivated;
    public GameObject InventoryMenu;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && menuActivated)
        {
            InventoryMenu.SetActive(false);
            menuActivated = false;
        }
        else if (Input.GetKeyDown(KeyCode.I) && !menuActivated)
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
}
