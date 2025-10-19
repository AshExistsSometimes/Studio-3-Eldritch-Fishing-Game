//using System.Collections.Generic;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////
public class BoatInventoryManager : InventoryManager
{
    //Instance of BoatInventoryManager
    public static BoatInventoryManager instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject boatInventorySlot;

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

        inventory = new List<Item>();
        CreateInventoryUI();
        boatInventoryUI.SetActive(false);
    }


    ////////////////////////////////////////////////////////////////////
    protected override void CreateInventoryUI()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject iSlot = Instantiate(boatInventorySlot, inventorySlotsParent.transform);
            if (inventory.Count > i)
            {
                iSlot.GetComponent<Image>().sprite = inventory[i].originalSO.image;
            }
            else
            {
                iSlot.GetComponent<Image>().sprite = null;
            }

        }
    }

    ////////////////////////////////////////////////////////////////////
    public override void UpdateInventoryUI()
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
        }
    }


    ////////////////////////////////////////////////////////////////////
   
}


