using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WarningPopupController : MonoBehaviour
{
    public GameObject warningPopup;

    public Button[] buttonsToToggle;

    private void Update()
    {
        if (warningPopup.activeSelf)
        {
            foreach (var button in buttonsToToggle) 
            {
                button.interactable = false;
            }
        }
        else
        {
            foreach (var button in buttonsToToggle)
            {
                button.interactable = true;
            }
        }
    }

    public void TriggerWarningPopup()
    {
        if (File.Exists(MenuSaveManager.instance.SaveFilePath))
        {
            warningPopup.SetActive(true);
        }
        else
        {
            MenuSaveManager.instance.StartNewGame("Level");
        }
    }
}
