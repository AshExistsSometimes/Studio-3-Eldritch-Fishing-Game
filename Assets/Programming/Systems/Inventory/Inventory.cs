using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|

/// <summary>
/// Inventory manager.
/// </summary>
public class Inventory : MonoBehaviour
{
    /// <summary>
    /// Singleton, so you can call this where ever.
    /// </summary>
    public static Inventory Instance { get; private set; }

    // UI stuff
    [SerializeField]
    public GameObject inventoryObject;

    [SerializeField]
    private GameObject boatInventoryObject;

    [SerializeField]
    private GameObject inventorySlotPrefab;

    [SerializeField]
    private GameObject inventorySlotParent;

    [SerializeField]
    private GameObject boatSlotParent;

    [SerializeField]
    private Image cursorImage;

    [SerializeField]
    private DisplayItemData displayItemData;

    // How many slots you want the inventory to be.
    [SerializeField]
    int invSize = 6;

    // how many slots you want the boat to have.
    [SerializeField]
    int boatInvSize = 30;

    // Both inventory slots.
    List<InventorySlotUI> inventorySlots = new List<InventorySlotUI>();
    List<InventorySlotUI> boatSlots = new List<InventorySlotUI>();

    // ccurrent selected slot. used for draging and checks.
    private int selectedID = -1;

    private int selectedItemOriginalSlot = -1;
    private InvItemSO selectedData = null;

    public bool inventoryOpen = false;

    // DEBUG REMOVE FROM FINAL
    // public InvItemSO spawnData;

    // used for double click to open item data.
    private bool isSecondClick = false;
    private int doubleClickSlotID = -1;

    // used for storing the previous hovered slot.
    private int hoverSlotID = -1;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        SetUpSlots(); // * If you need to reference other scripts that setup during Awake, do not use Awake, Awake is for initilization.

        if (displayItemData == null)
        {
            displayItemData = GetComponent<DisplayItemData>();

            if (displayItemData == null)
            {
                Debug.LogError($"Cannot find the {nameof(DisplayItemData)}, make sure to link it to this!", this);
            }
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CloseInventory();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(InputManager.GetKeyCode("INVENTORY STRING")))

        // Ideally something else handles calling open and close inventory, but this will suffice for now.
        // Dont forget to check if the user is on the boat or have a seperate call with OpenInventory(true), this opens both the inventory
        // and boat inventory together.
        //if (Input.GetKeyDown(InputManager.GetKeyCode("OpenInventory")))
        //{
        //    if (inventoryObject.activeSelf)
        //    {
        //        CloseInventory();
        //    }
        //    else
        //    {
        //        OpenInventory(true);
        //    }
        //}

        //// close the info panel if open otherwise close the inventory.
        //if (Input.GetKeyDown(InputManager.GetKeyCode("CloseMenu")))
        //{
        //    if (displayItemData.IsPanelOpen())
        //    {
        //        displayItemData.ClosePanel();
        //        return;
        //    }

        //    CloseInventory();
        //}

        // Example code for adding to inventory.
        // if (Input.GetKeyDown(KeyCode.Space))//remove this in final.
        // {

        //     // example add item in inventory, remove in final.
        //     if (AttemptAddItemToInventory(spawnData))
        //     {
        //         // rm -rf the fish object in world.
        //     }
        // }

