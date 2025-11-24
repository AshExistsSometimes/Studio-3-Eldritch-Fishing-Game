using System;
using System.IO;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    [Header("Analytics Settings")]
    [Tooltip("Full path to the current analytics file.")]
    public string FilePath { get; private set; }
    public string path;

    private string fileNameBase = "PlayTestAnalytics";
    private string fileExtension = ".Analyt";
    private int currentID = 0;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAnalyticsFile();
        path = FilePath;
    }

    /// <summary>
    /// Sets up the analytics file, incrementing the ID to avoid overwriting previous runs.
    /// </summary>
    private void SetupAnalyticsFile()
    {
        string directory = Application.persistentDataPath;

        // Find the next available ID
        while (File.Exists(Path.Combine(directory, $"{fileNameBase}{currentID}{fileExtension}")))
        {
            currentID++;
        }

        FilePath = Path.Combine(directory, $"{fileNameBase}{currentID}{fileExtension}");

        try
        {
            using (StreamWriter writer = new StreamWriter(FilePath, false))
            {
                writer.WriteLine($"Analytics of Player ID : {currentID}");
            }
            Debug.Log($"Analytics file created at: {FilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create analytics file: {e}");
        }
    }

    public void AddString(string message)
    {
        if (string.IsNullOrEmpty(FilePath))
        {
            Debug.LogWarning("Analytics file path not set. Call SetupAnalyticsFile first.");
            return;
        }

        // Use Time.time to get playtime in seconds
        float playTime = Time.time;
        int hours = Mathf.FloorToInt(playTime / 3600f);
        int minutes = Mathf.FloorToInt((playTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);
        int milliseconds = Mathf.FloorToInt((playTime - Mathf.Floor(playTime)) * 1000f);

        string timestamp = $"{hours:00}:{minutes:00}:{seconds:00}.{milliseconds:000}";
        string line = $"({timestamp}) - {message}";

        try
        {
            using (StreamWriter writer = new StreamWriter(FilePath, true))
            {
                writer.WriteLine(line);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to analytics file: {e}");
        }
    }
}
