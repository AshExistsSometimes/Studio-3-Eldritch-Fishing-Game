using UnityEngine;

[CreateAssetMenu(fileName = "NewNote", menuName = "Data/Note")]
public class NoteSO : ScriptableObject
{
    [Header("Note Data")]
    public string NoteName;
    public string NoteHeader;
    [TextArea(3, 10)]
    public string NoteText;
}