        // handles the cursor image.
        // if we have a item then we display.
        if (selectedData != null && selectedItemOriginalSlot != -1)
        {
            // Show the icon on the cursor image.
            cursorImage.sprite = selectedData.Icon;
            cursorImage.gameObject.SetActive(true);

            // move the image to the mouse position. Input.mousePosition is in screen space.
            cursorImage.transform.position = Input.mousePosition;

            // make the slot we are draging from darker.
            SetSlotAlpha(selectedItemOriginalSlot, 0.5f);

            // if we are hovering over a slot.
            if (selectedID != -1)
            {
                // if this is a new slot, remove the prev hover icon.
                if (hoverSlotID != selectedID && hoverSlotID != -1)
                {
                    SetSlotAlpha(hoverSlotID, 1f);
                    SetSlotImage(hoverSlotID, null);
                }

                // update the hover slot id with current.
                hoverSlotID = selectedID;

                // set the slot image to the item we have selected but as a ghost.
                SetSlotAlpha(hoverSlotID, 0.7f);
                SetSlotImage(hoverSlotID, selectedData.Icon);
            }
            // remove the hover icon if its not null.
            else if (selectedID == -1 && hoverSlotID != -1)
            {
                SetSlotAlpha(hoverSlotID, 1f);
                SetSlotImage(hoverSlotID, null);
            }
        }
        // hide the cursor image since we have no item.
        else
        {
            cursorImage.sprite = null;
            cursorImage.gameObject.SetActive(false);
        }

        // mouse down event, we store what we have selected.
        if (Input.GetMouseButtonDown(0) && selectedID != -1)
        {
            selectedData = GetSlotData(selectedID);
            selectedItemOriginalSlot = selectedID;

            // reset the double click.
            if (selectedID != doubleClickSlotID)
            {
                doubleClickSlotID = -1;
                isSecondClick = false;
            }
        }


        // If the item is dragged over a slot this will move the item and swap if there is a item in the exsisting slot.
        if (Input.GetMouseButtonUp(0) && selectedID != -1 && selectedID != selectedItemOriginalSlot && selectedData != null)
        {
            MoveAndSwapItem();

            ClearSelected();
        }

        // if we drag the item out, we drop the item.
        else if (Input.GetMouseButtonUp(0) && selectedID == -1 && selectedData != null)
        {
            DropItemInSlot(selectedItemOriginalSlot);
            ClearSelected();
        }
        // Open the info display if we select the item. I would like to double click
        else if (Input.GetMouseButtonUp(0) && selectedID == selectedItemOriginalSlot && selectedData != null)
        {
            // used for double clicking
            if (!isSecondClick)
            {
                isSecondClick = true;
                doubleClickSlotID = selectedItemOriginalSlot;
                ClearSelected();
                return;
            }

            isSecondClick = false;
            doubleClickSlotID = -1;

            // Open Gui Menu
            displayItemData.OpenItemDescription(selectedData);

            ClearSelected();

        }

        // if we click on a empty slot, then close the info panel if its open.
        else if (Input.GetMouseButtonUp(0) && selectedID != -1 && selectedID == selectedItemOriginalSlot && selectedData == null)
        {
            // We selected a empty slot, so close the display panel.

            if (!isSecondClick)
            {
                isSecondClick = true;
                doubleClickSlotID = selectedItemOriginalSlot;
                ClearSelected();
                return;
            }

            isSecondClick = false;
            doubleClickSlotID = -1;

            displayItemData.ClosePanel();

            ClearSelected();

        }

