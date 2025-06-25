using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public float scrollSpeed = 40f;
    public float stopScroll = 1000f;
    private Vector2 startPosition;
    private RectTransform rectTransform;
    private bool isScrolling = true;
    //private bool startSet = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();  
    }

    private void OnEnable()
    {
        startPosition = new Vector2(0, -900);
        ResetScroll();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isScrolling) return;

        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

        if (rectTransform.anchoredPosition.y >= stopScroll)
        {
            isScrolling = false;
        }
    }

    public void ResetScroll()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();  
        }
        rectTransform.anchoredPosition = startPosition;
        isScrolling = true;
    }
}
