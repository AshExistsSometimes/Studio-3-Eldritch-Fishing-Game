using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Spawns and manages landmark prefabs around a player within defined radii,
/// respecting spawn limits, spacing, rarity, large island rules, and despawn behavior.
/// Integrates fish into the FishingMinigame pools dynamically based on player proximity.
/// </summary>
public class LandmarkSpawner : MonoBehaviour
{
    [System.Serializable]
    public class LandmarkData
    {
        [Tooltip("Prefab of the landmark to spawn.")]
        public GameObject Landmark;

        [Tooltip("Display name for the landmark instance.")]
        public string LandmarkName = "Default Landmark";

        [Tooltip("Maximum number of this landmark type allowed at once.")]
        public int MaxSpawnNum = 1;

        [Tooltip("Chance for this landmark to spawn when checked (0 = never, 1 = always).")]
        [Range(0f, 1f)]
        public float Rarity = 1f;

        [Tooltip("Range of random scale multipliers applied to the landmark.")]
        public Vector2 ScaleVariation = new Vector2(1f, 1f);

        [Tooltip("Minimum required distance between this landmark and another landmark.")]
        public float SpaceRequired = 50f;

        [Tooltip("Range of random height variation on the Y axis.")]
        public Vector2 HeightVariation = new Vector2(0f, 0f);

        [Tooltip("Optional list of fish associated with this landmark.")]
        public List<FishEntry> IslandFishPool = new List<FishEntry>();

        [Tooltip("Radius around landmark that fish spawn in.")]
        public float FishSpawnRadius = 5f;

        [HideInInspector] public List<GameObject> SpawnedInstances = new List<GameObject>();
        [HideInInspector] public int TargetSpawnCount = 0;

        [Tooltip("If true, this landmark is a large island.")]
        public bool IsLargeIsland = false;

        [HideInInspector] public bool PlayerInsideRadius = false; // Tracks fish pool state
    }

    [System.Serializable]
    public class FishEntry
    {
        public FishSO fish;
        public FishingMinigame.FishRarity rarity;
        public bool caughtAtNight = true;
        public bool locked = false;
    }

    [Header("Regular Islands")]
    public List<LandmarkData> Landmarks = new List<LandmarkData>();
    public GameObject Player;
    public float SpawnRadius = 200f;
    public float DespawnRadius = 250f;
    public float NoSpawnRadius = 100f;

    [Header("Large Islands")]
    public float LargeIslandSpawnRadius = 500f;
    public float LargeIslandSpawnRadiusBounds = 50f;

    private bool initialized = false;
    private bool respawnRequested = false;
    private FishingMinigame fishingMinigame;

    private void Start()
    {
        if (!Player)
            Player = GameObject.FindGameObjectWithTag("Player");

        fishingMinigame = FindObjectOfType<FishingMinigame>();

        InitializeSpawnTargets();
        InitialGenerate();
    }

    private void InitializeSpawnTargets()
    {
        foreach (var data in Landmarks)
            data.TargetSpawnCount = Random.Range(0, data.MaxSpawnNum + 1);

        initialized = true;
    }

    private void InitialGenerate()
    {
        foreach (var data in Landmarks)
        {
            if (!data.Landmark) continue;

            for (int i = 0; i < data.TargetSpawnCount; i++)
            {
                if (Random.value > data.Rarity) continue;

                Vector3 spawnPos;
                if (TryGetValidSpawnPosition(data, out spawnPos, true))
                    SpawnLandmarkInstance(data, spawnPos);
            }
        }
    }

    private void Update()
    {
        if (!initialized) InitializeSpawnTargets();

        bool anyDespawned = DespawnDistantLandmarks();
        if (anyDespawned) respawnRequested = true;

        if (respawnRequested)
        {
            RegenerateAfterDespawn();
            respawnRequested = false;
        }

        TrySpawnLandmarks();
        HandleFishPools();
    }

