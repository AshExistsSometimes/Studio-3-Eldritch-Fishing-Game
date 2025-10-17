using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////
public class BuySellSystem : MonoBehaviour
{
    //Parameters
    public string merchantName;
    public float merchantBuyingFromMultiplier;
    public float merchantSellingToMultiplier;
    public float errorMessageDuration;

    //References
    public GameObject buySellUI;
    public GameObject buyingButton;
    public GameObject sellingButton;
    public Transform playerItemButtonsParent;
    public Transform merchantItemButtonsParent;
    public InventoryManager inventoryManager;
    public GameObject notEnoughGoldText;
    public GameObject noInventorySpaceText;
    public GameObject buyingUI;
    public GameObject sellingUI;

    //Prefabs
    public GameObject playerItemButtonPrefab;
    public GameObject merchantItemButtonPrefab;

    //State for whether player is currently buying or selling
    private enum State
    {
        Buying,
        Selling
    }
    private State currentState;

    //List of avail items to buy
    public List<ItemSO> buyableItems;

    ////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        ToggleUIVisibility(false);
    }

    ////////////////////////////////////////////////////////////////////
    private void Update()
    {
        ToggleUIToShow(currentState);
        if (Input.GetKeyDown(InputManager.GetKeyCode("OpenInventory")))// PLEASE ENSURE IT CLOSES WITH THE "CloseMenu" KEYBIND TOO
        {
            ToggleUIVisibility(true);
        }
    }

    ////////////////////////////////////////////////////////////////////
    private void BuyItemFromMerchant(ItemSO itemToBuy)
    {
        if (true) //Player has enough money
        {
            if (InventoryManager.instance.AttemptToAddItemToInventory(itemToBuy))
            {
                buyableItems.Remove(itemToBuy);
            }
            else
            {
                if (!notEnoughGoldText.activeSelf)
                {
                    StartCoroutine(DisplayErrorMessage(noInventorySpaceText));
                }
            }
        }
        else
        {
            if (!notEnoughGoldText.activeSelf)
            {
                StartCoroutine(DisplayErrorMessage(notEnoughGoldText));
            }
        }

        UpdateItemsForSaleUI();
    }

    ////////////////////////////////////////////////////////////////////
    private void SellItemToMerchant(ItemSO itemToSell)
    {
        InventoryManager.instance.AttemptToRemoveItemFromInventory(itemToSell);

        //Add gold to player 

        UpdateItemsForSaleUI();
    }
    ////////////////////////////////////////////////////////////////////
    private void UpdateItemsForSaleUI()
    {
        foreach (Transform child in playerItemButtonsParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in merchantItemButtonsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemSO item in InventoryManager.instance.inventory)
        {
            GameObject buttonObj = Instantiate(playerItemButtonPrefab, playerItemButtonsParent);
            buttonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemName;
            buttonObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Price " + item.sellValue.ToString();
            buttonObj.GetComponent<Button>().onClick.AddListener(() => SellItemToMerchant(item));
        }
        foreach (ItemSO item in buyableItems)
        {
            GameObject buttonObj = Instantiate(merchantItemButtonPrefab, merchantItemButtonsParent);
            buttonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemName;
            buttonObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Price " + item.priceToBuy.ToString();
            buttonObj.GetComponent<Button>().onClick.AddListener(() => BuyItemFromMerchant(item));
        }
    }

    ////////////////////////////////////////////////////////////////////
    private void ToggleUIToShow(State stateToSwapTo)
    {
        if (stateToSwapTo == State.Buying)
        {
            buyingUI.SetActive(true);
            sellingUI.SetActive(false);
        }
        else if (stateToSwapTo == State.Selling)
        {
            buyingUI.SetActive(false);
            sellingUI.SetActive(true);
        }
    }

    ////////////////////////////////////////////////////////////////////
    private void ToggleUIVisibility(bool stateToToggleTo)
    {
        UpdateItemsForSaleUI();
        buySellUI.SetActive(stateToToggleTo);
    }

    ////////////////////////////////////////////////////////////////////
    public void BuyingButton()
    {
        currentState = State.Buying;
    }

    ////////////////////////////////////////////////////////////////////
    public void SellingButton()
    {
        currentState = State.Selling;
    }

    ////////////////////////////////////////////////////////////////////
    private IEnumerator DisplayErrorMessage(GameObject errorMessageToDisplay)
    {
        errorMessageToDisplay.SetActive(true);
        yield return new WaitForSeconds(errorMessageDuration);
        errorMessageToDisplay.SetActive(false);
    }
}

////////////////////////////////////////////////////////////////////