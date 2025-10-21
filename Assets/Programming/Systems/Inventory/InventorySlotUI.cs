using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|

/// <summary>
/// This manages the inventory slots.
/// </summary>
public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image Icon;
    private int ID;


    private InvItemSO storedData = null;

    void Awake()
    {
        if (Icon == null)
        {
            Icon = GetComponent<Image>();

            if (Icon != null)
            {
                Debug.LogError($"{nameof(Icon)} is null and I couldn't find a {nameof(Image)} component");
            }
        }
    }

    /// <summary>
    /// Sets the id associated to this slot.
    /// </summary>
    /// <param name="id">The id given to this slot.</param>
    public void SetUp(int id)
    {
        ID = id;
    }

    /// <summary>
    /// Sets the stored data with the provided data.
    /// </summary>
    /// <param name="data">The data to store.</param>
    public void SetItemData(InvItemSO data)
    {
        if (data == null)
        {
            Icon.sprite = null;
            storedData = null;
            return;
        }

        Icon.sprite = data.Icon;
        storedData = data;

        // fix alpha from hover system.
        SetImageAlpha();
    }

    /// <summary>
    /// Gets the slot's ID.
    /// </summary>
    /// <returns>ID associated to this slot.</returns>
    public int GetID()
    {
        return ID;
    }

    /// <summary>
    /// Sets the icon's alpha.
    /// </summary>
    /// <param name="alpha">Alpha between 0f and 1f.</param>
    public void SetImageAlpha(float alpha = 1f)
    {
        Color imageColor = Icon.color;
        imageColor.a = alpha;
        Icon.color = imageColor;
    }

    /// <summary>
    /// Checks to see if there is any data in this slot.
    /// </summary>
    /// <returns>TRUE if there is data.</returns>
    public bool HasStoredData()
    {
        return storedData != null;
    }

    /// <summary>
    /// Returns the stored data.
    /// </summary>
    /// <returns>The data that is stored.</returns>
    public InvItemSO GetStoredData()
    {
        return storedData;
    }

    /// <summary>
    /// Sets the slot's icon, null will use the one stored inside.
    /// </summary>
    /// <param name="image">The sprite to replace with.</param>
    public void SetSlotImage(Sprite image)
    {
        if (image != null)
            Icon.sprite = image;
        else if (HasStoredData())
            Icon.sprite = storedData.Icon;
        else
            Icon.sprite = null;
    }

    // Pointer events to know when the mouse is hovering over this.
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Inventory.Instance.SelectSlot(ID);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Inventory.Instance.DeselectSlot();

    }
}
