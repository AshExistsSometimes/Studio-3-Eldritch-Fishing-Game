using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Note : Interactable
{
    [Header("Note Data")]
    public NoteSO noteSO;

    [Header("Behavior")]
    public bool disableOnPickup = true;

    private void Awake()
    {
        if (NoteManager.Instance.UnlockedNotes.Contains(noteSO))
        {
            gameObject.SetActive(false);
        }
    }

    public override void OnInteract()
    {
        if (noteSO == null)
        {
            Debug.LogWarning("Note interacted but NoteSO is not assigned.");
            return;
        }

        if (NoteManager.Instance == null)
        {
            Debug.LogWarning("NoteManager not present in scene.");
            return;
        }

        NoteManager.Instance.UnlockNote(noteSO);

        if (disableOnPickup)
            gameObject.SetActive(false);
    }
}
