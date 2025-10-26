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
        BackToMainMenu();
    }

    ////////////////////////////////////////////////////////////////////
    public void BackToMainMenu()
    {
        mainMenuUI.SetActive(true);
        settingsUI.SetActive(false);
        creditsUI.SetActive(false);
    }

    ////////////////////////////////////////////////////////////////////
    public void NewGameButton(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
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

