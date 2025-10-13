using UnityEngine;

////////////////////////////////////////////////////////////////////
public class DialogueableEntity : MonoBehaviour
{
    //Parameters 
    public string characterName;
    public Dialogue dialogue;

    ////////////////////////////////////////////////////////////////////
    public void InitiateDialogue()
    {
        DialogueManager.instance.StartDialogue(characterName, dialogue.rootNode);
    }

    ////////////////////////////////////////////////////////////////////
    //For testing only
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InitiateDialogue();
        }
    }

    ////////////////////////////////////////////////////////////////////
}