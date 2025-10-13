using TMPro;
using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////
public class DialogueManager : MonoBehaviour
{
    //Instance of DialogueManager
    public static DialogueManager instance { get; private set; }

    //References for UI Elements
    public GameObject DialogueParent;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public GameObject invisButtonPrefab;
    public GameObject responseButtonPrefab;
    public Transform buttonParent;

    ////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        //Ensures singleton nature of instance variable
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //Hides UI at game start
        DisableDialogueUI();
    }

    ////////////////////////////////////////////////////////////////////
    public void StartDialogue(string speakerName, DialogueNode node)
    {
        //Displays dialogue UI
        EnableDialogueUI();

        //Displays respective text 
        speakerNameText.text = speakerName;
        dialogueText.text = node.dialogueText;

        //Removes all preexisting buttons
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }


        //Checks number of responses to create according number of buttons

        //Checks if current node is final node
        if (node.availableResponses.Count == 0)
        {
            //Creates invisible button covering full screen that ends dialogue upon press
            GameObject buttonObj = Instantiate(invisButtonPrefab, buttonParent);
            buttonObj.GetComponent<Button>().onClick.AddListener(() => DisableDialogueUI());
        }
        //Checks if  dialogue has no responses
        else if (node.availableResponses.Count == 1 && node.availableResponses[0].responseText == "")
        {
            //Creates invisible button covering full screen that continues dialogue upon press
            GameObject buttonObj = Instantiate(invisButtonPrefab, buttonParent);

            // Setup button to trigger SelectResponse when clicked
            buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(node.availableResponses[0], speakerName));
        }
        //If dialogue has responses
        else
        {
            foreach (DialogueResponse response in node.availableResponses)
            {
                GameObject buttonObj = Instantiate(responseButtonPrefab, buttonParent);
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;

                // Setup button to trigger SelectResponse when clicked
                buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response, speakerName));
            }
        }
    }

    ////////////////////////////////////////////////////////////////////
    public void SelectResponse(DialogueResponse response, string speakerName)
    {
        StartDialogue(speakerName, response.nextDialogueNode);
    }

    ////////////////////////////////////////////////////////////////////
    public void DisableDialogueUI()
    {
        DialogueParent.SetActive(false);
    }

    ////////////////////////////////////////////////////////////////////
    private void EnableDialogueUI()
    {
        DialogueParent.SetActive(true);
    }

    ////////////////////////////////////////////////////////////////////
    public bool CheckForDialogueActive()
    {
        return DialogueParent.activeSelf;
    }

    ////////////////////////////////////////////////////////////////////
}
