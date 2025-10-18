using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////
public class InventoryManager : MonoBehaviour
{
    [Header("References")]
    public GameObject inventoryUI;
    public GameObject inventorySlotsParent;
    public GameObject inventoryBackground;
    public GameObject itemInfoParent;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescText;
    public TextMeshProUGUI itemSizeText;
    public TextMeshProUGUI itemValueText;

    [Header("Parameters")]
    public int inventorySize;
    public Color highlightedItemBackgroundColor;

    [Header("Keybinds")]
    public KeyCode openInventoryKey;

    [Header("Prefabs")]
    public GameObject inventorySlot;


    //Instance of InventoryManager
    public static InventoryManager instance { get; private set; }


    //Item struct
    public struct Item
    {
        public ItemSO originalSO;
        public GameObject prefab;
        public int priceToBuy;
        public int sellValue;

        public FishSO fishSO;
        public float fishSize;
    }

    [Header("Inventory")]
    public List<Item> inventory;

    //State the inventory is currently in
    private enum States
    {
        NotInInventory,
        InInventory
    }
    private States currentState;

    //Variables
    private int indexOfHighlightedItem;

    //WIP
    public bool mouseHoveringOverSomething;
    public int indexOfMouseHover;

    ////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        //Ensures singleton nature of instance variable
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //CreateInventoryUI();
    }

    ////////////////////////////////////////////////////////////////////
    private void Update()
    {
        //GetInput();
        //UpdateInventoryUI();
        //CheckToShowItemInfoText();
    }

    ////////////////////////////////////////////////////////////////////
    private void GetInput()
    {
        if (Input.GetKeyDown(openInventoryKey))
        {
            if (currentState == States.InInventory)
            {
                ToggleStateOfGame(States.NotInInventory);
            }
            else if (currentState == States.NotInInventory)
            {
                ToggleStateOfGame(States.InInventory);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))// && inventory.Count >= 1)
        {
            indexOfHighlightedItem = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))// && inventory.Count >= 2)
        {
            indexOfHighlightedItem = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))// && inventory.Count >= 3)
        {
            indexOfHighlightedItem = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))// && inventory.Count == 4)
        {
            indexOfHighlightedItem = 3;
        }
    }

    ////////////////////////////////////////////////////////////////////
    public bool AttemptToAddItemToInventory(Item itemToAdd)
    {
        if (!CheckForInventoryFull())
        {
            inventory.Add(itemToAdd);
            return true;
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////
    public bool CheckIfItemInIndex(int indexToCheck)
    {
        if (inventory.Count > indexToCheck)
        {
            return true;
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////
    public bool AttemptToRemoveItemFromInventory(Item itemToRemove)
    {
        if (inventory.Contains(itemToRemove))
        {
            indexOfHighlightedItem = 0;
            inventory.Remove(itemToRemove);
            return true;
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////
    public int GetNoOfAvailableInventorySpaces()
    {
        return inventorySize - inventory.Count;
    }

    ////////////////////////////////////////////////////////////////////
    public bool CheckForInventoryFull()
    {
        return inventory.Count == inventorySize;
    }

    ////////////////////////////////////////////////////////////////////
    private void CreateInventoryUI()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject iSlot = Instantiate(inventorySlot, inventorySlotsParent.transform);
            if (inventory.Count > i)
            {
                iSlot.GetComponent<Image>().sprite = inventory[i].originalSO.image;
            }
            else
            {
                iSlot.GetComponent<Image>().sprite = null;
            }
            if (i == indexOfHighlightedItem)// && inventory.Count != 0)
            {
                iSlot.transform.GetChild(0).GetComponent<Image>().color = highlightedItemBackgroundColor;
            }
        }

        inventoryBackground.SetActive(false);
        itemInfoParent.SetActive(false);
    }

    ////////////////////////////////////////////////////////////////////
    private void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject iSlot = inventorySlotsParent.transform.GetChild(i).gameObject;

            if (inventory.Count > i)
            {
                iSlot.GetComponent<Image>().sprite = inventory[i].originalSO.image;
            }
            else
            {
                iSlot.GetComponent<Image>().sprite = null;
            }
            if (i == indexOfHighlightedItem)// && inventory.Count != 0)
            {
                iSlot.transform.GetChild(0).GetComponent<Image>().color = highlightedItemBackgroundColor;
            }

        }
    }

    ////////////////////////////////////////////////////////////////////
    private void ToggleStateOfGame(States stateToToggleTo)
    {
        currentState = stateToToggleTo;

        if (stateToToggleTo == States.InInventory)
        {
            Cursor.visible = true;
            inventoryBackground.SetActive(true);
        }
        else if (stateToToggleTo == States.NotInInventory)
        {
            Cursor.visible = false;
            inventoryBackground.SetActive(false);
        }
    }

    ////////////////////////////////////////////////////////////////////
    private void UpdateItemInfoUI(Item itemToGetInfoFrom)
    {
        itemNameText.text = "Name: " + itemToGetInfoFrom.originalSO.itemName;
        itemDescText.text = itemToGetInfoFrom.originalSO.itemDescription;
        if (itemToGetInfoFrom.originalSO.isFish)
        {
            itemSizeText.text = "Size: " + itemToGetInfoFrom.fishSize;
        }
        else
        {
            itemSizeText.text = "";
        }
        if (itemToGetInfoFrom.originalSO.isSellable)
        {
            itemValueText.text = "Value: " + itemToGetInfoFrom.sellValue;
        }
        else
        {
            itemValueText.text = "";
        }
    }

    ////////////////////////////////////////////////////////////////////
    private void CheckToShowItemInfoText()
    {
        if (currentState == States.InInventory)
        {
            if (mouseHoveringOverSomething && inventory.Count > indexOfMouseHover)
            {
                UpdateItemInfoUI(inventory[indexOfMouseHover]);
                itemInfoParent.SetActive(true);
            }
            else
            {
                itemInfoParent.SetActive(false);
            }
        }
        else
        {
            itemInfoParent.SetActive(false);
        }

    }

    ////////////////////////////////////////////////////////////////////
    public Item ConvertItemSO(ItemSO itemSOToConvert, float fishSize, int sellValue)
    {
        //Assigns variables from ItemSO
        Item convertedItem = new Item();
        convertedItem.originalSO = itemSOToConvert;
        convertedItem.prefab = itemSOToConvert.prefab;
        convertedItem.priceToBuy = itemSOToConvert.priceToBuy;

        //Assigns fish relevant variables if applicable 
        if (convertedItem.originalSO.isFish)
        {
            convertedItem.fishSO = itemSOToConvert.fishSO;
            convertedItem.fishSize = fishSize;
            convertedItem.sellValue = sellValue;
        }
        else
        {
            convertedItem.sellValue = convertedItem.originalSO.baseSellValue;
        }

        return convertedItem;
    }

    ////////////////////////////////////////////////////////////////////
}
////////////////////////////////////////////////////////////////////


