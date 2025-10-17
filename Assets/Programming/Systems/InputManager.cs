using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Subsystems;

public class InputManager : MonoBehaviour
{
    // Singleton instance
    public static InputManager Instance { get; private set; }

    [Serializable]
    public class NamedKeybind
    {
        public string inputName;
        public KeyCode keyCode;
    }

    [Header("Keybinds")]
    [Tooltip("Assign string to KeyCode mappings here.")]
    public List<NamedKeybind> keybinds = new List<NamedKeybind>();

    // Internal lookup dictionary for fast access
    private Dictionary<string, KeyCode> _lookup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildLookup();
    }

    // Build dictionary for quick lookups
    private void BuildLookup()
    {
        _lookup = new Dictionary<string, KeyCode>(StringComparer.OrdinalIgnoreCase);
        foreach (var bind in keybinds)
        {
            if (!string.IsNullOrEmpty(bind.inputName))
            {
                _lookup[bind.inputName] = bind.keyCode;
            }
        }
    }

    /// <summary>
    /// Static method to get KeyCode from input name globally.
    /// </summary>
    public static KeyCode GetKeyCode(string inputName)
    {
        if (Instance == null)
        {
            Debug.LogError("InputManager instance not found in scene.");
            return KeyCode.None;
        }

        if (Instance._lookup == null)
            Instance.BuildLookup();

        if (Instance._lookup.TryGetValue(inputName, out var key))
            return key;

        Debug.LogWarning($"KeyCode not found for input name: {inputName}");
        return KeyCode.None;
    }
}

 //Example CODE BLOCK

 //   if (Input.GetKeyDown(InputManager.GetKeyCode("InputName")))
 //   {
 //       // Logic
 //   }

