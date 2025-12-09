using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FishDetailUI : MonoBehaviour
{
    [Header("UI References")]
    public Image FishIcon;
    public TMP_Text FishName;
    public TMP_Text FishInfo;

    public void SetData(FishSO fish)
    {
        if (fish == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (FishIcon != null)
            FishIcon.sprite = fish.fishIcon; // assumes FishSO has fishIcon (you said it does)

        if (FishName != null)
            FishName.text = fish.fishName;

        if (FishInfo != null)
            FishInfo.text = fish.fishInfo;
    }
}
