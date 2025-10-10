using UnityEngine;
using UnityEngine.UI;

public class Stressbar : MonoBehaviour
{
    // Assignable Slider Values
    [SerializeField] private Slider timerSlider; // The Slider object
    [SerializeField] private Text timerText; // The Text object
    [SerializeField] private float gameTime; // time for the minigame

    private bool fishCaught;
    void Start()
    {
        fishCaught = false;
        timerSlider.maxValue = gameTime;
        timerSlider.value = gameTime;
    }

    // Update is called once per frame
    void Update()
    {
        float time = gameTime - Time.time;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time - minutes * 60);

        string textTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        if (time <= 0)
        {
            fishCaught = true;
        }

        if (fishCaught == false)
        {
            timerText.text = textTime;
            timerSlider.value = time;
        }
    }
}
