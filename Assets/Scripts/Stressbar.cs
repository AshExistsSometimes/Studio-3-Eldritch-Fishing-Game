using UnityEngine;
using UnityEngine.UI;

public class Stressbar : MonoBehaviour
{
    // Assignable Slider Values
    [SerializeField] private Slider timerSlider; // The Slider object
    [SerializeField] private Text timerText; // The Text object
    [SerializeField] private Text resultText; // The Result Text object
    [SerializeField] private float gameTime; // time for the minigame
    private float catchBar; // the actual stress bar
    private bool fishCaught; // boolean saying the fish is caught
    private bool gameEnd; // boolean stating the game is over
    void Start()
    {
        fishCaught = false;
        catchBar = gameTime;
        timerSlider.maxValue = catchBar;
        timerSlider.value = catchBar;
    }

    // Update is called once per frame
    void Update()
    {
        float time = gameTime - Time.time;
        catchBar = catchBar - Time.time;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time - minutes * 60);

        string textTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        string resText = "";

        // Checks if the game is over
        if (gameEnd == false)
        {
            timerText.text = textTime;
            resultText.text = resText;
            timerSlider.value = time;
        }

        // Checks the time
        if (time <= 0)
        {
            // checks if the catch bar is empty
            if (catchBar <= 0)
            {
                fishCaught = false;
            }
            else
            {
                fishCaught = true;
            }

            if (fishCaught == false)
            {
                resText = "Escaped!";
            }
            else
            {
                resText = "Caught!";
            }
            gameEnd = true;
        }
    }
}
