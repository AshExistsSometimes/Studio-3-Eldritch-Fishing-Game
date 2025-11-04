using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuySellSystem : MonoBehaviour
{
    //public Inventory inventory;
    //public PlayerMovement player;

    //[SerializeField] private GameObject shopInventoryObject;

    //[SerializeField] private GameObject inventorySlotPrefab;

    //[SerializeField] private GameObject shopSlotParent;

    //[SerializeField] private GameObject sellSlotParent;

    //[SerializeField] private TMP_Text moneyText;

    //List<InventorySlotUI> shopSlots = new List<InventorySlotUI>();
    //List<InventorySlotUI> sellSlots = new List<InventorySlotUI>();

    //int shopInvSize = 5;

    //int sellInvSize = 1;

    //int money = 0;

    //private void Start()
    //{
    //    SetUpSlots();
    //    moneyText.text = "$" + money;
    //}

    //public void OpenInventory()
    //{
    //    inventory.CursorIcon.SetActive(true);
    //    player.canMove = false;
    //    Cursor.lockState = CursorLockMode.None;
    //    inventory.inventoryOpen = true;
    //    inventory.inventoryObject.SetActive(true);
    //    shopInventoryObject.SetActive(true);
    //}

    //public void CloseInventory()
    //{
    //    inventory.CursorIcon.SetActive(false);
    //    player.canMove = true;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    inventory.inventoryOpen = false;
    //    inventory.inventoryObject.SetActive(false);
    //    shopInventoryObject.SetActive(false);
    //}

    //private void SetUpSlots()
    //{
    //    int globalIDTracker = 0;
    //    for (int i = 0; i < shopInvSize; i++)
    //    {
    //        GameObject slot = Instantiate(inventorySlotPrefab, shopSlotParent.transform);
    //        InventorySlotUI slotUI = slot.GetComponent<InventorySlotUI>();
    //        slotUI.SetUp(globalIDTracker);
    //        shopSlots.Add(slotUI);
    //        globalIDTracker++;
    //    }

    //    for (int i = 0; i < sellInvSize; i++)
    //    {
    //        GameObject slot = Instantiate(inventorySlotPrefab, sellSlotParent.transform);
    //        InventorySlotUI slotUI = slot.GetComponent<InventorySlotUI>();
    //        slotUI.SetUp(globalIDTracker);
    //        sellSlots.Add(slotUI);
    //        globalIDTracker++;
    //    }
    //}
}