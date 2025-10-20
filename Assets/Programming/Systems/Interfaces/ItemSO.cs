using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Item/Item")]
////////////////////////////////////////////////////////////////////
public class ItemSO : ScriptableObject
{
    [Header("Item Data")]
    public string itemName;
    public Sprite image;
    public GameObject prefab;
    public int priceToBuy;
    public int baseSellValue;
    public string itemDescription;

    [Header("Parameters")]
    public bool isUsable;
    public bool isSellable;
    public bool isDroppable;

    public bool isFish = false;

}

////////////////////////////////////////////////////////////////////