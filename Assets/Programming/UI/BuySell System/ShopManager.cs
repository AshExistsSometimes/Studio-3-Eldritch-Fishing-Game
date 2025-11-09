using UnityEngine;

/// <summary>
/// Handles opening vendors, buying and selling with vendors.
/// Uses EconomyManager.instance for money operations.
/// </summary>
public class ShopManager : MonoBehaviour
{
    [SerializeField] private Transform ShopItemContainer;
    [SerializeField] private GameObject ShopItemButtonPrefab;

    public static ShopManager Instance;

    private SceneManager sceneManager;

    public VendorSO ActiveVendor;

    private void Awake()
    {
        Instance = this;

        sceneManager = GetComponent<SceneManager>();
    }

    public void OpenVendor(VendorSO vendor)
    {
        ActiveVendor = vendor;

        // Show player inventory, hide boat inventory, show shop display
        Inventory.Instance.OpenInventory(false, true);

        // Populate shop UI panel
        PopulateShopUI();
    }

    public void CloseVendor()
    {
        ActiveVendor = null;
    }

    private void PopulateShopUI()
    {
        // Clear old items
        foreach (Transform child in ShopItemContainer)
            Destroy(child.gameObject);

        foreach (var entry in ActiveVendor.Stock)
        {
            bool unlocked = true;

            if (entry.RequiredItem != null)
                unlocked &= Inventory.Instance.DoesPlayerOwnItem(entry.RequiredItem);

            if (entry.RequiredWeirdness >= 0)
                unlocked &= sceneManager.Weirdness >= entry.RequiredWeirdness;

            if (!unlocked) continue;

            var buttonObj = Instantiate(ShopItemButtonPrefab, ShopItemContainer);
            var button = buttonObj.GetComponent<ShopItemButton>();
            button.Setup(entry);
        }
    }

    public void BuyItem(VendorSO.VendorStockEntry entry)
    {
        if (EconomyManager.instance.Currency < entry.Price) return;

        EconomyManager.instance.TryRemoveMoney(entry.Price);

        Inventory.Instance.AttemptAddItemToInventory(entry.Item);
    }

    /// <summary>
    /// Sell an InvItemSO to the currently active vendor.
    /// Uses InvItemSO fields (IsFish, BaseSellValue, WeirdnessPenalty).
    /// </summary>
    public void SellItem(InvItemSO item)
    {
        if (item == null || ActiveVendor == null) return;

        int value = item.BaseSellValue;
        float weirdnessToAdd = item.WeirdnessPenalty;

        // If vendor applies weirdness penalty on selling, add it to scene manager
        if (ActiveVendor.AppliesWeirdnessPenaltyWhenSelling)
        {
            sceneManager.Weirdness += weirdnessToAdd;
        }

        // Add money via your EconomyManager singleton
        EconomyManager.instance.AddMoney(value);

        // Show popup at mouse position
        SellPopup.Instance.ShowPopup(Input.mousePosition, value, weirdnessToAdd, ActiveVendor.AppliesWeirdnessPenaltyWhenSelling);
    }
}
