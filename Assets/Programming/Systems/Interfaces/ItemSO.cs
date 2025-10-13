using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Image itemImage;
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
