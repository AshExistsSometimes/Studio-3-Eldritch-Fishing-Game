using System.IO;
using UnityEngine;

public class WarningPopupController : MonoBehaviour
{
    public GameObject warningPopup;

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
