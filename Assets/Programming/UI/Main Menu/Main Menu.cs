using TMPro;
using UnityEngine;

////////////////////////////////////////////////////////////////////
public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject creditsUI;

    ////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        //Disables submenus on game start
        settingsUI.SetActive(false);
        creditsUI.SetActive(false);
    }

    ////////////////////////////////////////////////////////////////////
    private void Update()
    {
        GetInput();
    }

    ////////////////////////////////////////////////////////////////////
    private void GetInput()
    {
        if (creditsUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            mainMenuUI.SetActive(true);
            creditsUI.SetActive(false);
        }


    }

    ////////////////////////////////////////////////////////////////////
    public void NewGameButton()
    {
        //Play Game
    }

    ////////////////////////////////////////////////////////////////////
    public void LoadGameButton()
    {
        //Load Game
    }

    ////////////////////////////////////////////////////////////////////
    public void SettingsButton()
    {
        mainMenuUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    ////////////////////////////////////////////////////////////////////
    public void CreditsButton()
    {
        mainMenuUI.SetActive(false);
        creditsUI.SetActive(true);
    }

    ////////////////////////////////////////////////////////////////////
    public void ExitButton()
    {
        Application.Quit();
    }

    ////////////////////////////////////////////////////////////////////
}
////////////////////////////////////////////////////////////////////

