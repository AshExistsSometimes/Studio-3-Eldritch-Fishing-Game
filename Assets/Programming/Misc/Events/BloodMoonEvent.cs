using System;
using System.Collections.Generic;
using UnityEngine;

public class BloodMoonEvent : MonoBehaviour
{
    private SceneManager sceneManager;
    private FishingMinigame fishMinigame;

    [Header("Water")]
    public Color foamOverrideColour = Color.white;
    public Color shallowOverrideColour = Color.red;
    public Color deepOverrideColour = Color.red;

    [Header("Fog")]
    public Color FogColour = Color.red;
    public float FogDensity = 0.0015f;

    [Header("Fish Pool")]
    public List<FishEntry> FishPool = new List<FishEntry>();

    [Serializable]
    public class FishEntry
    {
        public FishSO fish;
        public FishingMinigame.FishRarity rarity;
        public bool locked = false;
    }

    private void Start()
    {
        sceneManager = GameObject.Find("> GameManager").GetComponent<SceneManager>();
        fishMinigame = FindFirstObjectByType<FishingMinigame>();
    }

    private void OnEnable()
    {
        // Water Override
        sceneManager.OceanColourOverwritten = true;

        sceneManager.FoamOverrideColour = foamOverrideColour;
        sceneManager.ShallowOverrideColour = shallowOverrideColour;
        sceneManager.DeepOverrideColour = deepOverrideColour;

        sceneManager.UpdateOceanColour();

        // Fog Override
        sceneManager.FogOverwritten = true;

        sceneManager.FogOverwriteColour = FogColour;
        sceneManager.FogOverwriteDensity = FogDensity;

        // Add Fish to Pool
        foreach (var fish in FishPool)
        {
            fishMinigame.NightFishPool.Add(new FishingMinigame.FishEntry
            {
                fish = fish.fish,
                rarity = fish.rarity,
                locked = fish.locked
            });
        }
    }

    private void OnDisable()
    {
        // Water Reset
        sceneManager.OceanColourOverwritten = false;
        sceneManager.UpdateOceanColour();

        // Fog Reset
        sceneManager.FogOverwritten = false;

        // Remove fish from pool
        foreach (var fish in FishPool)
        {
            fishMinigame.NightFishPool.RemoveAll(e => e.fish == fish.fish);
        }
    }
}
