using UnityEngine;

public class Colourchangetest : MonoBehaviour
{
    public Material waterMaterial;

    public Color FoamDefaultColour = Color.white;
    public Color ShallowDefaultColour = Color.blue;
    public Color DeepDefaultColour = Color.blue;

    public Color FoamOverrideColour = Color.white;
    public Color ShallowOverrideColour = Color.blue;
    public Color DeepOverrideColour = Color.blue;

    public bool OceanColourOverwritten = false;

    private void Start()
    {
        FoamDefaultColour = waterMaterial.GetColor("_Foam_Colour");
        ShallowDefaultColour = waterMaterial.GetColor("_Shallow_Colour");
        DeepDefaultColour = waterMaterial.GetColor("_Deep_Colour");
    }

    private void Update()
    {
        ChangeColour();
    }
    public void ChangeColour()
    {
        if (OceanColourOverwritten)
        {
            waterMaterial.SetColor("_Foam_Colour", FoamOverrideColour);
            waterMaterial.SetColor("_Shallow_Colour", ShallowOverrideColour);
            waterMaterial.SetColor("_Deep_Colour", DeepOverrideColour);
        }
        else
        {
            waterMaterial.SetColor("_Foam_Colour", FoamDefaultColour);
            waterMaterial.SetColor("_Shallow_Colour", ShallowDefaultColour);
            waterMaterial.SetColor("_Deep_Colour", DeepDefaultColour);
        }   
    }
}
