using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (sceneName == "Level")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        Time.timeScale = 1.0f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void LoadGameButton()
    {
        //Load Game
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
