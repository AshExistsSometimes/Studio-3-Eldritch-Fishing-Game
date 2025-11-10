using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public Button buttonToSelect;
    private PlayerMovement player;
    private bool isPaused;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !isPaused)
        {
            OnPause();
        }
        else if (Input.GetKeyDown(KeyCode.P) && isPaused)
        {
            OnResume();
        }
    }
    public void OnPause()
    {
        buttonToSelect.Select();
        player.enabled = false;
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnResume()
    {
        player.enabled = true;
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
