using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    public bool MinigameOpen = false;
    private bool MinigameCanClose = false;
    [Header("Minigame Values")]
    [Range(0f, 10f)]
    public float FishProgress = 5f;
    [Space]
    public bool isFishing = false;
    [Space]
    public float DefaultDifficulty = 2f;// How large the target is
    public float DefaultSpeed = 1f; // The speed the target moves
    public float DefaultPersistance = 5f;// The speed the progress bar increases
    public float DefaultStrength = 5f;// The speed the progress bar decreases

    [Header("Fish Values")]
    public float FishDifficulty = 1f;// How large the target is
    public float FishSpeed = 1f;
    public float FishPersistance = 1f;// The speed the progress bar increases
    public float FishStrength = 1f;// The speed the progress bar decreases - Determined by size on FishSO

    [Header("References")]
    public Image target;
    public Image cursor;
    public RectTransform bounds;
    public GameObject MinigameUI;
    public Slider ProgressSlider;
    public TMP_Text ResultText;
    public PlayerMovement player;

    [Header("Bar Values")]
    float width = 0f;

    public float TargetSize = 0.2f;
    [Range(0f, 1f)]
    public float TargetCentre = 0.5f;

    public float cursorPoint = 0.5f;

    public float ReductionSpeed = 0.33f;
    public float RodBoostAmnt = 0.5f;

    [Header("DEBUG")]
    public bool InTarget = false;
    public bool MovingRight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        width = bounds.rect.width;
        isFishing = false;
        MinigameUI.SetActive(false);
        MinigameOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        ProgressSlider.value = FishProgress;

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            InitializeMinigame();
        }
        if (MinigameCanClose && Input.anyKeyDown)
        {
            CloseMinigame();
        }



        target.rectTransform.localPosition = new Vector3((TargetCentre * width) - (width / 2f), 0, 0);
        Rect rect = target.rectTransform.rect;

        rect.width = TargetSize * width;

        target.rectTransform.sizeDelta = new Vector2(rect.width, 0);

        cursor.rectTransform.localPosition = new Vector3((cursorPoint * width) - (width / 2f), 0, 0);



        cursorPoint -= Time.deltaTime * ReductionSpeed;

        Debug.Log(WithinBounds());

        if (Input.GetKeyDown(KeyCode.Mouse0) && isFishing)// REPLACE WHEN INPUT MANAGER ADDED
        {
            cursorPoint += RodBoostAmnt;// TODO - MAKE SMOOTHER
        }

        if (cursorPoint <= 0)
        {
            cursorPoint = 0;
        }

        if (cursorPoint >= 1)
        {
            cursorPoint = 1;
        }


        if (isFishing)
        {
            HandleTargetMovement();
            UpdateProgressBar();
            DetectWin();
        }
    }

    private void HandleTargetMovement()
    {
        if (MovingRight)
        {
            TargetCentre += Time.deltaTime * (DefaultSpeed * FishSpeed);
        }
        else if (!MovingRight)
        {
            TargetCentre -= Time.deltaTime * (DefaultSpeed * FishSpeed);
        }

        if ((TargetCentre + TargetSize) >= 1f || (TargetCentre - TargetSize) <= 0f)
        {
            FlipDirection();
        }
    }

    private void UpdateProgressBar()
    {
        // Progress bar
        if (WithinBounds())
        {
            FishProgress += Time.deltaTime * (DefaultPersistance - FishPersistance);// Higher persistence on fish makes it go slower
        }
        else if (!WithinBounds())
        {
            FishProgress -= Time.deltaTime * (DefaultStrength + FishStrength);// Higher Strength(Size) on fish makes it go faster
        }
        else if (!isFishing)
        {
            FishProgress = 5f;
        }
    }

    public void DetectWin()
    {
        if (FishProgress <= 0.0f)
        {
            FailMinigame();
        }
        else if (FishProgress >= 10.0f)
        {
            WinMinigame();
        }
        else
        {
            ResultText.gameObject.SetActive(false);
        }
    }


    public void WinMinigame()
    {
        ResultText.gameObject.SetActive(true);
        ResultText.text = "Caught It!";
        isFishing = false;
        StartCoroutine(MinigameCanEnd());
    }

    public void FailMinigame()
    {
        ResultText.gameObject.SetActive(true);
        ResultText.text = "It Got Away!";
        isFishing = false;
        StartCoroutine(MinigameCanEnd());
    }

    public void CloseMinigame()
    {
        ResultText.gameObject.SetActive(false);
        MinigameUI.SetActive(false);
        MinigameOpen = false;
        player.canMove = true;
    }


    public void InitializeMinigame()
    {
        ResultText.gameObject.SetActive(false);
        InitializeStats();
        FishProgress = 5f;
        TargetCentre = 0.5f;
        MinigameUI.SetActive(true);
        player.canMove = false;
        isFishing = true;
        MinigameOpen = true;
    }

    public void InitializeStats()
    {
        TargetSize = (DefaultDifficulty / 10) * FishDifficulty;
    }

    public void FlipDirection()
    {
        if (MovingRight)
        {
            MovingRight = false;
        }
        else
        {
            MovingRight = true;
        }
    }

    public IEnumerator MinigameCanEnd()
    {
        yield return new WaitForSeconds(0.25f);
        MinigameCanClose = true;
    }    
    

    public bool WithinBounds()
    {
        if (cursorPoint >= TargetCentre - (TargetSize /2f) && cursorPoint <= TargetCentre + (TargetSize / 2f))// Cursor is in bounds of target
        {
            return true;
            InTarget = true;
        }
        else// Cursor is not in bounds of target
        {
            return false;
            InTarget = false;
        }
    }
}
