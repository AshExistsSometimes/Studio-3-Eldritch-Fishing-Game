using UnityEngine;

public class HastursCrown : MonoBehaviour
{
    public SceneManager sceneManager;
    [Header("Day Fog")]
    public Color FogColour = Color.yellow;
    public float FogDensity = 0.001f;
    [Header("Night Fog")]
    public Color NightFogColour = Color.yellow;
    public float NightFogDensity = 0.001f;

    private void Awake()
    {
        sceneManager = GameObject.Find("> GameManager").GetComponent<SceneManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            sceneManager.FogOverwritten = false;
        }
    }
}
