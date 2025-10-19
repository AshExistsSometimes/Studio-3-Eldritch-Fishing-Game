using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

////////////////////////////////////////////////////////////////////
public class BoatInventoryItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    ////////////////////////////////////////////////////////////////////
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Updates variables for parent script showing what is currently hovered over
        int index = transform.GetSiblingIndex();

        InventoryManager.instance.mouseHoveringOverSomething = true;
        InventoryManager.instance.indexOfMouseHover = index;
        InventoryManager.instance.hoveredItemIsInBoatInv = true;
    }

    ////////////////////////////////////////////////////////////////////
    public void OnPointerExit(PointerEventData eventData)
    {
        //Updates variables for parent script showing what is currently hovered over
        InventoryManager.instance.mouseHoveringOverSomething = false;
    }

    ////////////////////////////////////////////////////////////////////
    public void OnClick()
    {
        if (InventoryManager.currentState == InventoryManager.States.InBoatInventory)
        {
            if (BoatInventoryManager.instance.inventory.Count > transform.GetSiblingIndex())
            {
                if (InventoryManager.instance.AttemptToAddItemToInventory(BoatInventoryManager.instance.inventory[BoatInventoryManager.instance.indexOfHighlightedItem]))
                {
                    BoatInventoryManager.instance.AttemptToRemoveItemFromInventory(BoatInventoryManager.instance.inventory[BoatInventoryManager.instance.indexOfHighlightedItem]);
                    BoatInventoryManager.instance.UpdateInventoryUI();
                }
                else
                {
                    Debug.Log("No space in player inventory");
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////
}
