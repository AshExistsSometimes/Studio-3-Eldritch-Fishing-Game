using System;
using System.IO;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    [Header("Save Settings")]
    [Tooltip("Between 0 and 24")]
    public float TimeOfDayToSave = 5f;

    [Header("File Save Path")]
    [Tooltip("Changes nothing if alterd")]
    public string SaveFilePath = "";

    [Header("References")]
    public BoatController boatController;
    public BoatHealthManager boatHealthManager;
    public BoatAttachmentsRecorder boatAttachments;
    public SceneManager sceneManager;
    public EconomyManager economyManager;
    public DebtManager debtManager;
    public Inventory inventory;

    [Header("Crossbow Objects")]
    public GameObject CrossbowBuyable;
    public GameObject CrossbowBoat;

    [Header("Boat")]
    public float SavedBoatCurrentHP;
    public float SavedBoatMaxHP;
    public float SavedBoatMaxSpeed;
    public float SavedBoatTurnSpeed;
    public float SavedBoatCurrentFuel;
    public float SavedBoatMaxFuel;

    public bool SavedCrossbowUnlockState;

    [Header("Scene")]
    public float SavedWeirdness;
    public int SavedDay;

    [Header("Money")]
    public int SavedMoney;

    [Header("Debt")]
    public int SavedDaysUntilDue;
    public int SavedDebtsUntilPaidOff;
    public int SavedDebtRemaining;
    public int SavedTotalDebt;
    public bool SavedDebtPaidOffState;

    [Header("Inventory")]
    public int SavedPlayerInvSize;
    public int SavedBoatInvSize;

    public static DeathManager Instance { get; private set; }

    [Serializable]
    public class SaveFileData
    {
        public float SavedBoatCurrentHP;
        public float SavedBoatMaxHP;
        public float SavedBoatMaxSpeed;
        public float SavedBoatTurnSpeed;
        public float SavedBoatCurrentFuel;
        public float SavedBoatMaxFuel;

        public bool SavedCrossbowUnlockState;

        public float SavedWeirdness;
        public int SavedDay;

        public int SavedMoney;

        public int SavedDaysUntilDue;
        public int SavedDebtsUntilPaidOff;
        public int SavedDebtRemaining;
        public int SavedTotalDebt;
        public bool SavedDebtPaidOffState;

        public int SavedPlayerInvSize;
        public int SavedBoatInvSize;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Build save file path
        SaveFilePath = Path.Combine(Application.persistentDataPath, "FishGameSaveData.sav");

        // Try auto-load (your existing logic)
        if (File.Exists(SaveFilePath))
        {
            LoadDataFromFile();
            ApplyLoadedData();
            Debug.Log("Existing save detected – loaded automatically.");
        }
        else
        {
            Debug.Log("No save found – using default values.");
        }

        ApplyCrossbowState();
    }
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }


    private void ApplyCrossbowState()
    {
        if (!CrossbowBuyable || !CrossbowBoat) return;

        if (SavedCrossbowUnlockState == false)
        {
            CrossbowBuyable.SetActive(true);
            CrossbowBoat.SetActive(false);
        }
        else
        {
            CrossbowBuyable.SetActive(false);
            CrossbowBoat.SetActive(true);
        }
    }

    // ---------------------------------------------------------------
    //                       SAVE FUNCTIONS
    // ---------------------------------------------------------------

    public void SaveProgress()
    {
        Debug.Log("Data Saved!");

        SavedBoatCurrentHP = boatHealthManager.HP;
        SavedBoatMaxHP = boatHealthManager.MaxHP;
        SavedBoatMaxSpeed = boatController.walkSpeed;
        SavedBoatTurnSpeed = boatController.turnSpeed;

        SavedCrossbowUnlockState = boatAttachments.CrossbowUnlocked;

        SavedWeirdness = sceneManager.Weirdness;
        SavedDay = sceneManager.DayTracker;

        SavedMoney = economyManager.Currency;

        SavedDaysUntilDue = debtManager.daysUntilDue;
        SavedDebtsUntilPaidOff = debtManager.DebtsUntilPaidOff;
        SavedDebtRemaining = debtManager.DebtRemaining;
        SavedTotalDebt = debtManager.TotalDebtAmount;
        SavedDebtPaidOffState = debtManager.DebtFullyPaid;

        SavedPlayerInvSize = inventory.invSize;
        SavedBoatInvSize = inventory.boatInvSize;
    }

    public void SaveDataToFile()
    {
        SaveFilePath = Path.Combine(Application.persistentDataPath, "FishGameSaveData.sav");

        SaveFileData data = new SaveFileData
        {
            SavedBoatCurrentHP = SavedBoatCurrentHP,
            SavedBoatMaxHP = SavedBoatMaxHP,
            SavedBoatMaxSpeed = SavedBoatMaxSpeed,
            SavedBoatTurnSpeed = SavedBoatTurnSpeed,
            SavedBoatCurrentFuel = SavedBoatCurrentFuel,
            SavedBoatMaxFuel = SavedBoatMaxFuel,

            SavedCrossbowUnlockState = SavedCrossbowUnlockState,

            SavedWeirdness = SavedWeirdness,
            SavedDay = SavedDay,

            SavedMoney = SavedMoney,

            SavedDaysUntilDue = SavedDaysUntilDue,
            SavedDebtsUntilPaidOff = SavedDebtsUntilPaidOff,
            SavedDebtRemaining = SavedDebtRemaining,
            SavedTotalDebt = SavedTotalDebt,
            SavedDebtPaidOffState = SavedDebtPaidOffState,

            SavedPlayerInvSize = SavedPlayerInvSize,
            SavedBoatInvSize = SavedBoatInvSize
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SaveFilePath, json);

        Debug.Log("SAVE FILE WRITTEN TO: " + SaveFilePath);
    }

    // ---------------------------------------------------------------
    //                       LOAD FUNCTIONS
    // ---------------------------------------------------------------

    public void LoadDataFromFile()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.LogWarning("NO SAVE FILE FOUND AT: " + SaveFilePath);
            return;
        }

        string json = File.ReadAllText(SaveFilePath);
        SaveFileData data = JsonUtility.FromJson<SaveFileData>(json);

        SavedBoatCurrentHP = data.SavedBoatCurrentHP;
        SavedBoatMaxHP = data.SavedBoatMaxHP;
        SavedBoatMaxSpeed = data.SavedBoatMaxSpeed;
        SavedBoatTurnSpeed = data.SavedBoatTurnSpeed;
        SavedBoatCurrentFuel = data.SavedBoatCurrentFuel;
        SavedBoatMaxFuel = data.SavedBoatMaxFuel;

        SavedCrossbowUnlockState = data.SavedCrossbowUnlockState;

        SavedWeirdness = data.SavedWeirdness;
        SavedDay = data.SavedDay;

        SavedMoney = data.SavedMoney;

        SavedDaysUntilDue = data.SavedDaysUntilDue;
        SavedDebtsUntilPaidOff = data.SavedDebtsUntilPaidOff;
        SavedDebtRemaining = data.SavedDebtRemaining;
        SavedTotalDebt = data.SavedTotalDebt;
        SavedDebtPaidOffState = data.SavedDebtPaidOffState;

        SavedPlayerInvSize = data.SavedPlayerInvSize;
        SavedBoatInvSize = data.SavedBoatInvSize;

        Debug.Log("SAVE FILE LOADED FROM: " + SaveFilePath);
    }

    public void ApplyLoadedData()
    {
        boatHealthManager.HP = SavedBoatCurrentHP;
        boatHealthManager.MaxHP = SavedBoatMaxHP;
        boatController.walkSpeed = SavedBoatMaxSpeed;
        boatController.turnSpeed = SavedBoatTurnSpeed;

        boatAttachments.CrossbowUnlocked = SavedCrossbowUnlockState;

        sceneManager.Weirdness = SavedWeirdness;
        sceneManager.DayTracker = SavedDay;

        economyManager.Currency = SavedMoney;

        debtManager.daysUntilDue = SavedDaysUntilDue;
        debtManager.DebtsUntilPaidOff = SavedDebtsUntilPaidOff;
        debtManager.DebtRemaining = SavedDebtRemaining;
        debtManager.TotalDebtAmount = SavedTotalDebt;
        debtManager.DebtFullyPaid = SavedDebtPaidOffState;

        inventory.invSize = SavedPlayerInvSize;
        inventory.boatInvSize = SavedBoatInvSize;
    }

    public void ReloadSceneAndLoadSave()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var loader = FindObjectOfType<SceneLoader>();
        if (loader != null)
        {
            loader.LoadScene(sceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }

    // ---------------------------------------------------------------
    //                      CLEAR SAVE DATA
    // ---------------------------------------------------------------

    public void ClearSaveData()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
            Debug.Log("SAVE FILE DELETED: " + SaveFilePath);
        }
        else
        {
            Debug.Log("NO SAVE TO DELETE.");
        }

        // Reset values to defaults
        SavedCrossbowUnlockState = false;
        ApplyCrossbowState();

        Debug.Log("Save data cleared – new game state active.");
    }
}
