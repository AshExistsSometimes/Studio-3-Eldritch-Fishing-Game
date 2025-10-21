using TMPro;
using UnityEngine;

////////////////////////////////////////////////////////////////////
public class CreditsText : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float creditsScrollSpeed;

    [Header("Credits Text")]
    [SerializeField][TextArea] private string creditsTextContent;
    ////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        //Assigns text of credits based on given var
        GetComponent<TextMeshProUGUI>().text = creditsTextContent;
    }

    ////////////////////////////////////////////////////////////////////
    private void FixedUpdate()
    {
        transform.position = transform.position + new Vector3(0,creditsScrollSpeed,0);
    }

    ////////////////////////////////////////////////////////////////////

}
