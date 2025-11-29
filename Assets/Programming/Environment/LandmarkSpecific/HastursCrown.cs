using UnityEngine;
using UnityEngine.SceneManagement;

public class HastursCrown : MonoBehaviour
{
    public SceneManager sceneManager;
    [Header("Day Fog")]
    public Color FogColour = Color.yellow;
    public float FogDensity = 0.001f;
    [Header("Night Fog")]
    public Color NightFogColour = Color.yellow;
    public float NightFogDensity = 0.001f;

    [Header("Water")]
    public Color foamOverrideColour = Color.white;
    public Color shallowOverrideColour = Color.yellow;
    public Color deepOverrideColour = Color.yellow;

    private void Awake()
    {
        sceneManager = GameObject.Find("> GameManager").GetComponent<SceneManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Fog Colour Overwrite
            sceneManager.FogOverwritten = true;
            if (sceneManager.IsDay)
            {
                sceneManager.FogOverwriteColour = FogColour;
                sceneManager.FogOverwriteDensity = FogDensity;
            }
            else
            {
                sceneManager.FogOverwriteColour = NightFogColour;
                sceneManager.FogOverwriteDensity = NightFogDensity;
            }

            // Ocean Colour Overwrite
            sceneManager.OceanColourOverwritten = true;

            sceneManager.FoamOverrideColour = foamOverrideColour;
            sceneManager.ShallowOverrideColour = shallowOverrideColour;
            sceneManager.DeepOverrideColour = deepOverrideColour;

            sceneManager.UpdateOceanColour();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            // Reset Fog
            sceneManager.FogOverwritten = false;

            // Reset Ocean Material
            sceneManager.OceanColourOverwritten = false;
            sceneManager.UpdateOceanColour();
        }
    }
}
