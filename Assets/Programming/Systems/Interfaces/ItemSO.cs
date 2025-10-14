using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Image itemImage;
    public bool canBeSold;
    public int priceToBuy;
    public int sellValue;
    public bool isAFish;

    public GameObject ModelPrefab;

    public StatToChange statToChange = new StatToChange();
    public int StatChangeAmount;

    public void SellItem()
    {
        if (statToChange == StatToChange.worth)
        {

        }
    }

    public enum StatToChange
    {
        none,
        worth
    }
}
