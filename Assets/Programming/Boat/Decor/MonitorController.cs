using TMPro;
using UnityEngine;

public class MonitorController : MonoBehaviour
{
    [Header ("Screen")]
    public GameObject Screen;
    public TMP_Text DisplayText;

    [Header("Data Sources")]
    public BoatController Boat;
    public BoatHealthManager BoatHPManager;

    public bool ScreenOn = false;

    private void Update()
    {
        UpdateOffOn();

        if (!ScreenOn)
            return;

        UpdateDisplayText();
    }

    private void UpdateOffOn()
    {
        if (Boat.isMounted)
        {
            Screen.gameObject.SetActive(true);
            ScreenOn = true;
        }
        else
        {
            Screen.gameObject.SetActive(false);
            ScreenOn = false;
        }
    }

    private void UpdateDisplayText()
    {
        float hpPercent = BoatHPManager.HP / BoatHPManager.MaxHP;
        int filledSegments = Mathf.Clamp(Mathf.CeilToInt(hpPercent * 10f), 0, 10);
        int emptySegments = 10 - filledSegments;

        string filled = new string('■', filledSegments);
        string empty = new string('□', emptySegments);
        string HPbar = filled + empty;

        DisplayText.text =
            "HP:  " + HPbar +
            "<br>Fuel:■■■■■■■■■■";
    }
}
