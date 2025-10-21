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
    public Sprite Icon;
    public GameObject Prefab;

    // Data
    public string Name = "Bob";

    [TextArea(1, 150)] // god fobid you exceed this amount.
    public string Description = "Lorem Ipsum";


    public int Worth = 1;
}
