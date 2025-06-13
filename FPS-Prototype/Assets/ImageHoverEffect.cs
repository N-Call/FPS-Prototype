using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image hoverImage;
    public Image initialImage;

    void Start()
    {
      
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverImage != null)
        {
            Debug.Log("Hover");
            hoverImage.gameObject.SetActive(true); // Show hover image
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("No Hover");
        hoverImage.gameObject.SetActive(false); // Hide hover image
    }
}