        // Dont do anything, just clear what we had selected
        else if (Input.GetMouseButtonUp(0))
        {
            ClearSelected();
        }
    }

    /// <summary>
    /// Remnoves all data relating to the selected slot.
    /// </summary>
    private void ClearSelected()
    {
        SetSlotAlpha(selectedItemOriginalSlot, 1f);
        selectedData = null;
        selectedItemOriginalSlot = -1;
    }

    /// <summary>
    /// Moves the item to the new slot and swaps with item in the new slow if there is a item there.
    /// </summary>
    private void MoveAndSwapItem()
    {
        InvItemSO otherItemData = null;

        // does the new slot have an item. Cache the data if so.
        if (DoesSlotHaveItem(selectedItemOriginalSlot))
        {
            otherItemData = GetSlotData(selectedID);
        }

        // move our item into the new slot.
        AddItemToSlot(selectedID, selectedData);

        // move the exsisting item to our original slot, if there isnt one, then remove the data in our original slot.
        if (otherItemData != null)
        {
            AddItemToSlot(selectedItemOriginalSlot, otherItemData);
        }
        else
        {
            RemoveItemAtSlot(selectedItemOriginalSlot);
        }
    }

    /// <summary>
    /// Opens the inventory UI.
    /// </summary>
    /// <param name="openBoatInventoryToo">If TRUE, this will also open the boat inventory</param>
    public void OpenInventory(bool openBoatInventoryToo = false)
    {
        inventoryOpen = true;
        inventoryObject.SetActive(true);
        boatInventoryObject.SetActive(openBoatInventoryToo);
    }

    /// <summary>
    /// Closes the invenotry screens and item data display.
    /// </summary>
    public void CloseInventory()
    {
        inventoryOpen = false;
        displayItemData.ClosePanel(); // jsut in case.
        inventoryObject.SetActive(false);
        boatInventoryObject.SetActive(false);
    }

    /// <summary>
    /// Creates the slots for item storage for both the inventory and boat.
    /// </summary>
    private void SetUpSlots()
    {
        int globalIDTracker = 0;
        for (int i = 0; i < invSize; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventorySlotParent.transform);
            InventorySlotUI slotUI = slot.GetComponent<InventorySlotUI>();
            slotUI.SetUp(globalIDTracker);
            inventorySlots.Add(slotUI);
            globalIDTracker++;
        }

        for (int i = 0; i < boatInvSize; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, boatSlotParent.transform);
            InventorySlotUI slotUI = slot.GetComponent<InventorySlotUI>();
            slotUI.SetUp(globalIDTracker);
            boatSlots.Add(slotUI);
            globalIDTracker++;
        }
    }

    /// <summary>
    /// Used for the slot UI, this sets the selected slot ID to the provided value.
    /// </summary>
    /// <param name="id">The slot ID to select.</param>
    public void SelectSlot(int id)
    {
        selectedID = id;
    }

    /// <summary>
    /// Remove the selected slot ID.
    /// </summary>
    public void DeselectSlot()
    {
        selectedID = -1;
    }

    /// <summary>
    /// Finds a empty slot, if not this will return -1 as a fail.
    /// </summary>
    /// <returns>Returns slot ID or -1 for fail.</returns>
    private int FindEmptyInvSlot()
    {
        foreach (InventorySlotUI slot in inventorySlots)
        {
            if (!slot.HasStoredData())
            {
                return slot.GetID();
            }
        }

        return -1;
    }

    /// <summary>
    /// Finds a empty slot in the boat's inventory, this will return -1 as a fail.
    /// </summary>
    /// <returns>Returns slot ID or -1 for fail.</returns>
    private int FindEmptyBoatSlot()
    {
        foreach (InventorySlotUI slot in boatSlots)
        {
            if (!slot.HasStoredData())
            {
                return slot.GetID();
            }
        }

        return -1;
    }

    /// <summary>
    /// Add item to the provided slot. Will override the original data.
    /// </summary>
    /// <param name="slotID">The slot ID to add to.</param>
    /// <param name="data">The data of the item.</param>
    private void AddItemToSlot(int slotID, InvItemSO data)
    {
        foreach (InventorySlotUI slot in inventorySlots)
        {
            if (slot.GetID() == slotID)
            {
                slot.SetItemData(data);
                return;
            }
        }

        foreach (InventorySlotUI slot in boatSlots)
        {
            if (slot.GetID() == slotID)
            {
                slot.SetItemData(data);
                return;
            }
        }
    }

    /// <summary>
    /// Checks if the slot has a item in it.
    /// </summary>
    /// <param name="slotID">The slot ID to check.</param>
    /// <returns>TRUE if there is an item.</returns>
    private bool DoesSlotHaveItem(int slotID)
    {
        foreach (InventorySlotUI slot in inventorySlots)
        {
            if (slot.GetID() == slotID)
            {
                return slot.HasStoredData();
            }
        }

        foreach (InventorySlotUI slot in boatSlots)
        {
            if (slot.GetID() == slotID)
            {
                return slot.HasStoredData();
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the slot data from the provided slot ID.
    /// </summary>
    /// <param name="slotID">The slot to get the data from.</param>
    /// <returns>Null if there is no data, or the InvItemSO data.</returns>
    private InvItemSO GetSlotData(int slotID)
    {
        foreach (InventorySlotUI slot in inventorySlots)
        {
            if (slot.GetID() == slotID)
            {
                return slot.GetStoredData();
            }
        }

        foreach (InventorySlotUI slot in boatSlots)
        {
            if (slot.GetID() == slotID)
            {
                return slot.GetStoredData();
            }
        }

        return null;
    }

    /// <summary>
    /// Remove the item at that slot by setting the data to null.
    /// </summary>
    /// <param name="slotID">The slot ID to remove the data from.</param>
    private void RemoveItemAtSlot(int slotID)
    {
        foreach (InventorySlotUI slot in inventorySlots)
        {
            if (slot.GetID() == slotID)
            {
                slot.SetItemData(null);
                return;
            }
        }

        foreach (InventorySlotUI slot in boatSlots)
        {
            if (slot.GetID() == slotID)
            {
                slot.SetItemData(null);
                return;
            }
        }
    }

    /// <summary>
    /// Spawns the item prefab and removes the item from the slot.
    /// </summary>
    /// <param name="slotID">The slot ID to drop the item.</param>
    public void DropItemInSlot(int slotID)
    {

        // print("Dropped Item");
        foreach (InventorySlotUI slot in inventorySlots)
        {
            if (slot.GetID() == slotID)
            {
                SpawnItem(slot.GetStoredData().Prefab);
                slot.SetItemData(null);
                return;
            }
        }

        foreach (InventorySlotUI slot in boatSlots)
        {
            if (slot.GetID() == slotID)
            {
                SpawnItem(slot.GetStoredData().Prefab);
                slot.SetItemData(null);
                return;
            }
        }
    }

    /// <summary>
    /// Sets the image alpha for that slot.
    /// </summary>
    /// <param name="slotID">The slot ID to set the alpha of.</param>
    /// <param name="alpha">The alpha value between 0f and 1f.</param>
    private void SetSlotAlpha(int slotID, float alpha)
    {
        foreach (InventorySlotUI slot in inventorySlots)
        {
            if (slot.GetID() == slotID)
            {
                slot.SetImageAlpha(alpha);
                return;
            }
        }

        foreach (InventorySlotUI slot in boatSlots)
        {
            if (slot.GetID() == slotID)
            {
                slot.SetImageAlpha(alpha);
                return;
            }
        }
    }

    /// <summary>
    /// Sets the image of the slot with the given sprite. Null will reset it.
    /// </summary>
    /// <param name="slotID">The slot ID to set the image of.</param>
    /// <param name="image">The new image if there is one. null will reset.</param>
    private void SetSlotImage(int slotID, Sprite image)
    {
        foreach (InventorySlotUI slot in inventorySlots)
        {
            if (slot.GetID() == slotID)
            {
                slot.SetSlotImage(image);
                return;
            }
        }

        foreach (InventorySlotUI slot in boatSlots)
        {
            if (slot.GetID() == slotID)
            {
                slot.SetSlotImage(image);
                return;
            }
        }
    }

    /// <summary>
    /// Please change this to what ever, this is how items are spawned into the world.
    /// </summary>
    /// <param name="prefab">The prefab associated with the item.</param>
    private void SpawnItem(GameObject prefab)
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// Try and add the item to the inventory.
    /// </summary>
    /// <param name="data">The data to store.</param>
    /// <returns>TRUE if sucessful. FALSE if this failed to find space.</returns>
    public bool AttemptAddItemToInventory(InvItemSO data)
    {
        int slotID = FindEmptyInvSlot();

        if (slotID == -1)
        {
            return false;
        }

        AddItemToSlot(slotID, data);

        return true;

    }


}
