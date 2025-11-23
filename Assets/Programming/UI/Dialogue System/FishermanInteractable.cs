using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class FishermanInteractable : Interactable
{
    public GameObject tutorialChoice;
    public GameObject yesButton;
    public GameObject noButton;
    public GameObject pressSpaceText;

    private TutorialDialogueSystem dialogue;

    private void Start()
    {
        dialogue = tutorialChoice.GetComponent<TutorialDialogueSystem>();
    }

    private void Update()
    {
        if (dialogue.dialogueText.text == dialogue.lines[dialogue.index] && dialogue.index == 0)
        {
            yesButton.SetActive(true);
            noButton.SetActive(true);
        }
    }

    public override void OnInteract()
    {
        yesButton.SetActive(false);
        noButton.SetActive(false);

        dialogue.canPressSpace = false;
        pressSpaceText.SetActive(false);
        tutorialChoice.SetActive(true);
    }
}
