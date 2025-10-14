using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class shittyScropt : MonoBehaviour
{
    public RectTransform bounds;

    float width = 0f;

    public Image target;
    public Image cursor;

    public float size = 0.2f;
    public float point = 0.5f;

    public float cursorPoint = 0.5f;

    public float reduce = 0.33f;
    public float boost = 0.5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        width = bounds.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        //print((cursorPoint * width) - (width / 2f));

        target.rectTransform.localPosition = new Vector3((point * width) - (width /2f),0,0);
        Rect rect = target.rectTransform.rect;

        rect.width = size * width;

        target.rectTransform.sizeDelta = new Vector2(rect.width, 0);

        cursor.rectTransform.localPosition = new Vector3((cursorPoint * width) - (width / 2f), 0, 0);

        cursorPoint -= Time.deltaTime * reduce;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            cursorPoint += boost;
        }

        print(WithinBounds());

        if (cursorPoint <= 0)
        {
            cursorPoint = 0;
        }

        if (cursorPoint >= 1)
        {
            cursorPoint = 1;
        }
    }

    public bool WithinBounds()
    {
        if (cursorPoint >= point - (size /2f) && cursorPoint <= point + (size / 2f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
