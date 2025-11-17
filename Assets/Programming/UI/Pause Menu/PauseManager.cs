using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject journalUI;
    public Button buttonToSelect;
    private PlayerController player;
    private bool isPaused;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        pauseMenu.SetActive(false);
        journalUI.SetActive(false);
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
        journalUI.SetActive(false);
        buttonToSelect.Select();
        player.enabled = false;
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnResume()
    {
        player.enabled = true;
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
