using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[ExecuteAlways]
public class SceneManager : MonoBehaviour
{
    public static SceneManager instance { get; private set; }

    private AnalyticsManager analytics;

    [Header("DEBUG")]
    public int ClockTime;
    [SerializeField] private float MinutesPerDay;

    public bool pauseDaylightCycle = false;
    public bool IsDay;
    public bool IsNight;
    [Space]
    public bool isLoading = true;

    [Header("REFERENCES")]
    public GameObject LoadingScreen;
    //public GameObject soundManager;
    [Space]
    public Light DirectionalLight;
    public TMP_Text CalenderText;
    public DebtManager debtManager;
    public DeathManager deathManager;

    [Header("Fog")]
    public Gradient FogGradient;
    public float DayFogDensity = 0.005f;
    public float DayFogLerpTime = 10f;
    [Space]
    public float NightFogDensity = 0.02f;
    public float NightFogLerpTime = 10f;
    [Space]
    [Space]
    [Space]
    public bool FogOverwritten = false;
    public Color FogOverwriteColour = Color.red;
    public float FogOverwriteDensity = 0.02f;
    [Space]
    [Header("Ocean Overwrite")]
    public Material waterMaterial;
    [Space]
    public Color FoamDefaultColour = Color.white;
    public Color ShallowDefaultColour = Color.blue;
    public Color DeepDefaultColour = Color.blue;
    [Space]
    public Color FoamOverrideColour = Color.white;
    public Color ShallowOverrideColour = Color.blue;
    public Color DeepOverrideColour = Color.blue;
    [Space]
    public bool OceanColourOverwritten = false;
    [Space]
    [Space]
    [Header("VARIABLES")]
    public float Weirdness = 0f;// THE BIG ONE
    public float WeirdnessIncreaseAmount = 1f;
    public float WeirdnessIncreaseSpeed = 1f;
    private bool WeirdnessCanIncrease = true;
    [Space]
    [Space]
    public int DayTracker = 0;
    private bool DayTickedOver = false;
    [Range(0, 24)] 
    public float TimeOfDay = 0f;
    [Range(0, 24)]
    public float TimeOfDayAtStart = 8f;
    [Space]
    public float SecondsInAnHour = 10f;// 10 makes day night/cycle 4 minutes | 60 makes a day/night cycle 24 minutes | 3600 makes a day/night cycle take 24 hours
    [Space]
    [Range(0, 24)]
    public float MorningHour = 6f;
    [Range(0, 24)]
    public float EveningHour = 18f;
    [Space]
    public float InitialisationTime = 0.15f;// TO DO: MAKE THIS SCALE WITH TIME.DELTATIME TO SYNC TO PC SPECS

