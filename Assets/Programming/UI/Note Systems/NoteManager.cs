using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteManager : Interactable
{
    public GameObject noteUI;
    public PlayerController player;

    public static NoteManager Instance { get; private set; }

    [Header("Unlocked Data (runtime)")]
    public List<NoteSO> UnlockedNotes = new List<NoteSO>();
    public List<FishSO> UnlockedFish = new List<FishSO>();

    [Header("Fish Details UI")]
    [Tooltip("Parent transform which will receive FishDetailUI instances (Vertical Layout recommended).")]
    public Transform FishDetailsParent;
    [Tooltip("Prefab with FishDetailUI component")]
    public GameObject FishDetailUIPrefab;

    [Header("Notes UI")]
    public TMP_Text NotesHeaderText;
    public TMP_Text NotesBodyText;
    public Button NotesPrevButton;
    public Button NotesNextButton;

    private List<GameObject> spawnedFishEntries = new List<GameObject>();

    // Note navigation
    private int currentNoteIndex = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Wire simple button callbacks if assigned
        if (NotesPrevButton != null) NotesPrevButton.onClick.AddListener(PrevNote);
        if (NotesNextButton != null) NotesNextButton.onClick.AddListener(NextNote);
    }

    private void Start()
    {
        // Ensure UI reflects any pre-unlocked entries (useful for editor testing)
        RefreshFishUI();
        RefreshNotesUI();
    }

    // ---------------------------
    // Public API
    // ---------------------------

    /// <summary>
    /// Unlock a fish (called when a fish is caught). If already unlocked, does nothing.
    /// Will instantiate a FishDetailUI prefab under FishDetailsParent.
    /// </summary>
    public void UnlockFish(FishSO fish)
    {
        if (fish == null) return;

        if (UnlockedFish.Contains(fish)) return;

        UnlockedFish.Add(fish);
        AddFishEntryToUI(fish);
    }

    /// <summary>
    /// Unlock a note (called by Note interactables). If already unlocked, does nothing.
    /// Adds the note to the unlocked list and, if this is the first note, selects it.
    /// </summary>
    public void UnlockNote(NoteSO note)
    {
        if (note == null) return;

        if (UnlockedNotes.Contains(note)) return;

        UnlockedNotes.Add(note);

        // If there was previously no note selected, select this one
        if (currentNoteIndex == -1)
            currentNoteIndex = 0;

        RefreshNotesUI();
    }

    /// <summary>
    /// Move to previous note in the unlocked list.
    /// </summary>
    public void PrevNote()
    {
        if (UnlockedNotes.Count == 0) return;
        currentNoteIndex--;
        if (currentNoteIndex < 0) currentNoteIndex = UnlockedNotes.Count - 1;
        RefreshNotesUI();
    }

    /// <summary>
    /// Move to next note in the unlocked list.
    /// </summary>
    public void NextNote()
    {
        if (UnlockedNotes.Count == 0) return;
        currentNoteIndex++;
        if (currentNoteIndex >= UnlockedNotes.Count) currentNoteIndex = 0;
        RefreshNotesUI();
    }

    // ---------------------------
    // UI helpers
    // ---------------------------

    private void AddFishEntryToUI(FishSO fish)
    {
        if (FishDetailUIPrefab == null || FishDetailsParent == null)
        {
            Debug.LogWarning("NoteManager: FishDetailUIPrefab or FishDetailsParent not assigned.");
            return;
        }

        // Instantiate and populate
        GameObject go = Instantiate(FishDetailUIPrefab, FishDetailsParent);
        var ui = go.GetComponent<FishDetailUI>();
        if (ui != null)
            ui.SetData(fish);
        else
            Debug.LogWarning("FishDetailUIPrefab is missing FishDetailUI component.");

        spawnedFishEntries.Add(go);
    }

    private void RefreshFishUI()
    {
        // Clear existing spawned entries (safe on start)
        foreach (var g in spawnedFishEntries)
            if (g != null) Destroy(g);
        spawnedFishEntries.Clear();

        // Recreate entries in unlocked order
        foreach (var fish in UnlockedFish)
            AddFishEntryToUI(fish);
    }

    private void RefreshNotesUI()
    {
        if (UnlockedNotes == null || UnlockedNotes.Count == 0)
        {
            // No notes unlocked: show empty state
            if (NotesHeaderText != null) NotesHeaderText.text = "No Notes";
            if (NotesBodyText != null) NotesBodyText.text = "Find notes to unlock journal entries.";
            return;
        }

        // Clamp index and display
        currentNoteIndex = Mathf.Clamp(currentNoteIndex, 0, UnlockedNotes.Count - 1);
        NoteSO note = UnlockedNotes[currentNoteIndex];

        if (NotesHeaderText != null) NotesHeaderText.text = note.NoteHeader;
        if (NotesBodyText != null) NotesBodyText.text = note.NoteText;
    }

    // Optional: expose a method to clear all notes/fish (editor/debug)
    public void ClearAll()
    {
        UnlockedNotes.Clear();
        UnlockedFish.Clear();
        currentNoteIndex = -1;

        foreach (var g in spawnedFishEntries)
            if (g != null) Destroy(g);
        spawnedFishEntries.Clear();

        RefreshNotesUI();
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

            RefreshFishUI();
            RefreshNotesUI();
        }
    }
}
