using UnityEngine;

[CreateAssetMenu(fileName = "Vendor", menuName = "SO/Vendor")]
public class VendorSO : ScriptableObject
{
    public string VendorName;
    public Sprite VendorPortrait;

    [Tooltip("If TRUE: Vendor sells items as well ass being able to be sold to")]
    public bool CanBuyFrom = true;
    [Tooltip("If TRUE: Vendor applies weirdness penalty when sold to")]
    public bool AppliesWeirdnessPenaltyWhenSelling = false;

    [System.Serializable]
    public struct VendorStockEntry
    {
        public string ItemName;
        public InvItemSO Item;
        public int Price;

        [Header("Unlock Conditions (Optional)")]
        public InvItemSO RequiredItem; // Leave null to ignore
        public float RequiredWeirdness; // Set -1 to ignore

        //[Header("Optional Purchase Effects")]
        //public UnityEvent OnPurchase;
    }

    public VendorStockEntry[] Stock;
}
