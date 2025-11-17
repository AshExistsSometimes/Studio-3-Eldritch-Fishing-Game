using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MenuSaveManager : MonoBehaviour
{
    public static MenuSaveManager instance;

    [Header("UI")]
    public Button LoadGameButton;

    public string saveFilePath = "FishGameSaveData.Sav";

    private enum PendingAction { None, NewGame, LoadGame }
    private PendingAction pending = PendingAction.None;
    private string targetScene = "Level";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

            UpdateLoadButtonState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void UpdateLoadButtonState()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFilePath);
        bool hasSave = File.Exists(path);

        if (LoadGameButton != null)
            LoadGameButton.interactable = hasSave;

        Debug.Log($"MenuSaveManager: Checking save file at {path} — Exists: {hasSave}");
    }

    public void StartNewGame(string sceneName)
    {
        pending = PendingAction.NewGame;
        targetScene = sceneName;

        // Use your SceneLoader to load the scene so its behaviour remains intact
        var loader = FindObjectOfType<SceneLoader>();
        if (loader != null)
            loader.LoadScene(sceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName); // fallback
    }

    public void LoadGame(string sceneName)
    {
        pending = PendingAction.LoadGame;
        targetScene = sceneName;

        var loader = FindObjectOfType<SceneLoader>();
        if (loader != null)
            loader.LoadScene(sceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName); // fallback
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only react if we intended to do something
        if (pending == PendingAction.None) return;

        // We want DeathManager to be present in scene. Wait one frame so Awake/Start run.
        StartCoroutine(ApplyPendingActionNextFrame());
    }

    private IEnumerator ApplyPendingActionNextFrame()
    {
        // Wait a frame so all Awake/Start have run in the loaded scene
        yield return null;

        // Find the DeathManager instance in the loaded scene
        var deathManager = FindObjectOfType<DeathManager>();
        if (deathManager == null)
        {
            Debug.LogError("MenuSaveManager: DeathManager not found in scene. Cannot apply save/load action.");
            Cleanup();
            yield break;
        }

        if (pending == PendingAction.NewGame)
        {
            Debug.Log("MenuSaveManager: New Game requested — clearing save data.");
            // Clear the save file and keep scene defaults
            deathManager.ClearSaveData();

            UpdateLoadButtonState();
        }
        else if (pending == PendingAction.LoadGame)
        {
            Debug.Log("MenuSaveManager: Load Game requested — loading save file and applying data.");
            deathManager.LoadDataFromFile();
            deathManager.ApplyLoadedData();
        }

        // Done — cleanup MenuSaveManager
        Cleanup();
    }

    private void Cleanup()
    {
        pending = PendingAction.None;
        targetScene = "";

        // Unsubscribe and destroy this helper object
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        Destroy(gameObject);
    }
}
