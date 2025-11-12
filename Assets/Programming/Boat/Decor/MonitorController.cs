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
    public SceneManager sceneManager;
    public DebtManager debtManager;

    public bool ScreenOn = false;

    private string debtString;

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
        // HP to Text Health Bar
        float hpPercent = BoatHPManager.HP / BoatHPManager.MaxHP;
        int hpFilledSegments = Mathf.Clamp(Mathf.CeilToInt(hpPercent * 10f), 0, 10);
        int hpEmptySegments = 10 - hpFilledSegments;

        string hpFilled = new string('■', hpFilledSegments);
        string hpEmpty = new string('□', hpEmptySegments);
        string HPbar = hpFilled + hpEmpty;

        // Fuel to Text Fuel Bar
        //float fuelPercent = CURRENT FUEL / MAX FUEL;
        //int fuelFilledSegments = Mathf.Clamp(Mathf.CeilToInt(hpPercent * 10f), 0, 10);
        //int fuelEmptySegments = 10 - hpFilledSegments;

        //string fuelFilled = new string('■', fuelFilledSegments);
        //string fuelEmpty = new string('□', fuelEmptySegments);
        //string Fuelbar = fuelFilled + fuelEmpty;

        if (!debtManager.DebtPaid)
        {
            string debtString = new string("<br><br><size=100%>Debt to Pay: <size=130%>" + debtManager.DebtRemaining + "<size=100%>");
        }
        else
        {
            string debtString = new string("<br><br><size=100%>Debt paid off<br><size=100%>");
        }

        DisplayText.text =
            "<color=#9CB79F>HP:  <color=#B76262>" + HPbar +// HP Display
            "<color=#9CB79F><br>Fuel:■■■■■■■■■■" +// Fuel Display
             debtString +
            "<br><br><br><size=150%>" + sceneManager.ClockTime + ":00";// Time Display
    }
}
