using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CreditsText : MonoBehaviour
{
    [Header("Parameters")]
    public float DefaultScrollSpeed = 0.75f;
    public float SpaceSpeedMultiplier = 1.5f;
    private float ScrollSpeed;

    [Header("Scroll Positions")]
    public float StartYPos = -500f;
    public float EndYPos = 1200f;

    [Header("Credits Text")]
    [SerializeField][TextArea] private string creditsTextContent;

    [Header("Events")]
    public UnityEvent OnCreditsFinished;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Assigns text of credits
        GetComponent<TextMeshProUGUI>().text = creditsTextContent;

        ScrollSpeed = DefaultScrollSpeed;
    }

    private void OnEnable()
    {
        // Reset to start position whenever the credits screen is enabled
        Vector3 startPos = rectTransform.anchoredPosition;
        startPos.y = StartYPos;
        rectTransform.anchoredPosition = startPos;
    }

    private void Update()
    {
        // Fast Forward with Space
        if (Input.GetKey(KeyCode.Space))
        {
            ScrollSpeed = DefaultScrollSpeed * SpaceSpeedMultiplier;
        }
        else
        {
            ScrollSpeed = DefaultScrollSpeed;
        }

        // Move up
        Vector3 pos = rectTransform.anchoredPosition;
        pos.y += ScrollSpeed * Time.deltaTime;
        rectTransform.anchoredPosition = pos;

        // Check for completion
        if (rectTransform.anchoredPosition.y >= EndYPos)
        {
            OnCreditsFinished?.Invoke();
        }
    }
}

