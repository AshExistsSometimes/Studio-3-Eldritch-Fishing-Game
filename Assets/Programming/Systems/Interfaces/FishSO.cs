using UnityEngine;

[CreateAssetMenu(fileName = "Fish", menuName = "Item/Fish")]
////////////////////////////////////////////////////////////////////
public class FishSO : ScriptableObject
{
    [Header("Fish Data")]
    public string fishName;
    public string fishDescription;
    public GameObject fishPrefab;

    [Header("Minigame Stats")]
    public float weirdnessLevel;
    public float difficulty;
    public float persistence;
    public float agility;
    public Vector2 sizeVariance;

    [Header("Other")]
    public float WeirdnessPenalty;
    public float BaseSellValue;
}

////////////////////////////////////////////////////////////////////
