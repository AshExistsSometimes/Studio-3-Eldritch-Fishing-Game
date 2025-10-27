using UnityEngine;
using UnityEngine.EventSystems;

public class RememberSelectedButton : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject lastSelectedButton;

    private void Reset()
    {
        eventSystem = FindFirstObjectByType<EventSystem>();

        if (!eventSystem)
        {
            return;
        }

        lastSelectedButton = eventSystem.firstSelectedGameObject;
    }

    private void Update()
    {
        if (!eventSystem)
        {
            return;
        }

        if (eventSystem.currentSelectedGameObject &&
            lastSelectedButton != eventSystem.currentSelectedGameObject)
        {
            lastSelectedButton = eventSystem.currentSelectedGameObject;
        }

        if (!eventSystem.currentSelectedGameObject && lastSelectedButton)
        {
            eventSystem.SetSelectedGameObject(lastSelectedButton);
        }
    }
}
