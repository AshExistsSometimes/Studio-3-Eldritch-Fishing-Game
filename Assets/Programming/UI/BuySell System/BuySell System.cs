using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuySellSystem : MonoBehaviour
{
    public Inventory inventory;
    public PlayerMovement player;

    [SerializeField] private GameObject shopInventoryObject;

    [SerializeField] private GameObject inventorySlotPrefab;

    [SerializeField] private GameObject shopSlotParent;

    [SerializeField] private GameObject sellSlotParent;

    [SerializeField] private TMP_Text moneyText;

    List<InventorySlotUI> shopSlots = new List<InventorySlotUI>();
    List<InventorySlotUI> sellSlots = new List<InventorySlotUI>();

    int shopInvSize = 5;

    int sellInvSize = 1;

    int money = 0;

    private void Start()
    {
        SetUpSlots();
        moneyText.text = "$" + money;
    }

    public void OpenInventory()
    {
        inventory.CursorIcon.SetActive(true);
        player.canMove = false;
        Cursor.lockState = CursorLockMode.None;
        inventory.inventoryOpen = true;
        inventory.inventoryObject.SetActive(true);
        shopInventoryObject.SetActive(true);
    }

    public void CloseInventory()
    {
        inventory.CursorIcon.SetActive(false);
        player.canMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        inventory.inventoryOpen = false;
        inventory.inventoryObject.SetActive(false);
        shopInventoryObject.SetActive(false);
    }

    private void SetUpSlots()
    {
        int globalIDTracker = 0;
        for (int i = 0; i < shopInvSize; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, shopSlotParent.transform);
            InventorySlotUI slotUI = slot.GetComponent<InventorySlotUI>();
            slotUI.SetUp(globalIDTracker);
            shopSlots.Add(slotUI);
            globalIDTracker++;
        }

        for (int i = 0; i < sellInvSize; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, sellSlotParent.transform);
            InventorySlotUI slotUI = slot.GetComponent<InventorySlotUI>();
            slotUI.SetUp(globalIDTracker);
            sellSlots.Add(slotUI);
            globalIDTracker++;
        }
    }

        ////Parameters
        //public string merchantName;
        //public float merchantBuyingFromMultiplier;
        //public float merchantSellingToMultiplier;
        //public float errorMessageDuration;

        ////References
        //public GameObject buySellUI;
        //public GameObject buyingButton;
        //public GameObject sellingButton;
        //public Transform playerItemButtonsParent;
        //public Transform merchantItemButtonsParent;
        //public GameObject notEnoughGoldText;
        //public GameObject noInventorySpaceText;
        //public GameObject buyingUI;
        //public GameObject sellingUI;

        ////Prefabs
        //public GameObject playerItemButtonPrefab;
        //public GameObject merchantItemButtonPrefab;

        ////State for whether player is currently buying or selling
        //private enum State
        //{
        //    Buying,
        //    Selling
        //}
        //private State currentState;


        //////////////////////////////////////////////////////////////////////
        //private void Awake()
        //{
        //    ToggleUIVisibility(false);
        //}

        //////////////////////////////////////////////////////////////////////
        //private void Update()
        //{
        //    ToggleUIToShow(currentState);
        //    if (Input.GetKeyDown(InputManager.GetKeyCode("OpenInventory")))// PLEASE ENSURE IT CLOSES WITH THE "CloseMenu" KEYBIND TOO
        //    {
        //        ToggleUIVisibility(true);
        //    }
        //}

        //////////////////////////////////////////////////////////////////////
        //private void BuyItemFromMerchant(InventoryManager.Item itemToBuy)
        //{
        //    if (true) //Player has enough money
        //    {
        //        if (InventoryManager.instance.AttemptToAddItemToInventory(itemToBuy))
        //        {
        //            buyableItems.Remove(itemToBuy);
        //        }
        //        else
        //        {
        //            if (!notEnoughGoldText.activeSelf)
        //            {
        //                StartCoroutine(DisplayErrorMessage(noInventorySpaceText));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (!notEnoughGoldText.activeSelf)
        //        {
        //            StartCoroutine(DisplayErrorMessage(notEnoughGoldText));
        //        }
        //    }

        //    UpdateItemsForSaleUI();
        //}

        //////////////////////////////////////////////////////////////////////
        //private void SellItemToMerchant(InventoryManager.Item itemToSell)
        //{
        //    InventoryManager.instance.AttemptToRemoveItemFromInventory(itemToSell);

        //    //Add gold to player 

        //    UpdateItemsForSaleUI();
        //}
        //////////////////////////////////////////////////////////////////////
        //private void UpdateItemsForSaleUI()
        //{
        //    foreach (Transform child in playerItemButtonsParent)
        //    {
        //        Destroy(child.gameObject);
        //    }
        //    foreach (Transform child in merchantItemButtonsParent)
        //    {
        //        Destroy(child.gameObject);
        //    }

        //    foreach (InventoryManager.Item item in InventoryManager.instance.inventory)
        //    {
        //        GameObject buttonObj = Instantiate(playerItemButtonPrefab, playerItemButtonsParent);
        //        buttonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.originalSO.itemName;
        //        buttonObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Price " + item.sellValue.ToString();
        //        buttonObj.GetComponent<Button>().onClick.AddListener(() => SellItemToMerchant(item));
        //    }
        //    foreach (InventoryManager.Item item in buyableItems)
        //    {
        //        GameObject buttonObj = Instantiate(merchantItemButtonPrefab, merchantItemButtonsParent);
        //        buttonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.originalSO.itemName;
        //        buttonObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Price " + item.priceToBuy.ToString();
        //        buttonObj.GetComponent<Button>().onClick.AddListener(() => BuyItemFromMerchant(item));
        //    }
        //}

        //////////////////////////////////////////////////////////////////////
        //private void ToggleUIToShow(State stateToSwapTo)
        //{
        //    if (stateToSwapTo == State.Buying)
        //    {
        //        buyingUI.SetActive(true);
        //        sellingUI.SetActive(false);
        //    }
        //    else if (stateToSwapTo == State.Selling)
        //    {
        //        buyingUI.SetActive(false);
        //        sellingUI.SetActive(true);
        //    }
        //}

        //////////////////////////////////////////////////////////////////////
        //private void ToggleUIVisibility(bool stateToToggleTo)
        //{
        //    UpdateItemsForSaleUI();
        //    buySellUI.SetActive(stateToToggleTo);
        //}

        //////////////////////////////////////////////////////////////////////
        //public void BuyingButton()
        //{
        //    currentState = State.Buying;
        //}

        //////////////////////////////////////////////////////////////////////
        //public void SellingButton()
        //{
        //    currentState = State.Selling;
        //}

        //////////////////////////////////////////////////////////////////////
        //private IEnumerator DisplayErrorMessage(GameObject errorMessageToDisplay)
        //{
        //    errorMessageToDisplay.SetActive(true);
        //    yield return new WaitForSeconds(errorMessageDuration);
        //    errorMessageToDisplay.SetActive(false);
        //}
}