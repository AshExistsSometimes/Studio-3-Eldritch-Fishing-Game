using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
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
