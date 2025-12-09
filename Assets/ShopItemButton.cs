using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemButton : MonoBehaviour
{
    public Image Icon;
    public TMP_Text NameText;
    public TMP_Text PriceText;
    private VendorSO.VendorStockEntry entry;

    public void Setup(VendorSO.VendorStockEntry newEntry)
    {
        entry = newEntry;
        Icon.sprite = entry.Item.Icon;
        NameText.text = entry.ItemName;
        PriceText.text = "$" + entry.Price;
    }

    public void Buy()
    {
        ShopManager.Instance.BuyItem(entry);

        if (Inventory.Instance.moneyText != null && EconomyManager.instance != null)
        {
            Inventory.Instance.moneyText.text = "$" + EconomyManager.instance.Currency;
        }
    }
}