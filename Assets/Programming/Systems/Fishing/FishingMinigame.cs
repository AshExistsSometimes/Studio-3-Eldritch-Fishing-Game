using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    // public InventoryManager inventory;

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
        // inventory = InventoryManager.instance;
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
        AddToInventory();
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
    private void SelectFishForGame() // Randomly selects a fish from the active pool based on rarity and scene weirdness.
    {
        // Determine time pool
        List<FishEntry> activePool = sceneManager.IsNight ? NightFishPool : DayFishPool;

        // rarity weighting
        FishRarity chosenRarity;
        float roll = Random.value;
        if (roll < CommonThreshold) chosenRarity = FishRarity.Common;
        else if (roll < UncommonThreshold) chosenRarity = FishRarity.Uncommon;
        else if (roll < OddThreshold) chosenRarity = FishRarity.Odd;
        else if (roll < WeirdThreshold) chosenRarity = FishRarity.Weird;
        else chosenRarity = FishRarity.Eldritch;

        List<FishSO> validFish = new List<FishSO>();

        foreach (var entry in activePool)
        {
            if (entry.locked) continue;
            if (entry.rarity != chosenRarity) continue;
            if (sceneManager.Weirdness < entry.fish.weirdnessLevel) continue;

            validFish.Add(entry.fish);
        }

        Debug.Log($"[FishingMinigame] Selected rarity: {chosenRarity}, Weirdness: {sceneManager.Weirdness}, Pool size: {activePool.Count}");
        if (validFish.Count > 0)
        {
            selectedFish = validFish[Random.Range(0, validFish.Count)];
        }
        else
        {
            // fallback to common unlocked fish
            validFish.Clear();
            foreach (var entry in activePool)
            {
                if (!entry.locked && sceneManager.Weirdness >= entry.fish.weirdnessLevel)
                {
                    Debug.Log($"Checking {entry.fish.fishName}: Rarity={entry.rarity}, Locked={entry.locked}, WeirdnessReq={entry.fish.weirdnessLevel}");
                    validFish.Add(entry.fish);
                }
            }
            if (validFish.Count > 0)
                selectedFish = validFish[Random.Range(0, validFish.Count)];
            else
                Debug.LogWarning("No valid fish found in pool!");
        }
    }

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
        if (selectedFish == null || selectedFish.fishPrefab == null)
        {
            Debug.LogWarning("No valid fish prefab found on selectedFish!");
            return;
        }
        else
        {
            Debug.Log($"Fish '{selectedFish.fishName}' would be added to inventory, showing in world for now.");
            DropFishOnGround();
        }

        // Check inventory capacity
        //if (inventory != null && inventory.CheckForInventoryFull())
        //{
        //    Debug.Log("Inventory is full. Fish placed in world instead.");
        //    DropFishOnGround();
        //}
        //else
        //{
        //    Debug.Log($"Fish '{selectedFish.fishName}' would be added to inventory, showing in world for now.");
        //    DropFishOnGround();
        //}
    }

    private void DropFishOnGround()
    {
        // Determine where to spawn the fish (player position or ground nearby)
        Vector3 spawnPosition = FishDropPoint.position;

        // Instantiate the fish prefab
        GameObject fishObject = Instantiate(selectedFish.fishPrefab, spawnPosition, Quaternion.identity);

        // Apply size scaling
        fishObject.transform.localScale = Vector3.one * selectedFishSize;
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

