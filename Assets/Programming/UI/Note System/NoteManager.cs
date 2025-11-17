using UnityEngine;

public class NoteManager : Interactable
{
    public GameObject noteUI;
    public PlayerController player;

    private void Start()
    {
        noteUI.SetActive(false);
    }

    public override void OnInteract()
    {
        if (noteUI.activeSelf)
        {
            player.enabled = true;
            noteUI.SetActive(false);
        }
        else
        {
            player.enabled = false;
            noteUI.SetActive(true);
        }
    }
}
