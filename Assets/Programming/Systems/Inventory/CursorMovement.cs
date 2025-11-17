using UnityEngine;

public class CursorMovement : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        MoveCursor();
    }


    public void MoveCursor()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            Input.mousePosition,
            null,
            out Vector2 localPoint
        );

        GetComponent<RectTransform>().anchoredPosition = localPoint;
    }
}
