using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns and manages landmark prefabs around a player within a defined ring area,
/// respecting spawn limits, spacing, and despawn behavior.
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

        [Tooltip("Range of random scale multipliers applied to the landmark.")]
        public Vector2 ScaleVariation = new Vector2(1f, 1f);

        [Tooltip("Minimum required distance between this landmark and another landmark.")]
        public float SpaceRequired = 50f;

        [Tooltip("Range of random height variation on the Y axis.")]
        public Vector2 HeightVariation = new Vector2(0f, 0f);

        [HideInInspector] public List<GameObject> SpawnedInstances = new List<GameObject>();
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

    /// <summary>
    /// Called every frame to manage spawn/despawn logic.
    /// </summary>
    private void Update()
    {
        if (Player == null)
            return;

        DespawnDistantLandmarks();
        TrySpawnLandmarks();
    }

    /// <summary>
    /// Spawns landmarks until their maximum count is reached, if valid spawn points are found.
    /// </summary>
    private void TrySpawnLandmarks()
    {
        foreach (var data in Landmarks)
        {
            if (data.Landmark == null)
                continue;

            // Skip if this landmark type has reached its max count.
            if (data.SpawnedInstances.Count >= data.MaxSpawnNum)
                continue;

            Vector3 spawnPos;
            if (TryGetValidSpawnPosition(data, out spawnPos))
            {
                // Random rotation around Y axis.
                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                GameObject obj = Instantiate(data.Landmark, spawnPos, rotation);
                obj.name = data.LandmarkName;

                // Random uniform scale.
                float randomScale = Random.Range(data.ScaleVariation.x, data.ScaleVariation.y);
                obj.transform.localScale = Vector3.one * randomScale;

                // Random height variation.
                float heightOffset = Random.Range(data.HeightVariation.x, data.HeightVariation.y);
                obj.transform.position += new Vector3(0f, heightOffset, 0f);

                data.SpawnedInstances.Add(obj);
            }
        }
    }

    /// <summary>
    /// Attempts to find a valid spawn position for a landmark, ensuring spacing and exclusion zones.
    /// </summary>
    private bool TryGetValidSpawnPosition(LandmarkData data, out Vector3 result)
    {
        const int maxAttempts = 25;
        result = Vector3.zero;

        for (int i = 0; i < maxAttempts; i++)
        {
            // Random distance within the ring band (between SpawnRadius and DespawnRadius).
            float distance = Random.Range(SpawnRadius, DespawnRadius);
            Vector2 offset2D = Random.insideUnitCircle.normalized * distance;
            Vector3 candidate = Player.transform.position + new Vector3(offset2D.x, 0f, offset2D.y);

            // Reject if within the no-spawn radius around world origin.
            if (candidate.magnitude < NoSpawnRadius)
                continue;

            // Reject if too close to other landmarks.
            if (IsTooCloseToOtherLandmarks(candidate, data.SpaceRequired))
                continue;

            result = candidate;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks whether the given position is too close to existing landmarks.
    /// </summary>
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
    /// Removes landmarks that exceed the despawn radius relative to the player.
    /// </summary>
    private void DespawnDistantLandmarks()
    {
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
                }
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draws debug gizmos in the Scene view to visualize spawn and exclusion radii.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Draw no-spawn radius (around world origin).
        Gizmos.color = Color.red;
        DrawCircle(Vector3.zero, NoSpawnRadius);

        if (Player != null)
        {
            // Draw spawn radius (inner band edge).
            Gizmos.color = Color.green;
            DrawCircle(Player.transform.position, SpawnRadius);

            // Draw despawn radius (outer band edge).
            Gizmos.color = Color.yellow;
            DrawCircle(Player.transform.position, DespawnRadius);
        }
    }

    /// <summary>
    /// Helper method to draw a flat circle in the Scene view using Gizmos.
    /// </summary>
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
