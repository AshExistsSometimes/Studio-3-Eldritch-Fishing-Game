using UnityEngine;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|

// modify this to your needs.

[CreateAssetMenu(fileName = "InvItem", menuName = "SO/InvItem")]
public class InvItemSO : ScriptableObject
{

    // Data
    public string Name = "Item";

    [TextArea(1, 150)] // god fobid you exceed this amount.
    public string Description = "Lorem Ipsum";
    [Space]
    public Sprite Icon;
    public GameObject Prefab;
    [Space]

    // Added / altered by Ashley ------------
    [Tooltip("Can the item put in the hotbar to be held in the hand")]
    public bool Equippable = false;
    [Tooltip ("Can the item be used when held in the hand")]
    public bool Usable = false;
    [Space]
    public int BaseSellValue = 1;
    public float WeirdnessPenalty = 0f;
    [Header("Fish Exclusive"), Tooltip("1 is default, applies a multiplier to value if higher")]
    public float FishSize = 1f;
    //---------------------------------------
}
