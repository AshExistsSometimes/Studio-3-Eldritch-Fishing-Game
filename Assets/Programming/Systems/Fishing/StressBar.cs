using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StressBar : MonoBehaviour
{
    [Header("UI References")]
    public Slider tensionSlider;        // Visual bar for progress
    public Text resultText;             // Displays result text

    [Header("Gameplay Settings")]
    public float maxBarValue = 10f;     // Max value for success
    public float decayRate = 1f;        // How quickly the bar decreases per second
    public float increasePerClick = 1f; // How much the bar increases per right click
    public Transform spawnPoint;        // Where the item spawns on success

    [Header("Reward Items")]
    public List<ItemSO> rewardItems = new List<ItemSO>(); // Possible item rewards

    public float currentBarValue; // Current progress of the bar
    public bool gameActive = false;
    public bool gameEnded = false;

    void Start()
    {
        // Initialize the slider
        currentBarValue = maxBarValue / 2f; // Start halfway, gives tension from start
        tensionSlider.maxValue = maxBarValue;
        tensionSlider.value = currentBarValue;
        resultText.text = "";
    }

    void Update()
    {
        if (!gameActive || gameEnded)
            return;

        // Decrease bar gradually over time
        currentBarValue -= decayRate * Time.deltaTime;
        currentBarValue = Mathf.Clamp(currentBarValue, 0, maxBarValue);
        tensionSlider.value = currentBarValue;

        // Check for input (right click to increase tension)
        if (Input.GetMouseButtonDown(1))
        {
            currentBarValue += increasePerClick;
            currentBarValue = Mathf.Clamp(currentBarValue, 0, maxBarValue);
            tensionSlider.value = currentBarValue;
        }

        // Check for win or fail
        if (currentBarValue <= 0f)
        {
            CatchFailed();
        }
        else if (currentBarValue >= maxBarValue)
        {
            CatchSuccessful();
        }
    }

    /// Starts the fishing minigame. Can be called externally.
    public void StartFishing()
    {
        gameActive = true;
        gameEnded = false;
        currentBarValue = maxBarValue / 2f;
        tensionSlider.value = currentBarValue;
        resultText.text = "";
    }

    /// Called on minigame won
    /// Spawns a random ItemSO's prefab as the reward.
    private void CatchSuccessful()
    {
        gameEnded = true;
        gameActive = false;
        resultText.text = "Caught!";

        if (rewardItems.Count > 0)
        {
            int randomIndex = Random.Range(0, rewardItems.Count);
            ItemSO randomItem = rewardItems[randomIndex];

            if (randomItem != null && randomItem.ModelPrefab != null)
            {
                Instantiate(randomItem.ModelPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }

    // Called on minigame failed
    private void CatchFailed()
    {
        gameEnded = true;
        gameActive = false;
        resultText.text = "Escaped!";
    }
}

