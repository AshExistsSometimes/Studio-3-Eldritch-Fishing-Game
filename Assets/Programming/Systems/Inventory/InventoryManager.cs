using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////
public class InventoryManager : MonoBehaviour
{
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

    //States the inventory can be in
    public enum States
    {
        NotInInventory,
        InInventory,
        InBoatInventory
    }


    [Header("Inventory")]
    public List<Item> inventory;

    [Header("References")]
    [SerializeField] private GameObject inventoryUI;
    public GameObject inventorySlotsParent;
    [SerializeField] private GameObject inventoryBackground;
    [SerializeField] private GameObject itemInfoParent;
    public GameObject boatInventoryUI;

    [SerializeField]private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescText;
    [SerializeField] private TextMeshProUGUI itemSizeText;
    [SerializeField] private TextMeshProUGUI itemValueText;

    [Header("Parameters")]
    public int inventorySize;
    [SerializeField] private Color highlightedItemBackgroundColour;
    [SerializeField] private Color unhighlightedItemBackgroundColour;

    [Header("Prefabs")]
    [SerializeField] private GameObject inventorySlot;


    //Variables
    public static States currentState;
    [HideInInspector] public int indexOfHighlightedItem;
    [HideInInspector] public bool mouseHoveringOverSomething;
    [HideInInspector] public int indexOfMouseHover;
    [HideInInspector] public bool hoveredItemIsInBoatInv;

    ////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        //Instantiates list
        inventory = new List<Item>();

        //Ensures singleton nature of instance variable
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        CreateInventoryUI();
    }

    ////////////////////////////////////////////////////////////////////
    private void Update()
    {
        GetInput();
        UpdateInventoryUI();
        CheckToShowItemInfoText();
    }

    ////////////////////////////////////////////////////////////////////
    private void GetInput()
    {

        if (currentState == States.NotInInventory)
        {
            //Checks to swap currently highlighted item
            if (Input.GetKeyDown(KeyCode.Alpha1) && inventory.Count >= 1)
            {
                indexOfHighlightedItem = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && inventory.Count >= 2)
            {
                indexOfHighlightedItem = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && inventory.Count >= 3)
            {
                indexOfHighlightedItem = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && inventory.Count == 4)
            {
                indexOfHighlightedItem = 3;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////
    protected virtual void CreateInventoryUI()
    {
        //Creates UI for inventory
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
            if (i == indexOfHighlightedItem && inventory.Count != 0)
            {
                iSlot.transform.GetChild(0).GetComponent<Image>().color = highlightedItemBackgroundColour;
            }
            else
            {
                iSlot.transform.GetChild(0).GetComponent<Image>().color = unhighlightedItemBackgroundColour;
            }
        }

        inventoryBackground.SetActive(false);
        itemInfoParent.SetActive(false);
    }

    ////////////////////////////////////////////////////////////////////
    public virtual void UpdateInventoryUI()
    {
        //Updates UI for inventory
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
            if (i == indexOfHighlightedItem && inventory.Count != 0)
            {
                iSlot.transform.GetChild(0).GetComponent<Image>().color = highlightedItemBackgroundColour;
            }
            else
            {
                iSlot.transform.GetChild(0).GetComponent<Image>().color = unhighlightedItemBackgroundColour;
            }

        }
    }

    ////////////////////////////////////////////////////////////////////
    protected void ToggleStateOfGame(States stateToToggleTo)
    {
        currentState = stateToToggleTo;
        if (stateToToggleTo == States.InInventory)
        {
            Cursor.visible = true;
            inventoryBackground.SetActive(true);
            boatInventoryUI.SetActive(false);
        }
        else if (stateToToggleTo == States.InBoatInventory)
        {
            Cursor.visible = true;
            inventoryBackground.SetActive(true);
            boatInventoryUI.SetActive(true);

        }
        else if (stateToToggleTo == States.NotInInventory)
        {
            Cursor.visible = false;
            inventoryBackground.SetActive(false);
            boatInventoryUI.SetActive(false);
        }
    }

    ////////////////////////////////////////////////////////////////////
    private void UpdateItemInfoUI(Item itemToGetInfoFrom)
    {
        //Updates item info UI based on given parameter
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
        if (currentState != States.NotInInventory)
        {
            if (InventoryManager.instance.mouseHoveringOverSomething && hoveredItemIsInBoatInv)
            {
                if (BoatInventoryManager.instance.inventory.Count > indexOfMouseHover && Input.GetKey(KeyCode.LeftShift))
                {
                    UpdateItemInfoUI(BoatInventoryManager.instance.inventory[indexOfMouseHover]);
                    itemInfoParent.SetActive(true);
                }
            }
            else if (InventoryManager.instance.mouseHoveringOverSomething && !hoveredItemIsInBoatInv)
            {
                if (inventory.Count > indexOfMouseHover && Input.GetKey(KeyCode.LeftShift))
                {
                    UpdateItemInfoUI(inventory[indexOfMouseHover]);
                    itemInfoParent.SetActive(true);
                }
            }
            else
            {
                itemInfoParent.SetActive(false);
            }
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
    public void ToggleInventoryUI()
    {
        if (currentState == States.NotInInventory)
        {
            ToggleStateOfGame(States.InInventory);
        }
        else
        {
            ToggleStateOfGame(States.NotInInventory);
        }
    }

    ////////////////////////////////////////////////////////////////////
    public void ToggleBoatInventoryUI()
    {
        if (currentState == States.NotInInventory)
        {
            ToggleStateOfGame(States.InBoatInventory);
        }
        else
        {
            ToggleStateOfGame(States.NotInInventory);
        }
    }

    ////////////////////////////////////////////////////////////////////
}
////////////////////////////////////////////////////////////////////


