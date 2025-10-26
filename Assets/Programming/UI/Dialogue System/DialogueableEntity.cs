using UnityEngine;

////////////////////////////////////////////////////////////////////
public class DialogueableEntity : Interactable
{
    //Parameters 
    public string characterName;
public Dialogue dialogue;

    public override void OnInteract()
    {
            InitiateDialogue();
    }

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
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            DialogueManager.instance.DisableDialogueUI();
        }
    }

    ////////////////////////////////////////////////////////////////////
}