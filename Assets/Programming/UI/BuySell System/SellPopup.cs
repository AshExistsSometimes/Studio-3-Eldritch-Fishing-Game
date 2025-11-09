using UnityEngine;
using TMPro;

public class SellPopup : MonoBehaviour
{
    public static SellPopup Instance;

    [Header("References")]
    public TMP_Text PopupText; // Single text field now

    [Header("Popup Settings")]
    public float Lifetime = 1.2f;        // How long before fade starts
    public float FadeDuration = 0.5f;    // Fade time
    public float MoveSpeed = 30f;        // Upward motion speed

    private CanvasGroup group;
    private float timer = 0f;

    private void Awake()
    {
        Instance = this;
        group = gameObject.GetComponent<CanvasGroup>();

        if (group == null)
            group = gameObject.AddComponent<CanvasGroup>();

        group.alpha = 0f;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        timer += Time.deltaTime;

        // floating motion upward
        transform.position += Vector3.up * MoveSpeed * Time.deltaTime;

        // Fade out after lifetime
        if (timer > Lifetime)
        {
            float fadeT = (timer - Lifetime) / FadeDuration;
            group.alpha = Mathf.Lerp(1f, 0f, fadeT);

            if (fadeT >= 1f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void ShowPopup(Vector3 screenPos, int moneyAmount, float weirdnessAmount, bool vendorAppliesWeirdness)
    {
        timer = 0f;
        group.alpha = 1f;
        transform.position = screenPos;
        gameObject.SetActive(true);

        // Base money text with color
        PopupText.text = "<color=#E5D360>+$" + moneyAmount + "</color>";

        // Append weirdness only if the vendor applies it
        if (vendorAppliesWeirdness)
        {
            Debug.Log("Applying Weirdness Text");
            PopupText.text += " <color=#C860E5>(+" + weirdnessAmount + " W)</color>";
        }
    }
}
