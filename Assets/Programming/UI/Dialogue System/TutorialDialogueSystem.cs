using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TutorialDialogueSystem : MonoBehaviour
{
    public PlayerController player;

    public TextMeshProUGUI dialogueText;
    public string[] lines;
    public float textSpeed;

    public bool canPressSpace;

    [HideInInspector]
    public int index;

    public void OnEnable()
    {
        StartDialogue();
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Input.GetMouseButtonDown(0) && canPressSpace)
        {
            if (dialogueText.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = lines[index];
            }
        }
    }

    public void StartDialogue()
    {
        player.enabled = false;
        dialogueText.text = string.Empty;

        index = 0;
        StartCoroutine(TypeLine());
    }

    public void NextLine()
    {
        dialogueText.text = string.Empty;

        if (index < lines.Length - 1)
        {
            index++;
            dialogueText.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
            player.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void PrintWholeLine()
    {
        dialogueText.text = lines[index];
    }

    public void EnablePressingSpace()
    {
        canPressSpace = true;
    }
}