    private void TrySpawnLandmarks()
    {
        foreach (var data in Landmarks)
        {
            if (!data.Landmark || data.IsLargeIsland) continue;
            if (data.SpawnedInstances.Count >= data.TargetSpawnCount) continue;
            if (Random.value > data.Rarity) continue;

            Vector3 spawnPos;
            if (TryGetValidSpawnPosition(data, out spawnPos, false))
                SpawnLandmarkInstance(data, spawnPos);
        }

        // Handle large island
        LandmarkData largeIsland = Landmarks.Find(x => x.IsLargeIsland);
        if (largeIsland != null && largeIsland.SpawnedInstances.Count == 0)
        {
            Vector3 spawnPos;
            if (TryGetValidLargeIslandPosition(largeIsland, out spawnPos))
            {
                // Delete any overlapping landmarks
                foreach (var other in Landmarks)
                {
                    for (int i = other.SpawnedInstances.Count - 1; i >= 0; i--)
                    {
                        if (Vector3.Distance(other.SpawnedInstances[i].transform.position, spawnPos) < largeIsland.SpaceRequired)
                        {
                            Destroy(other.SpawnedInstances[i]);
                            other.SpawnedInstances.RemoveAt(i);
                        }
                    }
                }
                SpawnLandmarkInstance(largeIsland, spawnPos);
            }
        }
    }

    private void RegenerateAfterDespawn()
    {
        foreach (var data in Landmarks)
        {
            int missing = data.MaxSpawnNum - data.SpawnedInstances.Count;
            if (missing <= 0 || data.IsLargeIsland) continue;

            for (int i = 0; i < missing; i++)
            {
                if (Random.value > data.Rarity) continue;

                Vector3 spawnPos;
                if (TryGetValidSpawnPosition(data, out spawnPos, false))
                    SpawnLandmarkInstance(data, spawnPos);
            }
        }
    }

    private void SpawnLandmarkInstance(LandmarkData data, Vector3 position)
    {
        Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        GameObject obj = Instantiate(data.Landmark, position, rotation);
        obj.name = data.LandmarkName;

        float randomScale = Random.Range(data.ScaleVariation.x, data.ScaleVariation.y);
        obj.transform.localScale = Vector3.one * randomScale;

        float heightOffset = Random.Range(data.HeightVariation.x, data.HeightVariation.y );
        obj.transform.position = new Vector3(obj.transform.position.x, heightOffset, obj.transform.position.z);

        data.SpawnedInstances.Add(obj);
    }

    private void HandleFishPools()
    {
        if (fishingMinigame == null || Player == null) return;

        foreach (var data in Landmarks)
        {
            foreach (var instance in data.SpawnedInstances)
            {
                if (!instance) continue;

                float dist = Vector3.Distance(Player.transform.position, instance.transform.position);
                bool inside = dist <= data.FishSpawnRadius;

                if (inside && !data.PlayerInsideRadius)
                {
                    // Add fish to pools
                    foreach (var fish in data.IslandFishPool)
                    {
                        if (!fish.caughtAtNight)
                        {
                            fishingMinigame.DayFishPool.Add(new FishingMinigame.FishEntry
                            {
                                fish = fish.fish,
                                rarity = fish.rarity,
                                locked = fish.locked
                            });
                        }
                        else
                        {
                            fishingMinigame.NightFishPool.Add(new FishingMinigame.FishEntry
                            {
                                fish = fish.fish,
                                rarity = fish.rarity,
                                locked = fish.locked
                            });
                        }
                    }
                    data.PlayerInsideRadius = true;
                }
                else if (!inside && data.PlayerInsideRadius)
                {
                    // Remove fish from pools
                    foreach (var fish in data.IslandFishPool)
                    {
                        fishingMinigame.DayFishPool.RemoveAll(e => e.fish == fish.fish);
                        fishingMinigame.NightFishPool.RemoveAll(e => e.fish == fish.fish);
                    }
                    data.PlayerInsideRadius = false;
                }
            }
        }
    }

