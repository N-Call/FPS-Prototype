using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image hoverImage;
    public Image initialImage;

    public UpgradeData upgradeData;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("point is down");
        GameManager.instance.BuyUpgrade(upgradeData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverImage != null)
        {
            hoverImage.gameObject.SetActive(true); // Show hover image
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverImage.gameObject.SetActive(false); // Hide hover image
    }
}