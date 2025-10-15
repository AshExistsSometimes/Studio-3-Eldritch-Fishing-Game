using UnityEngine;

[CreateAssetMenu(fileName = "Fish", menuName = "Item/Fish")]
////////////////////////////////////////////////////////////////////
public class FishSO : ScriptableObject
{
    [Header("Fish Data")]
    public string fishName;
    public GameObject fishPrefab;
    public string fishDescription;
    public float weirdnessLevel;
    public float difficulty;
    public float persistence;
    public Vector2 sizeVariance;
}

////////////////////////////////////////////////////////////////////
