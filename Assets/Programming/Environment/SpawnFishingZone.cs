using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFishingZone : MonoBehaviour
{
    public FishingMinigame fishingLogic;

    [Header("Fish Pool")]
    public List<FishEntry> DefaultDayFishPool = new List<FishEntry>();
    public List<FishEntry> DefaultNightFishPool = new List<FishEntry>();
    [Space]
    public List<FishEntry> SpawnIslandDayFishPool = new List<FishEntry>();
    public List<FishEntry> SpawnIslandNightFishPool = new List<FishEntry>();

    [Serializable]
    public class FishEntry
    {
        public FishSO fish;
        public FishingMinigame.FishRarity rarity;
        public bool locked = false;
    }

    private bool DefaultsSaved = false;

    private void Awake()
    {
        DefaultDayFishPool = new List<FishEntry>();
        foreach (var entry in fishingLogic.DayFishPool)
        {
            DefaultDayFishPool.Add(new FishEntry
            {
                fish = entry.fish,
                rarity = entry.rarity,
                locked = entry.locked
            });
        }

        DefaultNightFishPool = new List<FishEntry>();
        foreach (var entry in fishingLogic.NightFishPool)
        {
            DefaultNightFishPool.Add(new FishEntry
            {
                fish = entry.fish,
                rarity = entry.rarity,
                locked = entry.locked
            });
        }

        DefaultsSaved = true;

        SetPoolsToSpawnAreaList();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!DefaultsSaved) return;
        if (other.CompareTag("Player"))
        {
            SetPoolsToSpawnAreaList();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!DefaultsSaved) return;
        if (other.CompareTag("Player"))
        {
            ResetPoolsToDefault();
        }
    }


    // FUNCTIONS


    private void SetPoolsToSpawnAreaList()
    {
        // Remove all default fish from their pools, and add the spawn area fish // 
        
        // REMOVE //
        // Remove default night fish from pool
        foreach (var fish in DefaultNightFishPool)
        {
            fishingLogic.NightFishPool.RemoveAll(e => e.fish == fish.fish);
        }
        // Remove default day fish from pool
        foreach (var fish in DefaultDayFishPool)
        {
            fishingLogic.DayFishPool.RemoveAll(e => e.fish == fish.fish);
        }

        // ADD //
        // Add Default Night Fish to Pool
        foreach (var fish in SpawnIslandNightFishPool)
        {
            fishingLogic.NightFishPool.Add(new FishingMinigame.FishEntry
            {
                fish = fish.fish,
                rarity = fish.rarity,
                locked = fish.locked
            });
        }
        // Add Default Day Fish to Pool
        foreach (var fish in SpawnIslandDayFishPool)
        {
            fishingLogic.DayFishPool.Add(new FishingMinigame.FishEntry
            {
                fish = fish.fish,
                rarity = fish.rarity,
                locked = fish.locked
            });
        }
    }

    private void ResetPoolsToDefault()
    {
        // remove the spawn area fish, then add all default fish back into their pools //
        
        // REMOVE //
        // Remove spawn island night fish from pool
        foreach (var fish in SpawnIslandNightFishPool)
        {
            fishingLogic.NightFishPool.RemoveAll(e => e.fish == fish.fish);
        }
        // Remove spawn island day fish from pool
        foreach (var fish in SpawnIslandDayFishPool)
        {
            fishingLogic.DayFishPool.RemoveAll(e => e.fish == fish.fish);
        }

        // ADD //
        // Add Default Night Fish to Pool
        foreach (var fish in DefaultNightFishPool)
        {
            fishingLogic.NightFishPool.Add(new FishingMinigame.FishEntry
            {
                fish = fish.fish,
                rarity = fish.rarity,
                locked = fish.locked
            });
        }
        // Add Default Day Fish to Pool
        foreach (var fish in DefaultDayFishPool)
        {
            fishingLogic.DayFishPool.Add(new FishingMinigame.FishEntry
            {
                fish = fish.fish,
                rarity = fish.rarity,
                locked = fish.locked
            });
        }
    }
}
