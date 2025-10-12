using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/DialogueNode")]
////////////////////////////////////////////////////////////////////
public class DialogueNode: ScriptableObject
{
    //Components of dialogue node
    public string dialogueText;
    public List<DialogueResponse> availableResponses;

    //Identifies is current node is last in dialogue
    public bool IsLastNode()
    {
        return availableResponses.Count <= 0;
    }
}

////////////////////////////////////////////////////////////////////