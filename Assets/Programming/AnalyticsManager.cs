using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    [Header("Analytics Settings")]
    public string FilePath { get; private set; }

    private string fileNameBase = "PlayTestAnalytics_";
    private string fileExtension = ".Analyt";
    private int currentID = 0;

    // Counters
    private int fishCaught = 0;
    private int islandsVisited = 0;
    private int boatSank = 0;

    // Dictionaries to track counts
    private Dictionary<string, int> fishCaughtDict = new Dictionary<string, int>();
    private Dictionary<string, int> islandsVisitedDict = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAnalyticsFile();
    }

    private void SetupAnalyticsFile()
    {
        string directory = Application.persistentDataPath;

        // Find next available ID
        while (File.Exists(Path.Combine(directory, $"{fileNameBase}{currentID}{fileExtension}")))
        {
            currentID++;
        }

        FilePath = Path.Combine(directory, $"{fileNameBase}{currentID}{fileExtension}");

        using (StreamWriter writer = new StreamWriter(FilePath, false))
        {
            writer.WriteLine($"Analytics of Player ID : {currentID}");
            WriteHeader(writer);
            writer.WriteLine("--- LOG ---");
        }

        Debug.Log($"Analytics file created at: {FilePath}");
    }

    private void WriteHeader(StreamWriter writer)
    {
        writer.WriteLine($"Fish Caught: {fishCaught}");
        writer.WriteLine($"Islands Visited: {islandsVisited}");
        writer.WriteLine($"Times Boat Sank: {boatSank}");
        writer.WriteLine();

        writer.WriteLine("Number of Each Fish Caught:");
        foreach (var pair in fishCaughtDict.OrderByDescending(x => x.Value))
            writer.WriteLine($"{pair.Key} - {pair.Value}");
        writer.WriteLine();

        writer.WriteLine("Number of Times Visited Each Island:");
        foreach (var pair in islandsVisitedDict.OrderByDescending(x => x.Value))
            writer.WriteLine($"{pair.Key} - {pair.Value}");
        writer.WriteLine();
    }

    public void AddString(string message)
    {
        if (string.IsNullOrEmpty(FilePath)) return;

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

    public void AddFishToCounter(string fishName)
    {
        fishCaught++;
        if (fishCaughtDict.ContainsKey(fishName)) fishCaughtDict[fishName]++;
        else fishCaughtDict[fishName] = 1;

        UpdateHeaderInFile();
    }

    public void AddIslandToCounter(string islandName)
    {
        islandsVisited++;
        if (islandsVisitedDict.ContainsKey(islandName)) islandsVisitedDict[islandName]++;
        else islandsVisitedDict[islandName] = 1;

        UpdateHeaderInFile();
    }

    public void AddDeathToCounter()
    {
        boatSank++;
        UpdateHeaderInFile();
    }

    /// <summary>
    /// Updates only the header portion of the file, leaving the event log untouched.
    /// </summary>
    public void UpdateHeaderInFile()
    {
        if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath)) return;

        string[] allLines = File.ReadAllLines(FilePath);
        int logStartIndex = Array.FindIndex(allLines, l => l.StartsWith("--- LOG ---"));

        if (logStartIndex == -1)
        {
            Debug.LogError("Could not find event log marker.");
            return;
        }

        // Extract current event log
        string[] logLines = allLines.Skip(logStartIndex).ToArray();

        // Rewrite file with updated header
        using (StreamWriter writer = new StreamWriter(FilePath, false))
        {
            writer.WriteLine($"Analytics of Player ID : {currentID}");
            WriteHeader(writer);
            foreach (var line in logLines)
                writer.WriteLine(line);
        }
    }

    private void OnApplicationQuit() => UpdateHeaderInFile();
    private void OnApplicationPause(bool pause)
    {
        if (pause) UpdateHeaderInFile();
    }
}