    [Header("Events")]
    public UnityEvent LoadFinished;
    [Space]
    public UnityEvent IsDawn;
    public UnityEvent IsDusk;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        analytics = AnalyticsManager.Instance;
    }
    private void Start()
    {
        Weirdness = 0;// Set to last saved weirdness
        TimeOfDay = 0f;// Ensures lighting initialises correctly to give the void effect
        OceanColourOverwritten = false;
        UpdateOceanColour();

        if (LoadingScreen != null)
        {
            isLoading = true;
            LoadingScreen.SetActive(true);
            //soundManager.SetActive(false);
            StartCoroutine(RunInitialisation(InitialisationTime));
        }
    }
    private void Update()
    {
        if (WeirdnessCanIncrease)
        {
            StartCoroutine(TickUpWeirdness());
        }

        UpdateLightAndFog();

        if (MorningHour < TimeOfDay && TimeOfDay < EveningHour)// 6am and 6pm | Daytime Check
        {
            IsDay = true;
            IsNight = false;
            if (!DayTickedOver)
            {
                DayTickedOver = true;
                CalenderText.text = DayTracker.ToString();

                debtManager.DayPassed();
                deathManager.SaveProgress(); Debug.Log("Attempting to Save Data");
                deathManager.SaveDataToFile(); Debug.Log("Attempting to Save Data to file");
                analytics.AddString("New Day started, player is on: Day " + DayTracker + " - Weirdness is: " + Weirdness);
                analytics.UpdateHeaderInFile();

                DayTracker += 1;
            }

            if (!FogOverwritten)
            {
                RenderSettings.fogDensity = Mathf.Lerp(NightFogDensity, DayFogDensity, DayFogLerpTime);
            }

            IsDawn.Invoke();
        }
        else if (TimeOfDay < MorningHour)
        {
            IsDay = false;
            IsNight = true;
            if (DayTickedOver)
            {
                DayTickedOver = false;
            }

            if (!FogOverwritten)
            {
                RenderSettings.fogDensity = Mathf.Lerp(DayFogDensity, NightFogDensity, NightFogLerpTime);
            }
            IsDusk.Invoke();
        }
        else if (TimeOfDay > EveningHour)
        {
            IsDay = false;
            IsNight = true;
            if (DayTickedOver)
            {
                DayTickedOver = false;
            }
            RenderSettings.fogDensity = Mathf.Lerp(DayFogDensity, NightFogDensity, NightFogLerpTime);
            IsDusk.Invoke();
        }



        if (MorningHour > TimeOfDay - 1 || TimeOfDay > EveningHour + 1)// 6am and 6pm | NightTime Check
        {
            Debug.Log("Sun off");
            DirectionalLight.intensity = 0f;
        }
        else
        {
            Debug.Log("Sun on");
            DirectionalLight.intensity = 2f;
        }
    }

    public void UpdateOceanColour()// Call once when override starts, and once when it ends
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
    private void UpdateLightAndFog()
    {
        if (Application.isPlaying)
        {
            if (!pauseDaylightCycle)
            {
                TimeOfDay += Time.deltaTime * (SecondsInAnHour / 100);
            }
            TimeOfDay %= 24; // Clamp between 0 and 24
            ClockTime = Mathf.FloorToInt(TimeOfDay %= 24);
            UpdateLighting(TimeOfDay / 24f);
            MinutesPerDay = ((SecondsInAnHour * 24) / 60);

            if (!FogOverwritten)
            {
                Color currentFogColour = FogGradient.Evaluate(TimeOfDay / 24);
                RenderSettings.fogColor = currentFogColour;
            }
            else
            {
                Color currentFogColour = FogOverwriteColour;
                RenderSettings.fogColor = currentFogColour;

                RenderSettings.fogDensity = FogOverwriteDensity;
            }
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
            MinutesPerDay = ((SecondsInAnHour * 24) / 60);
            TimeOfDay = 0f;
        }
    }

    private void UpdateLighting(float timePercent)
    {
        if (DirectionalLight != null)
        {
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 120f, -100f));//84
        }
    }

    public void LockNightTime()
    {
        TimeOfDay = 0;
        pauseDaylightCycle = true;
    }


    private void OnValidate()
    {
        if (DirectionalLight != null)
        {
            return;
        }

        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if(light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                }
            }
        }
    }

    public IEnumerator RunInitialisation(float loadingTime)
    {
        yield return new WaitForSeconds(loadingTime / 1.33333333f);// 3 Quarters of loading time
        isLoading = false;//                                          Gap Between the initialisation and the player being able to
        LoadFinished.Invoke();//                                      see and hear ensures that even on a lower end computer,
        TimeOfDay = TimeOfDayAtStart;//                               the player will see nothing loading in.
        yield return new WaitForSeconds(loadingTime / 4f);//          1 Quarter of loading time 
        LoadingScreen.SetActive(false);
        //soundManager.SetActive(true);
    }

    public IEnumerator TickUpWeirdness()
    {
        if (WeirdnessCanIncrease)
        {
            WeirdnessCanIncrease = false;
            Weirdness += WeirdnessIncreaseAmount;
            yield return new WaitForSeconds(WeirdnessIncreaseSpeed);
            WeirdnessCanIncrease = true;
        }
    }

}