    private bool TryGetValidSpawnPosition(LandmarkData data, out Vector3 result, bool useFullRing)
    {
        const int maxAttempts = 25;
        result = Vector3.zero;

        for (int i = 0; i < maxAttempts; i++)
        {
            float minRadius = useFullRing ? NoSpawnRadius : SpawnRadius;
            float distance = Random.Range(minRadius, DespawnRadius);
            Vector2 offset2D = Random.insideUnitCircle.normalized * distance;

            Vector3 candidate = Player.transform.position + new Vector3(offset2D.x, 0f, offset2D.y);

            if (Vector3.Distance(Player.transform.position, candidate) < NoSpawnRadius) continue;
            if (IsTooCloseToOtherLandmarks(candidate, data.SpaceRequired)) continue;

            result = candidate;
            return true;
        }
        return false;
    }

    private bool TryGetValidLargeIslandPosition(LandmarkData data, out Vector3 result)
    {
        const int maxAttempts = 25;
        result = Vector3.zero;

        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = Random.Range(0f, 360f);
            float radius = Random.Range(LargeIslandSpawnRadius - LargeIslandSpawnRadiusBounds,
                                        LargeIslandSpawnRadius + LargeIslandSpawnRadiusBounds);

            Vector3 candidate = Player.transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                                                                        0f,
                                                                        Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
            result = candidate;
            return true;
        }
        return false;
    }

    private bool IsTooCloseToOtherLandmarks(Vector3 position, float minDistance)
    {
        foreach (var type in Landmarks)
        {
            foreach (var instance in type.SpawnedInstances)
            {
                if (!instance) continue;
                if (Vector3.Distance(position, instance.transform.position) < minDistance) return true;
            }
        }
        return false;
    }

    private bool DespawnDistantLandmarks()
    {
        bool anyDespawned = false;

        foreach (var data in Landmarks)
        {
            for (int i = data.SpawnedInstances.Count - 1; i >= 0; i--)
            {
                GameObject obj = data.SpawnedInstances[i];
                if (!obj)
                {
                    data.SpawnedInstances.RemoveAt(i);
                    continue;
                }

                float distance = Vector3.Distance(Player.transform.position, obj.transform.position);
                float limit = data.IsLargeIsland ? LargeIslandSpawnRadius + LargeIslandSpawnRadiusBounds : DespawnRadius;

                if (distance >= limit)
                {
                    Destroy(obj);
                    data.SpawnedInstances.RemoveAt(i);
                    anyDespawned = true;
                }
            }
        }

        return anyDespawned;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LandmarkSpawner.LandmarkData))]
    public class LandmarkDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty nameProp = property.FindPropertyRelative("LandmarkName");
            string displayName = nameProp != null ? nameProp.stringValue : label.text;
            label.text = string.IsNullOrEmpty(displayName) ? "Unnamed Landmark" : displayName;

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }

    private void OnDrawGizmos()
    {
        if (Player == null) return;

        // Regular island radii
        Gizmos.color = Color.green;
        DrawCircle(Player.transform.position, SpawnRadius);
        Gizmos.color = Color.yellow;
        DrawCircle(Player.transform.position, DespawnRadius);

        // Large island radii
        Gizmos.color = Color.red;
        DrawCircle(Player.transform.position, LargeIslandSpawnRadius);
        Gizmos.color = new Color(1f, 0.5f, 1f);
        DrawCircle(Player.transform.position, LargeIslandSpawnRadius - LargeIslandSpawnRadiusBounds);
        DrawCircle(Player.transform.position, LargeIslandSpawnRadius + LargeIslandSpawnRadiusBounds);

        // Landmark SpaceRequired / FishSpawnRadius for each landmark
        foreach (var data in Landmarks)
        {
            foreach (var instance in data.SpawnedInstances)
            {
                if (!instance) continue;

                Gizmos.color = Color.red;
                DrawCircle(instance.transform.position, data.SpaceRequired);

                Gizmos.color = Color.blue;
                DrawCircle(instance.transform.position, data.FishSpawnRadius);
            }
        }
    }

    private void DrawCircle(Vector3 center, float radius, int segments = 64)
    {
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)) * radius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
#endif
}
