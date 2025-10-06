using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns and manages landmark prefabs around a player within defined radii,
/// respecting spawn limits, spacing, rarity, and despawn behavior.
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
        [Range(0f, 1f)] public float Rarity = 1f;

        [Tooltip("Range of random scale multipliers applied to the landmark.")]
        public Vector2 ScaleVariation = new Vector2(1f, 1f);

        [Tooltip("Minimum required distance between this landmark and another landmark.")]
        public float SpaceRequired = 50f;

        [Tooltip("Range of random height variation on the Y axis, applied relative to world Y=0.")]
        public Vector2 HeightVariation = new Vector2(0f, 0f);

        [HideInInspector] public List<GameObject> SpawnedInstances = new List<GameObject>();
        [HideInInspector] public int TargetSpawnCount = 0; // Random target number between 0 and MaxSpawnNum
    }

    [Header("Landmark Settings")]
    [Tooltip("List of landmarks that can be spawned.")]
    public List<LandmarkData> Landmarks = new List<LandmarkData>();

    [Header("Spawner Settings")]
    [Tooltip("Player reference used as a center point for spawning/despawning.")]
    public GameObject Player;

    [Tooltip("Inner radius from player where landmarks can begin to spawn.")]
    public float SpawnRadius = 200f;

    [Tooltip("Outer radius from player where landmarks can spawn and beyond which they despawn.")]
    public float DespawnRadius = 250f;

    [Tooltip("Radius around world origin where spawning is disallowed.")]
    public float NoSpawnRadius = 100f;

    [Tooltip("If true, initial landmark generation occurs at Start().")]
    public bool InitializeAtStart = true;

    private bool initialized = false;
    private bool respawnRequested = false; // Flag to trigger regeneration after despawns

    private void Start()
    {
        if (InitializeAtStart)
        {
            InitializeSpawnTargets();
            InitialGenerate();
        }
    }

    /// <summary>
    /// Initializes random target spawn counts for each landmark type.
    /// </summary>
    private void InitializeSpawnTargets()
    {
        foreach (var data in Landmarks)
        {
            data.TargetSpawnCount = Random.Range(0, data.MaxSpawnNum + 1);
        }
        initialized = true;
    }

    /// <summary>
    /// Performs an initial spawn pass across the full valid area (NoSpawnRadius to DespawnRadius).
    /// </summary>
    private void InitialGenerate()
    {
        foreach (var data in Landmarks)
        {
            if (data.Landmark == null)
                continue;

            for (int i = 0; i < data.TargetSpawnCount; i++)
            {
                if (Random.value > data.Rarity)
                    continue;

                Vector3 spawnPos;
                if (TryGetValidSpawnPosition(data, out spawnPos, useFullRing: true))
                {
                    SpawnLandmarkInstance(data, spawnPos);
                }
            }
        }
    }

    private void Update()
    {
        if (Player == null)
            return;

        if (!initialized)
            InitializeSpawnTargets();

        // Handle despawning and trigger regeneration if needed
        bool anyDespawned = DespawnDistantLandmarks();
        if (anyDespawned)
            respawnRequested = true;

        // Perform respawn after despawn cleanup
        if (respawnRequested)
        {
            RegenerateAfterDespawn();
            respawnRequested = false;
        }

        TrySpawnLandmarks();
    }

    /// <summary>
    /// Attempts to spawn landmarks during runtime, using the normal spawn ring (SpawnRadius–DespawnRadius).
    /// </summary>
    private void TrySpawnLandmarks()
    {
        foreach (var data in Landmarks)
        {
            if (data.Landmark == null)
                continue;

            if (data.SpawnedInstances.Count >= data.TargetSpawnCount)
                continue;

            if (Random.value > data.Rarity)
                continue;

            Vector3 spawnPos;
            if (TryGetValidSpawnPosition(data, out spawnPos, useFullRing: false))
            {
                SpawnLandmarkInstance(data, spawnPos);
            }
        }
    }

    /// <summary>
    /// Called after any despawn event, attempts to spawn new landmarks up to MaxSpawnNum.
    /// </summary>
    private void RegenerateAfterDespawn()
    {
        foreach (var data in Landmarks)
        {
            int currentCount = data.SpawnedInstances.Count;
            int maxAllowed = data.MaxSpawnNum;
            if (currentCount >= maxAllowed)
                continue;

            int missing = maxAllowed - currentCount;
            for (int i = 0; i < missing; i++)
            {
                if (Random.value > data.Rarity)
                    continue;

                Vector3 spawnPos;
                if (TryGetValidSpawnPosition(data, out spawnPos, useFullRing: false))
                {
                    SpawnLandmarkInstance(data, spawnPos);
                }
            }
        }
    }

    /// <summary>
    /// Instantiates and configures a new landmark instance.
    /// </summary>
    private void SpawnLandmarkInstance(LandmarkData data, Vector3 position)
    {
        Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        GameObject obj = Instantiate(data.Landmark, position, rotation);
        obj.name = data.LandmarkName;

        float randomScale = Random.Range(data.ScaleVariation.x, data.ScaleVariation.y);
        obj.transform.localScale = Vector3.one * randomScale;

        float heightOffset = Random.Range(data.HeightVariation.x, data.HeightVariation.y);
        obj.transform.position = new Vector3(obj.transform.position.x, heightOffset, obj.transform.position.z);

        data.SpawnedInstances.Add(obj);
    }

    /// <summary>
    /// Attempts to find a valid spawn position for a landmark.
    /// </summary>
    private bool TryGetValidSpawnPosition(LandmarkData data, out Vector3 result, bool useFullRing)
    {
        const int maxAttempts = 25;
        result = Vector3.zero;

        for (int i = 0; i < maxAttempts; i++)
        {
            float minRadius = useFullRing ? NoSpawnRadius : SpawnRadius;
            float distance = Random.Range(minRadius, DespawnRadius);
            Vector2 offset2D = Random.insideUnitCircle.normalized * distance;

            Vector3 candidate = new Vector3(
                Player.transform.position.x + offset2D.x,
                0f,
                Player.transform.position.z + offset2D.y
            );

            if (candidate.magnitude < NoSpawnRadius)
                continue;

            if (IsTooCloseToOtherLandmarks(candidate, data.SpaceRequired))
                continue;

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
                if (instance == null)
                    continue;

                if (Vector3.Distance(position, instance.transform.position) < minDistance)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Despawns landmarks too far from the player.
    /// Returns true if any were removed this frame.
    /// </summary>
    private bool DespawnDistantLandmarks()
    {
        bool anyDespawned = false;

        foreach (var data in Landmarks)
        {
            for (int i = data.SpawnedInstances.Count - 1; i >= 0; i--)
            {
                GameObject obj = data.SpawnedInstances[i];
                if (obj == null)
                {
                    data.SpawnedInstances.RemoveAt(i);
                    continue;
                }

                float distance = Vector3.Distance(Player.transform.position, obj.transform.position);
                if (distance >= DespawnRadius)
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawCircle(Vector3.zero, NoSpawnRadius);

        if (Player != null)
        {
            Gizmos.color = Color.green;
            DrawCircle(Player.transform.position, SpawnRadius);
            Gizmos.color = Color.yellow;
            DrawCircle(Player.transform.position, DespawnRadius);
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

