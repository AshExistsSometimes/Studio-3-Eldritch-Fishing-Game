using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    public bool MinigameOpen = false;
    private bool MinigameCanClose = false;
    [Header("Minigame Values")]
    [Range(0f, 10f)]
    public float FishProgress = 5f;
    [Space]
    public bool isFishing = false;
    [Space]
    public float DefaultDifficulty = 2f;// How large the target is
    public float DefaultSpeed = 1f; // The speed the target moves
    public float DefaultPersistance = 5f;// The speed the progress bar increases
    public float DefaultStrength = 5f;// The speed the progress bar decreases

    [Header("Fish Values")]
    private FishSO selectedFish;
    private float selectedFishSize = 1f;
    public float FishDifficulty = 1f;// How large the target is
    public float FishSpeed = 1f;
    public float FishPersistance = 1f;// The speed the progress bar increases
    public float FishStrength = 1f;// The speed the progress bar decreases - Determined by size on FishSO

    [Header("Inventory")]
     public Inventory inventory;

    [Header("References")]
    public Image target;
    public Image cursor;
    public RectTransform bounds;
    public GameObject MinigameUI;
    public Slider ProgressSlider;
    public TMP_Text ResultText;
    public PlayerMovement player;
    public Transform FishDropPoint;

    [Header("Bar Values")]
    float width = 0f;

    public float TargetSize = 0.2f;
    [Range(0f, 1f)]
    public float TargetCentre = 0.5f;

    public float cursorPoint = 0.5f;

    public float ReductionSpeed = 0.33f;
    public float RodBoostAmnt = 0.5f;
    private float boostVelocity = 0f;
    public float BoostSmoothTime = 0.15f; // How long the boost takes to fade
    public float MaxBoostVelocity = 1.5f; // Limits rapid spam speed stacking

    [Header("DEBUG")]
    public bool InTarget = false;
    public bool MovingRight = true;


    public static FishingMinigame instance { get; private set; }

    public SceneManager sceneManager;

    public enum FishRarity { Common, Uncommon, Odd, Weird, Eldritch }
    [Range(0f, 1f)] 
    public float CommonThreshold = 0.5f;
    [Range(0f, 1f)]
    public float UncommonThreshold = 0.75f;
    [Range(0f, 1f)]
    public float OddThreshold = 0.9f;
    [Range(0f, 1f)]
    public float WeirdThreshold = 0.98f;

    [System.Serializable]
    public class FishEntry
    {
        public FishSO fish;
        public FishRarity rarity;
        public bool locked = false;
    }

    [Header("Fish Pools")]
    public List<FishEntry> DayFishPool = new List<FishEntry>();
    public List<FishEntry> NightFishPool = new List<FishEntry>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (sceneManager == null)
            sceneManager = FindObjectOfType<SceneManager>();
    }

    void Start()
    {
        inventory = Inventory.Instance;
        width = bounds.rect.width;
        isFishing = false;
        MinigameUI.SetActive(false);
        MinigameOpen = false;
    }

    void Update()
    {
        ProgressSlider.value = FishProgress;

        if (Input.GetKeyDown(InputManager.GetKeyCode("DebugFishing")))
        {
            MinigameUI.SetActive(true);
            InitializeMinigame();
        }

        if (MinigameCanClose && Input.anyKeyDown)
        {
            CloseMinigame();
        }

        target.rectTransform.localPosition = new Vector3((TargetCentre * width) - (width / 2f), 0, 0);
        Rect rect = target.rectTransform.rect;

        rect.width = TargetSize * width;
        target.rectTransform.sizeDelta = new Vector2(rect.width, 0);
        cursor.rectTransform.localPosition = new Vector3((cursorPoint * width) - (width / 2f), 0, 0);

        if (isFishing)
        {
            cursorPoint -= Time.deltaTime * ReductionSpeed;
            if (boostVelocity > 0f)
            {
                cursorPoint += boostVelocity * Time.deltaTime * (1f / BoostSmoothTime);
                boostVelocity = Mathf.MoveTowards(boostVelocity, 0f, Time.deltaTime * (RodBoostAmnt / BoostSmoothTime));
            }
        }

        if (Input.GetKeyDown(InputManager.GetKeyCode("Reel")) && isFishing)
        {
            boostVelocity += RodBoostAmnt;
            boostVelocity = Mathf.Clamp(boostVelocity, 0f, MaxBoostVelocity);
        }

        cursorPoint = Mathf.Clamp(cursorPoint, 0f, 1f);

        if (isFishing)
        {
            HandleTargetMovement();
            UpdateProgressBar();
            DetectWin();
        }
    }

    private void HandleTargetMovement()
    {
        if (MovingRight)
        {
            TargetCentre += Time.deltaTime * (DefaultSpeed * FishSpeed);
        }
        else
        {
            TargetCentre -= Time.deltaTime * (DefaultSpeed * FishSpeed);
        }

        if ((TargetCentre + TargetSize) >= 1f || (TargetCentre - TargetSize) <= 0f)
        {
            FlipDirection();
        }
    }

    private void UpdateProgressBar()
    {
        if (WithinBounds())
        {
            FishProgress += Time.deltaTime * (DefaultPersistance - FishPersistance);
        }
        else
        {
            FishProgress -= Time.deltaTime * (DefaultStrength + FishStrength);
        }
    }

    public void DetectWin()
    {
        if (FishProgress <= 0.0f)
        {
            FailMinigame();
        }
        else if (FishProgress >= 10.0f)
        {
            WinMinigame();
        }
        else
        {
            ResultText.gameObject.SetActive(false);
        }
    }

    public void WinMinigame()
    {
        ResultText.gameObject.SetActive(true);
        ResultText.text = "Caught It!";
        if(inventory != null)
        {
            AddToInventory();
        }
        isFishing = false;
        StartCoroutine(MinigameCanEnd());
    }

    public void FailMinigame()
    {
        ResultText.gameObject.SetActive(true);
        ResultText.text = "It Got Away!";
        isFishing = false;
        StartCoroutine(MinigameCanEnd());
    }

    public void CloseMinigame()
    {
        ResultText.gameObject.SetActive(false);
        MinigameUI.SetActive(false);
        MinigameOpen = false;
        player.canMove = true;
    }

    public void InitializeMinigame()
    {
        MinigameCanClose = false;
        MinigameUI.SetActive(true);
        ResultText.gameObject.SetActive(false);

        // Select fish before initializing stats
        SelectFishForGame();

        InitializeStats();
        FishProgress = 5f;
        TargetCentre = 0.5f;
        player.canMove = false;
        isFishing = true;
        MinigameOpen = true;
    }
   
    // Randomly selects a fish from the active pool based on time of day, rarity, and scene weirdness. ////////////////////
    private void SelectFishForGame()
    {
        // Determine active pool based on time of day
        List<FishEntry> activePool = null;

        if (sceneManager.IsDay)
        {
            activePool = DayFishPool;
            Debug.Log("[FishingMinigame] Time of Day: DAY - Using DayFishPool");
        }
        else if (sceneManager.IsNight)
        {
            activePool = NightFishPool;
            Debug.Log("[FishingMinigame] Time of Day: NIGHT - Using NightFishPool");
        }
        else
        {
            Debug.LogWarning("[FishingMinigame] Neither IsDay nor IsNight is true! Defaulting to DayFishPool.");
            activePool = DayFishPool;
        }

        if (activePool == null || activePool.Count == 0)
        {
            Debug.LogError("[FishingMinigame] Active fish pool is empty or null! Cannot select fish.");
            return;
        }

        // Determine rarity //
        FishRarity chosenRarity;
        float roll = Random.value;
        if (roll < CommonThreshold) chosenRarity = FishRarity.Common;
        else if (roll < UncommonThreshold) chosenRarity = FishRarity.Uncommon;
        else if (roll < OddThreshold) chosenRarity = FishRarity.Odd;
        else if (roll < WeirdThreshold) chosenRarity = FishRarity.Weird;
        else chosenRarity = FishRarity.Eldritch;
        //////////////////////

        Debug.Log($"[FishingMinigame] Rarity roll: {roll:F2}, Selected rarity: {chosenRarity}");

        List<FishSO> validFish = new List<FishSO>();
        Dictionary<FishRarity, int> rarityCounts = new Dictionary<FishRarity, int>();

        // Initialize rarity counts
        foreach (FishRarity rarity in System.Enum.GetValues(typeof(FishRarity)))
            rarityCounts[rarity] = 0;

        // Filtering process with logs
        foreach (var entry in activePool)
        {
            rarityCounts[entry.rarity]++;

            if (entry.locked)
            {
                Debug.Log($"[FishingMinigame] EXCLUDED {entry.fish.fishName}: Locked");
                continue;
            }
            if (entry.rarity != chosenRarity)
            {
                Debug.Log($"[FishingMinigame] EXCLUDED {entry.fish.fishName}: Rarity mismatch ({entry.rarity} != {chosenRarity})");
                continue;
            }
            if (sceneManager.Weirdness < entry.fish.weirdnessLevel)
            {
                Debug.Log($"[FishingMinigame] EXCLUDED {entry.fish.fishName}: Requires Weirdness {entry.fish.weirdnessLevel}, current {sceneManager.Weirdness}");
                continue;
            }

            Debug.Log($"[FishingMinigame] VALID {entry.fish.fishName} (Rarity={entry.rarity}, WeirdnessReq={entry.fish.weirdnessLevel})");
            validFish.Add(entry.fish);
        }

        // Rarity distribution log
        string rarityLog = "[FishingMinigame] Pool Rarity Distribution:";
        foreach (var kvp in rarityCounts)
            rarityLog += $" {kvp.Key}={kvp.Value}";
        Debug.Log(rarityLog);

        // If valid fish found, pick one randomly
        if (validFish.Count > 0)
        {
            selectedFish = validFish[Random.Range(0, validFish.Count)];
            Debug.Log($"[FishingMinigame] Selected fish: {selectedFish.fishName}");
            return;
        }

        // Fallback - no valid fish found, try all unlocked regardless of rarity
        Debug.LogWarning("[FishingMinigame] No valid fish met conditions. Attempting unlocked fallback...");
        validFish.Clear();
        foreach (var entry in activePool)
        {
            if (!entry.locked)
            {
                Debug.Log($"[FishingMinigame] Fallback unlocked candidate: {entry.fish.fishName}");
                validFish.Add(entry.fish);
            }
        }

        if (validFish.Count > 0)
        {
            selectedFish = validFish[Random.Range(0, validFish.Count)];
            Debug.Log($"[FishingMinigame] Fallback (unlocked) fish selected: {selectedFish.fishName}");
            return;
        }

        // Fallback 2 - pick any fish of the chosen rarity
        Debug.LogError("[FishingMinigame] No unlocked fish found. Selecting random fish by rarity only as final fallback.");
        validFish.Clear();
        foreach (var entry in activePool)
        {
            if (entry.rarity == chosenRarity)
                validFish.Add(entry.fish);
        }

        if (validFish.Count > 0)
        {
            selectedFish = validFish[Random.Range(0, validFish.Count)];
            Debug.Log($"[FishingMinigame] Final fallback: selected {selectedFish.fishName} (Rarity={chosenRarity})");
        }
        else
        {
            // Absolute safety: pick SOMETHING if everything else fails
            selectedFish = activePool[Random.Range(0, activePool.Count)].fish;
            Debug.LogError($"[FishingMinigame] No valid or rarity-matched fish available. Randomly selected {selectedFish.fishName} to prevent null reference.");
        }
    }

    //////////////////////////////


    public void InitializeStats()// Initializes minigame values using the FishSO stats
    {
        FishDifficulty = selectedFish.difficulty;
        FishPersistance = selectedFish.persistence;
        FishSpeed = selectedFish.agility;

        float stepCount = Mathf.Round((selectedFish.sizeVariance.y - selectedFish.sizeVariance.x) * 10f);
        float randomStep = Random.Range(0, stepCount + 1);
        float selectedFishSize = selectedFish.sizeVariance.x + (randomStep / 10f);
        FishStrength = selectedFishSize;

        TargetSize = (DefaultDifficulty / 10) * FishDifficulty;
    }

    public void AddToInventory()
    {
        InvItemSO fishToAdd = selectedFish.InventoryItem;
        inventory.AttemptAddItemToInventory(fishToAdd);
    }

    private void DropFishOnGround()
    {
        // Logic to drop fish when inventory full
    }

    public void FlipDirection()
    {
        MovingRight = !MovingRight;
    }

    public IEnumerator MinigameCanEnd()
    {
        yield return new WaitForSeconds(0.25f);
        MinigameCanClose = true;
    }

    public bool WithinBounds()
    {
        if (cursorPoint >= TargetCentre - (TargetSize / 2f) && cursorPoint <= TargetCentre + (TargetSize / 2f))
        {
            return true;
            InTarget = true;
        }
        else
        {
            return false;
            InTarget = false;
        }
    }
}

