using UnityEngine;

public class GameManagerDebug : MonoBehaviour
{
    public bool debugEnabled = false;
    [Space]
    // SceneManager
    private SceneManager sceneManager;
    [Header ("Scene Manager")]
    public bool SceneManagerDebugOn = false;
    public bool ForceOverwriteOcean = false;

    private void Start()
    {
        sceneManager = GetComponentInParent<SceneManager>();
    }

    private void Update()
    {
        if (!debugEnabled) { return; }

        DebugSceneManager();
    }

    public void DebugSceneManager()
    {
        if (!SceneManagerDebugOn) { return; }
        sceneManager.OceanColourOverwritten = ForceOverwriteOcean;
        sceneManager.UpdateOceanColour();
    }
}
