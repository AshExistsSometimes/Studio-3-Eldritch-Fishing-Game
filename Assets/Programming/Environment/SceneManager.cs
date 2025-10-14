using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[ExecuteAlways]
public class SceneManager : MonoBehaviour
{
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

    [Header("Fog")]
    public Gradient FogGradient;
    public float DayFogDensity = 0.005f;
    public float DayFogLerpTime = 10f;
    [Space]
    public float NightFogDensity = 0.02f;
    public float NightFogLerpTime = 10f;

    [Header("VARIABLES")]
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



    private void Start()
    {
        TimeOfDay = 0f;// Ensures lighting initialises correctly to give the void effect
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


            Color currentFogColour = FogGradient.Evaluate(TimeOfDay / 24);
            RenderSettings.fogColor = currentFogColour;

        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
            MinutesPerDay = ((SecondsInAnHour * 24) / 60);
            TimeOfDay = 0f;
        }

        if (MorningHour < TimeOfDay && TimeOfDay < EveningHour)// 6am and 6pm | Daytime Check
        {
            IsDay = true;
            IsNight = false;
            if (!DayTickedOver)
            {
                DayTickedOver = true;
                DayTracker += 1;
                CalenderText.text = DayTracker.ToString();
            }
            RenderSettings.fogDensity = Mathf.Lerp(NightFogDensity, DayFogDensity, DayFogLerpTime);
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
            RenderSettings.fogDensity = Mathf.Lerp(DayFogDensity, NightFogDensity, NightFogLerpTime);
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

}
