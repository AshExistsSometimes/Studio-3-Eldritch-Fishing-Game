using TMPro;
using UnityEngine;
using UnityEngine.UI;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|

public class DisplayItemData : MonoBehaviour
{
    [SerializeField]
    private GameObject itemInfoPanel;

    [SerializeField]
    private TMP_Text itemNameDisplay;

    [SerializeField]
    private Image itemIcon;

    [SerializeField]
    private TMP_Text itemDescription;

    [SerializeField]
    private TMP_Text itemWorth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ClosePanel();
    }

    /// <summary>
    /// Checks to see if this panel is open.
    /// </summary>
    /// <returns>TRUE if this is open.</returns>
    public bool IsPanelOpen()
    {
        return itemInfoPanel.activeSelf;
    }

    /// <summary>
    /// Closes the panel.
    /// </summary>
    public void ClosePanel()
    {
        itemInfoPanel.SetActive(false);
    }

    /// <summary>
    /// Tries to open the panel with the data. Will close if null.
    /// </summary>
    /// <param name="data">The data to shove into the UI.</param>
    public void OpenItemDescription(InvItemSO data)
    {
        // If there is no data, dont present a broken screen.
        if (data == null)
        {
            ClosePanel();
            return;
        }

        // this is all tempoary but you get the idea, set the text to the data.
        itemNameDisplay.text = data.name;

        itemIcon.sprite = data.Icon;

        itemDescription.text = data.Description;

        itemWorth.text = "£" + data.Worth.ToString();

        itemInfoPanel.SetActive(true);
    }
}
