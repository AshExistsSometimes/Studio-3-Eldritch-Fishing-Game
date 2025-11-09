using UnityEngine;

public class VendorInteraction : Interactable
{
    public VendorSO vendorSO;

    public override void OnInteract()
    {
        ShopManager.Instance.OpenVendor(vendorSO);
    }
}
