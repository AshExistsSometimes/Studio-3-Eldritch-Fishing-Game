using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderstormEvent : MonoBehaviour
{
    private SceneManager sceneManager;
    private Transform player;
    private FishingMinigame fishMinigame;

    [Header("Lightning Settings")]
    public GameObject LightningPrefab;
    public float LightningMinRadius = 10f;
    public float LightningMaxRadius = 30f;
    public float LightningHeight = 25f;
    public float LightningExistsTime = 0.5f;

    [Header("Fog Settings")]
    public Color FlashFogColour = Color.white;
    public float FlashFogDensity = 0.0015f;

    [Header("Thunder Timing")]
    public Vector2 StrikeIntervalRange = new Vector2(5f, 15f);
    public float LightningSoundDelay = 0.35f;
    public AudioClip ThunderClip;
    public AudioSource ThunderAudioSource;

    [Header("Storm Follow Settings")]
    public float FollowHeight = 75f;

    private bool eventActive = true;

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
        player = GameObject.FindGameObjectWithTag("Player").transform;
        fishMinigame = FindFirstObjectByType<FishingMinigame>();
    }

    private void OnEnable()
    {
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

        StartCoroutine(ThunderstormRoutine());
    }

    private void Update()
    {
        if (eventActive && player != null)
        {
            transform.position = player.position + Vector3.up * FollowHeight;
        }
    }

    private IEnumerator ThunderstormRoutine()
    {
        while (eventActive)
        {
            float wait = UnityEngine.Random.Range(StrikeIntervalRange.x, StrikeIntervalRange.y);
            yield return new WaitForSeconds(wait);

            yield return ThunderStrike();
        }
    }

    private IEnumerator ThunderStrike()
    {
        sceneManager.FogOverwriteColour = FlashFogColour;
        sceneManager.FogOverwriteDensity = FlashFogDensity;
        sceneManager.FogOverwritten = true;

        Vector3 pos = GetRandomLightningPosition();
        GameObject lightning = Instantiate(LightningPrefab, pos, Quaternion.identity);

        if (ThunderAudioSource != null && ThunderClip != null)
        {
            StartCoroutine(PlayThunderDelayed());
        }

        yield return new WaitForSeconds(LightningExistsTime);

        if (lightning != null)
            Destroy(lightning);

        sceneManager.FogOverwritten = false;
    }

    private IEnumerator PlayThunderDelayed()
    {
        yield return new WaitForSeconds(LightningSoundDelay);
        ThunderAudioSource.PlayOneShot(ThunderClip);
    }

    private Vector3 GetRandomLightningPosition()
    {
        float radius = UnityEngine.Random.Range(LightningMinRadius, LightningMaxRadius);
        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

        return player.position + offset + new Vector3(0, LightningHeight, 0);
    }

    private void OnDisable()
    {
        sceneManager.FogOverwritten = false;

        // Remove fish from pool
        foreach (var fish in FishPool)
        {
            fishMinigame.NightFishPool.RemoveAll(e => e.fish == fish.fish);
        }
    }
}
